using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client1.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task Logout()
        {
            // Hangi üyelik tipinden çıkış yaptığını belirtmek için scheme değişkenini giriyoruz.
            // Uygulamada çıkış yapar.
            await HttpContext.SignOutAsync("Cookies");
            // IdentityServer'da çıkış yapar.
            await HttpContext.SignOutAsync("oidc");
        }
    }
}