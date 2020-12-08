using System.Linq;
using System.Threading.Tasks;
using IdentityAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignUpAsync(SignUpViewModel model)
        {
            var user = new ApplicationUser();
            user.UserName = model.Username;
            user.Email = model.Email;
            user.City = model.City;

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors.Select(e => e.Description));
        }
    }
}