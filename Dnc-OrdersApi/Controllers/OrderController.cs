using Dnc.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Dnc.OrdersApi.Controllers
{
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:scopes")]
    [Authorize]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        [HttpGet("all")]
        public IEnumerable<Order> GetOrders()
        {
            return
            [
                new Order { OrderId = 102, CustomerId = 1, TotalAmount = 75.00m },
                new Order { OrderId = 101, CustomerId = 1, TotalAmount = 250.00m },
                new Order { OrderId = 201, CustomerId = 2, TotalAmount = 200.00m },
                new Order { OrderId = 102, CustomerId = 2, TotalAmount = 150.50m },
                new Order { OrderId = 103, CustomerId = 3, TotalAmount = 325.75m },
                new Order { OrderId = 302, CustomerId = 3, TotalAmount = 200.00m }
            ];
        }
    }
}
