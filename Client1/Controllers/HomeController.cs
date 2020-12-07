using Microsoft.AspNetCore.Mvc;

namespace Client1.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl">Erişilemeyen sayfanın url'i</param>
        /// <returns></returns>
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.url = returnUrl;

            return View();
        }
    }
}