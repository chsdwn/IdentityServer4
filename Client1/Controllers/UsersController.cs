using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client1.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            // Hangi üyelik tipinden çıkış yaptığını belirtmek için scheme değişkenini giriyoruz.
            // Uygulamada çıkış yapar.
            await HttpContext.SignOutAsync("Cookies");
            // IdentityServer'da çıkış yapar.
            // Resource Owner tipinde gerek yok. Uygulama üzerinden çıkış yapılıyor.
            // await HttpContext.SignOutAsync("oidc");

            return RedirectToAction("Index", "Login");
        }

        // Kullanıcının access token'ı ile işlem yapmak istediği zaman hata alırsa
        // arka tarafta kullanıcının refresh token'ı ile yeni bir access token alıp
        // kullanıcıya belli etmeden bu access token ile işleme devam edilir.
        public async Task<IActionResult> GetRefreshToken()
        {
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var httpClient = new HttpClient();

            var discovery = await httpClient.GetDiscoveryDocumentAsync("https://localhost:4001");
            if (discovery.IsError)
                Console.WriteLine(discovery.Error);

            var refreshTokenReq = new RefreshTokenRequest();
            refreshTokenReq.ClientId = _configuration["ClientResourceOwner:ClientId"];
            refreshTokenReq.ClientSecret = _configuration["ClientResourceOwner:ClientSecret"];
            refreshTokenReq.RefreshToken = refreshToken;
            refreshTokenReq.Address = discovery.TokenEndpoint;

            var tokenRes = await httpClient.RequestRefreshTokenAsync(refreshTokenReq);
            if (tokenRes.IsError)
            {
                Console.WriteLine(tokenRes.Error);
                return RedirectToAction(nameof(Index));
            }

            var tokens = new List<AuthenticationToken>
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
                    Value = DateTime.UtcNow.AddSeconds(tokenRes.ExpiresIn).ToString("o", CultureInfo.InvariantCulture)
                }
            };

            var authResult = await HttpContext.AuthenticateAsync();
            var properties = authResult.Properties;
            properties.StoreTokens(tokens);

            await HttpContext.SignInAsync("Cookies", authResult.Principal, properties);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminAction()
        {
            return View();
        }

        [Authorize(Roles = "admin, customer")]
        public IActionResult CustomerAction()
        {
            return View();
        }
    }
}