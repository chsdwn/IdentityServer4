using System.Collections.Generic;
using API2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API2.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PicturesController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult GetPictures()
        {
            var pictures = new List<Picture>
            {
                new Picture { Id = 1, Name = "Çiçek Resmi", Url = "resim.jpg" },
                new Picture { Id = 2, Name = "Dağ Resmi", Url = "resim.jpg" },
                new Picture { Id = 3, Name = "Araba Resmi", Url = "resim.jpg" },
                new Picture { Id = 4, Name = "Kar Resmi", Url = "resim.jpg" }
            };

            return Ok(pictures);
        }
    }
}