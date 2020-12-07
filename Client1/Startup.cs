using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Client1
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies", options =>
            {
                options.AccessDeniedPath = "/Home/AccessDenied";
            }).AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                // Yetkili. Token gönderen.
                options.Authority = "https://localhost:4001";
                options.ClientId = "Client1_MVC";
                options.ClientSecret = "secret";
                // code: Authorization code. id_token: doğrulama kodu. token: access token
                options.ResponseType = "code id_token";
                // Access token'da claim bilgileri bulunmaz. Scope'lar bulunur.
                // Giriş yapıldıktan sonra user endpoint'ten claim'ler alınır.
                // Bütün claimleri çekip, giriş yapmış olan kullanıcının bilgilerine ekler.
                options.GetClaimsFromUserInfoEndpoint = true;
                // Access ve refresh token değerlerini cookie'ye ekler.
                options.SaveTokens = true;

                options.Scope.Add("api1.read");
                // Refresh token scope
                options.Scope.Add("offline_access");
                options.Scope.Add("CountryAndCity");
                options.Scope.Add("Roles");

                // Custom claim'ler manuel olarak map'lenmeli. Yoksa user claim'leri içinde görünmez.
                // CountryAndCity: { "country", "city" }
                options.ClaimActions.MapUniqueJsonKey("country", "country");
                options.ClaimActions.MapUniqueJsonKey("city", "city");
                options.ClaimActions.MapUniqueJsonKey("role", "role");

                // [Authorize(Roles = "")] attribute'u kullanıldığı zaman role'ü kontrol etmek için
                // role claim'ine bakacak.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = "role"
                };
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();    // kimlik doğrulama
            app.UseAuthorization();     // yetki kontrolü

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
