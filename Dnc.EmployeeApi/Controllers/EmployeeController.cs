using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Dnc.EmployeeApi.Controllers
{
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:scopes")]
    [Authorize]
    [Route("api/employees")]
    public class EmployeeController : ControllerBase
    {
        [HttpGet("all")]
        public IEnumerable<Employee> GetEmployees()
        {
            return
           [
               new() { Id = 1, FirstName = "John", LastName = "Smith", Email = "john.smith@dotnetcoder.com"},
               new() { Id = 2, FirstName = "Sam", LastName = "Mell", Email = "sam.mell@dotnetcoder.com"},
               new() { Id = 3, FirstName = "Sara", LastName = "Smith", Email = "sara.smith@dotnetcoder.com"},
           ];
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
