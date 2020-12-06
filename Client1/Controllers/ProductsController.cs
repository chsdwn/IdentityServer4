using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Client1.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Client1.Controllers
{
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

            // Discovery endpoint: /.well-known/openid-configuration
            var discovery = await httpClient.GetDiscoveryDocumentAsync("https://localhost:4001");
            if (discovery.IsError)
                Console.WriteLine(discovery.Error);

            var tokenReq = new ClientCredentialsTokenRequest();
            tokenReq.ClientId = _configuration["Client:ClientId"];
            tokenReq.ClientSecret = _configuration["Client:ClientSecret"];
            tokenReq.Address = discovery.TokenEndpoint; // /connect/token

            var token = await httpClient.RequestClientCredentialsTokenAsync(tokenReq);
            if (token.IsError)
                Console.WriteLine(token.Error);

            httpClient.SetBearerToken(token.AccessToken);

            var response = await httpClient.GetAsync("https://localhost:5001/api/products/getproducts");
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