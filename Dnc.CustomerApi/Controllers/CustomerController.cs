using Dnc.CustomerApi.Services;
using Dnc.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Dnc.CustomerApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly IOrderApiService orderApiService;

        public CustomerController(IOrderApiService orderApiService)
        {
            this.orderApiService = orderApiService;
        }


        [HttpGet("all")]
        public IEnumerable<Customer> GetCustomers()
        {
            return
            [    new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                 new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" },
                 new Customer { Id = 3, FirstName = "Emily", LastName = "Joe", Email = "emily.Joe@example.com" }
            ];
        }

        [HttpGet("orders/all")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:scopes")]
        public async Task<IEnumerable<Customer>?> GetCustomersWithOrders()
        {
            var orders = await orderApiService.GetOrders();
            if (orders.Any())
            {
                return
                [    new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Orders= orders.Where(v=>v.CustomerId == 1) },
                     new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Orders= orders.Where(v=>v.CustomerId == 2)},
                     new Customer { Id = 3, FirstName = "Emily", LastName = "Joe", Email = "emily.Joe@example.com", Orders= orders.Where(v=>v.CustomerId == 3) }
                ];
            }
            return null;
        }
    }
}
