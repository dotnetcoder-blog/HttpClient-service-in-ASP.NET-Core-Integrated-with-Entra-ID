using Dnc.BlazorWebApp.Components;
using Dnc.BlazorWebApp.Services;
using Dnc.Services.RestClient.EntraId;
using Dnc.Services.RestClient.Extensions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Runtime.Intrinsics.X86;
using System.Text.Json.Serialization;
using System.Text.Json;
using Dnc.BlazorWebApp.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Authentication
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        options.ClientId = builder.Configuration.GetValue<string>("EntraId:ClientId");
        options.ClientSecret = builder.Configuration.GetValue<string>("EntraId:ClientSecret");
        options.TenantId = builder.Configuration.GetValue<string>("EntraId:TenantId");
        options.ResponseType = builder.Configuration.GetValue<string>("EntraId:ResponseType");
        options.Domain = builder.Configuration.GetValue<string>("EntraId:Domain");
        options.Instance = builder.Configuration.GetValue<string>("EntraId:Instance");
        options.CallbackPath = builder.Configuration.GetValue<string>("EntraId:CallbackPath");

        options.Scope.Add($"api://{builder.Configuration.GetValue<string>("CustomerApi:CustomerApiClientId")}/access_as_user");
    }, cookieOptions =>
    {
        cookieOptions.ExpireTimeSpan = TimeSpan.FromHours(3);
        cookieOptions.SlidingExpiration = false;
        cookieOptions.Cookie.Name = "BlazorAuthCookie";
        cookieOptions.EventsType = typeof(CustomCookieHandler);
    }, OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.AddScoped<CustomCookieHandler>();


builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

// Configure Authorization
//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = options.DefaultPolicy;
//});


/**************************************************************/
//This piece of code configures your Blazor Server app to:

//1-Use Azure Active Directory (Entra ID) for authentication (to log users in).
//2-Allow the app to acquire access tokens for calling external APIs (e.g., Microsoft Graph API).
//3-Cache these tokens in memory to avoid requesting new tokens too often.

// Register ITokenAcquisition toget token for calling downstream APIs

//This method sets up authentication using Azure Active Directory (Entra ID) for your web app.
//It reads the Azure AD configuration from your appsettings.json file. For example, it will look for the Client ID, Tenant ID, and other settings there.
//This tells your Blazor Server app to authenticate users via Azure AD and to handle the sign-in process.

//builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration)
//                .EnableTokenAcquisitionToCallDownstreamApi() // For token acquisition : This method enables the app to acquire access tokens for APIs after a user is authenticated.
//                .AddInMemoryTokenCaches(); //Cache these tokens in memory to avoid requesting new tokens too often

// Configure Services 
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddEntraIdAuthenticationService<IEntraIdAuthenticationService, EntraIdAuthenticationService>(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("EntraId:ClientId");
    options.ClientSecret = builder.Configuration.GetValue<string>("EntraId:ClientSecret");
    options.TenantId = builder.Configuration.GetValue<string>("EntraId:TenantId");
});

builder.Services.AddRestClientService<ICustomerApiService, CustomerApiService>(options =>
{
    options.Audience = builder.Configuration.GetValue<string>("CustomerApi:CustomerApiClientId");
    options.BaseAddress = builder.Configuration.GetValue<string>("CustomerApi:BaseAddress");
    options.UserScope = builder.Configuration.GetValue<string>("CustomerApi:UserScope");
    options.AppScope = builder.Configuration.GetValue<string>("CustomerApi:AppScope");

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Add
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
