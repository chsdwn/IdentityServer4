using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Client1.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace Client1.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var httpClient = new HttpClient();

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            httpClient.SetBearerToken(accessToken);

            var response = await httpClient.GetAsync("https://localhost:5001/api/products/getproducts");
            // httpClient.PostAsync();
            // httpClient.PutAsync();
            // httpClient.DeleteAsync();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(content);
                return View(products);
            }
            else
            {
                Console.WriteLine(response.RequestMessage);
                return View();
            }
        }
    }
}