using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using models;

namespace api2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class OrchestratorController : ControllerBase
    {
        static readonly string[] scopeRequiredByApi = new string[] { "api2.Read" };

        private readonly IDownstreamWebApi _downstreamWebApi2a;
        private readonly IDownstreamWebApi _downstreamWebApi2b;

        public OrchestratorController(IDownstreamWebApi downstreamWebApi2a, IDownstreamWebApi downstreamWebApi2b)
        {
            _downstreamWebApi2a = downstreamWebApi2a;
            _downstreamWebApi2b = downstreamWebApi2b;
        }

        [HttpGet("[action]/{profileId}")]
        [Authorize(Roles = "api2-ReadonlyRole")]
        public async Task<IActionResult> GetProfileProducts(string profileId)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            var profileInfo = await _downstreamWebApi2a.CallWebApiForUserAsync<Profile>("DownstreamApi2a",
                options =>
                {
                    options.RelativePath = $"Profile/GetProfileInfo/{profileId}";
                });
            var products = await _downstreamWebApi2b.CallWebApiForUserAsync<List<Product>>("DownstreamApi2b",
                options =>
                {
                    options.RelativePath = $"Product/GetProductInfo/{profileId}";
                });

            return Ok(new ProfileProductResponse
            {
                //Profile = new Profile() { FirstName = "test", LastName = "test", ProfileId = "3", Address = "184848 S. N. St."},
                //ProfileProducts = products

                Profile = profileInfo,
                ProfileProducts = products
            });
        }

    }
}
