using System.Collections.Generic;
using API1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API1.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductsController : ControllerBase
    {
        // api/product/getproducts
        [Authorize]
        [HttpGet]
        public IActionResult GetProducts()
        {
            var productList = new List<Product>
            {
                new Product { Id = 1, Name="Kalem",Price = 100, Stock = 500 },
                new Product { Id = 2, Name="Silgi",Price = 100, Stock = 500 },
                new Product { Id = 3, Name="Defter",Price = 100, Stock = 500 },
                new Product { Id = 4, Name="Kitap",Price = 100, Stock = 500 },
                new Product { Id = 5, Name="Bant",Price = 100, Stock = 500 },
            };

            return Ok(productList);
        }
    }
}