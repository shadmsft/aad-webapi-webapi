using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using models;

namespace api2b.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class ProductController : Controller
    {
        static readonly string[] scopeRequiredByApi = new string[] { "api2b.Read" };

        private List<Product> productList = new List<Product>()
        {
            new Product
            {
                ProductId = "1",
                ProductName = "Tires",
                ProductDescription = "Helps to roll"
            },
            new Product
            {
                ProductId = "2",
                ProductName = "Soap",
                ProductDescription = "Helps to clean"
            }
        };

        [HttpGet("[action]/{productId}")]
        [Authorize(Roles = "api2b-ReadonlyRole")]
        public async Task<List<Product>> GetProductInfo()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            return productList;
        }

    }
}
