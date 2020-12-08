using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    // IdentityServerAccessToken scope'u olmayan client erişemez.
    [Authorize(LocalApi.PolicyName)]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public IActionResult SignUp()
        {
            return Ok(nameof(SignUp));
        }
    }
}