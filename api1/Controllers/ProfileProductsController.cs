using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using models;

namespace api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [Authorize]
    public class ProfileProductsController : ControllerBase
    {
        static readonly string[] scopeRequiredByApi = new string[] { "api1.Read" };

        private readonly IDownstreamWebApi _downstreamWebApi;

        public ProfileProductsController(IDownstreamWebApi downstreamWebApi)
        {
            _downstreamWebApi = downstreamWebApi;
        }

        [HttpGet("[action]/{profileId}")]
        [Authorize(Roles = "api1-ReadonlyRole")]
        public async Task<IActionResult> GetProfileProducts(string profileId)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            var profileProducts = await _downstreamWebApi.CallWebApiForUserAsync<ProfileProductResponse>("DownstreamApi",
                options =>
                {
                    options.RelativePath = $"Orchestrator/GetProfileProducts/{profileId}";
                });
            return Ok(profileProducts);

        }

    }
}
