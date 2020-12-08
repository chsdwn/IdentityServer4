using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Client1.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client1.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

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
                Console.WriteLine(tokenRes.Error);

            var userInfoReq = new UserInfoRequest();
            userInfoReq.Token = tokenRes.AccessToken;
            userInfoReq.Address = discovery.UserInfoEndpoint;

            var userInfoRes = await client.GetUserInfoAsync(userInfoReq);
            if (userInfoRes.IsError)
                Console.WriteLine(userInfoRes.Error);

            var identity = new ClaimsIdentity(
                userInfoRes.Claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var authenticationProperties = new AuthenticationProperties();
            var authenticationTokens = new List<AuthenticationToken>
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = tokenRes.IdentityToken
                },
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

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("User", "Index");
        }
    }
}