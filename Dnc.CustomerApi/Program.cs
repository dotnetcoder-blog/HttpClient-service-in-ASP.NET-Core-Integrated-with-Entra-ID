using Dnc.CustomerApi.Services;
using Dnc.Services.RestClient.EntraId;
using Dnc.Services.RestClient.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
      .EnableTokenAcquisitionToCallDownstreamApi()
      .AddInMemoryTokenCaches();


//// This code works but use the default one above.
////The code below is designed to separate concerns between bearer token validation (what tokens are acceptable) and identity configuration (how to communicate with Azure AD).
////This makes it easy to adjust one aspect without affecting the other.

////bearerOptions: Used to configure JWT Bearer token options.
////identityOptions: Used to configure the Microsoft identity platform settings for Azure AD.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddMicrosoftIdentityWebApi(bearerOptions => 
//      {
//          bearerOptions.TokenValidationParameters.ValidateAudience = true;
//          bearerOptions.Audience = builder.Configuration.GetValue<string>("AzureAd:Audience");

//          bearerOptions.TokenValidationParameters = new TokenValidationParameters
//          {
//              ValidateIssuer = true,
//              ValidIssuer = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0",
//          };
//      },identityOptions => 
//      { 
//          //identityOptions.Instance = builder.Configuration.GetValue<string>("AzureAd:Instance");
//          identityOptions.Instance = "https://login.microsoftonline.com/";
//          identityOptions.Domain = builder.Configuration.GetValue<string>("AzureAd:Domain");
//          identityOptions.TenantId = builder.Configuration.GetValue<string>("AzureAd:TenantId");
//          identityOptions.ClientId = builder.Configuration.GetValue<string>("AzureAd:ClientId");
//          identityOptions.ClientSecret = builder.Configuration.GetValue<string>("AzureAd:ClientSecret");
//      })//This method enables the API to acquire tokens for calling downstream APIs,
//        //like Microsoft Graph or other APIs protected by Azure AD.
//        //This is useful if your API needs to act on behalf of a user or application to call another API
//      .EnableTokenAcquisitionToCallDownstreamApi(options=> { })
//      //This configures an in-memory cache for storing tokens.
//      //It helps avoid repeated token acquisitions by caching tokens in memory during the app's lifetime.
//      .AddInMemoryTokenCaches();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure RestClient service
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddEntraIdAuthenticationService<IEntraIdAuthenticationService, EntraIdAuthenticationService>(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("AzureAd:ClientId");
    options.ClientSecret = builder.Configuration.GetValue<string>("AzureAd:ClientSecret");
    options.TenantId = builder.Configuration.GetValue<string>("AzureAd:TenantId");
});

builder.Services.AddRestClientService<IOrderApiService, OrderApiService>(options =>
{
    options.Audience = builder.Configuration.GetValue<string>("OrderApi:OrderApiClientId");
    options.UserScope = builder.Configuration.GetValue<string>("OrderApi:UserScope");
    options.BaseAddress = builder.Configuration.GetValue<string>("OrderApi:BaseAddress");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
