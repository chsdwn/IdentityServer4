using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Client1.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace Client1.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            var client = new HttpClient();

            var discovery = await client.GetDiscoveryDocumentAsync(_configuration["AuthServerUrl"]);
            if (discovery.IsError)
                Console.WriteLine(discovery.Error);

            var passwordReq = new PasswordTokenRequest();
            passwordReq.Address = discovery.TokenEndpoint;
            passwordReq.UserName = model.Email;
            passwordReq.Password = model.Password;
            passwordReq.ClientId = _configuration["ClientResourceOwner:ClientId"];
            passwordReq.ClientSecret = _configuration["ClientResourceOwner:ClientSecret"];

            var tokenRes = await client.RequestPasswordTokenAsync(passwordReq);
            if (tokenRes.IsError)
            {
                ModelState.AddModelError("", "Email veya şifre yanlış.");
                Console.WriteLine(tokenRes.Error);
                return View();
            }

            var userInfoReq = new UserInfoRequest();
            userInfoReq.Token = tokenRes.AccessToken;
            userInfoReq.Address = discovery.UserInfoEndpoint;

            var userInfoRes = await client.GetUserInfoAsync(userInfoReq);
            if (userInfoRes.IsError)
                Console.WriteLine(userInfoRes.Error);

            var identity = new ClaimsIdentity(
                userInfoRes.Claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                "name",     // NameClaimType
                "role");    // RoleClaimType

            var principal = new ClaimsPrincipal(identity);

            var authenticationProperties = new AuthenticationProperties();
            var authenticationTokens = new List<AuthenticationToken>
            {
                // Hybrid akış olmadığı için id_token dönmez
                // new AuthenticationToken
                // {
                //     Name = OpenIdConnectParameterNames.IdToken,
                //     Value = tokenRes.IdentityToken
                // },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenRes.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenRes.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.ExpiresIn,
                    // Giriş yapılan ülke'den bağımsız evrensel tarih formatı kullanır.
                    Value = DateTime.UtcNow.AddSeconds(tokenRes.ExpiresIn)
                        .ToString("o", CultureInfo.InvariantCulture)
                }
            };
            authenticationProperties.StoreTokens(authenticationTokens);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authenticationProperties);

            return RedirectToAction("Index", "Users");
        }

        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            var client = new HttpClient();

            var discovery = await client.GetDiscoveryDocumentAsync(_configuration["AuthServerUrl"]);
            if (discovery.IsError)
                Console.WriteLine(discovery.Error);

            var tokenReq = new ClientCredentialsTokenRequest();
            tokenReq.ClientId = _configuration["ClientResourceOwner:ClientId"];
            tokenReq.ClientSecret = _configuration["ClientResourceOwner:ClientSecret"];
            tokenReq.Address = discovery.TokenEndpoint;

            var tokenRes = await client.RequestClientCredentialsTokenAsync(tokenReq);
            if (tokenRes.IsError)
                Console.WriteLine(tokenRes.Error);

            client.SetBearerToken(tokenRes.AccessToken);

            // SignUpViewModel'i json'a cevirir.
            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:4001/api/user/signup", stringContent);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Users");

            var errorJson = await response.Content.ReadAsStringAsync();
            var errorList = JsonConvert.DeserializeObject<List<string>>(errorJson);
            return BadRequest(errorList);
        }
    }
}