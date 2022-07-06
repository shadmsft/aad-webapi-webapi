using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using models;

namespace api2a.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class ProfileController : ControllerBase
    {
        static readonly string[] scopeRequiredByApi = new string[] { "api2a.Read" };

        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly GraphServiceClient _graphServiceClient;

        public ProfileController(ITokenAcquisition tokenAcquisition, GraphServiceClient graphServiceClient)
        {
            _tokenAcquisition = tokenAcquisition;
            _graphServiceClient = graphServiceClient;
        }

        private List<Profile> profileList = new List<Profile>()
        {
            new Profile
            {
                ProfileId = "1",
                FirstName = "Shad",
                LastName = "Phillips",
                Address = "123 Easy St."
            },
            new Profile
            {
                ProfileId = "2",
                FirstName = "Israel",
                LastName = "Vega",
                Address = "100 Mesa Ave."
            }
        };

        [HttpGet("[action]/{profileId}")]
        [Authorize(Roles = "api2a-ReadonlyRole")]
        public async Task<IActionResult> GetProfileInfo(string profileId)
        {
            var user = await _graphServiceClient.Me.Request().GetAsync();
            string departmentCallingUser = user.Department.ToString();

            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            var profile = profileList.FirstOrDefault(x => x.ProfileId == profileId);
            if (profile == null)
            {
                return NotFound();
            }
            else
            {
                profile.Department = departmentCallingUser;
                return Ok(profile);
            }
        }
    }
}
