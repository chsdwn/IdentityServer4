using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client2.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult User()
        {
            return View();
        }
    }
}