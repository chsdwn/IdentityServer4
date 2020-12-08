using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace AuthServer
{
    public static class Config
    {
        // API'lar
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("resource_api1")
                {
                    Scopes = { "api1.read", "api1.write", "api1.update" },
                    ApiSecrets = new[] { new Secret ("secret_api1".Sha256()) }
                },
                new ApiResource("resource_api2")
                {
                    Scopes = { "api2.read", "api2.write", "api2.update" },
                    ApiSecrets = new[] { new Secret ("secret_api2".Sha256()) }
                }
            };
        }

        // API izinleri
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("api1.read", "API 1 için okuma izni"),
                new ApiScope("api1.write", "API 1 için yazma izni"),
                new ApiScope("api1.update", "API 1 için güncelleme izni"),
                new ApiScope("api2.read", "API 2 için okuma izni"),
                new ApiScope("api2.write", "API 2 için yazma izni"),
                new ApiScope("api2.update", "API 2 için güncelleme izni")
            };
        }

        // API'ları kullanacak Client'lar
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "Client1",
                    ClientName = "Client 1 WebApp",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1.read" }
                },
                new Client
                {
                    ClientId = "Client2",
                    ClientName = "Client 2 WebApp",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1.read", "api1.update", "api2.write", "api2.update" }
                },
                new Client
                {
                    ClientId = "Client1_MVC",
                    ClientName = "Client 1 MVC",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    // response_type: code id_token
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    // Client1'e open id connect eklendiği için /signin-oidc/ endpoint'i otomatik oluştu.
                    RedirectUris = new[] { "https://localhost:7001/signin-oidc" },
                    PostLogoutRedirectUris = new[] { "https://localhost:7001/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        // Refresh token scope
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1.read",
                        "CountryAndCity",
                        "Roles"
                    },
                    RequirePkce = false,
                    AccessTokenLifetime = 2 * 60 * 60,
                    // Refresh token'ı aktifleştirir.
                    // Offline Access etkinken kullanıcı her girişinde Consent(Onay) sayfasından onay vermek zorunda
                    AllowOfflineAccess = true,
                    // Yenilendiğinde aynı refresh token'ı döndürür.
                    RefreshTokenUsage = TokenUsage.ReUse,
                    // 60 içerisinde istek yapılsada yapılmasada refresh token'ın geçerliliği biter.
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.UtcNow.AddDays(60) - DateTime.UtcNow).TotalSeconds
                    // 5 gün içinde istek yapıldığında refresh token'ın ömrünü beş gün daha uzatır.
                    // SlidingRefreshTokenLifetime = DateTime.AddDays(5).Second
                },
                new Client
                {
                    ClientId = "Client2_MVC",
                    ClientName = "Client 2 MVC",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    // response_type: code id_token
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    // Client1'e open id connect eklendiği için /signin-oidc/ endpoint'i otomatik oluştu.
                    RedirectUris = new[] { "https://localhost:8001/signin-oidc" },
                    PostLogoutRedirectUris = new[] { "https://localhost:8001/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        // Refresh token scope
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1.read",
                        "api2.read",
                        "CountryAndCity",
                        "Roles"
                    },
                    RequirePkce = false,
                    AccessTokenLifetime = 2 * 60 * 60,
                    // Refresh token'ı aktifleştirir.
                    // Offline Access etkinken kullanıcı her girişinde Consent(Onay) sayfasından onay vermek zorunda
                    AllowOfflineAccess = true,
                    // Yenilendiğinde aynı refresh token'ı döndürür.
                    RefreshTokenUsage = TokenUsage.ReUse,
                    // 60 içerisinde istek yapılsada yapılmasada refresh token'ın geçerliliği biter.
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.UtcNow.AddDays(60) - DateTime.UtcNow).TotalSeconds
                    // 5 gün içinde istek yapıldığında refresh token'ın ömrünü beş gün daha uzatır.
                    // SlidingRefreshTokenLifetime = DateTime.AddDays(5).Second
                },
                new Client
                {
                    ClientId = "js-client",
                    ClientName = "JS Client Angular",
                    // Mobil ve SPA app'larda secret tutmak güvenlik açığı oluşturur.
                    // ClientSecrets = "secret,
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        // Refresh token scope
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1.read",
                        "CountryAndCity",
                        "Roles"
                    },
                    RedirectUris = { "http://localhost:4200/callback" },
                    PostLogoutRedirectUris = { "http://localhost:4200" },
                    AllowedCorsOrigins = { "http://localhost:4200" },
                },
                new Client
                {
                    ClientId = "Client_ResourceOwner_MVC",
                    ClientName = "Client Resource Owner MVC",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        // Refresh token scope
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1.read",
                        "CountryAndCity",
                        "Roles"
                    },
                    AccessTokenLifetime = 2 * 60 * 60,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.UtcNow.AddDays(60) - DateTime.UtcNow).TotalSeconds
                },
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                /* ZORUNLU */
                // Kullanıcı işlemleri için mutlaka olmalı.
                // Token'a subject id ekler. Hangi kullanıcı için üretildiğini belirtir.
                new IdentityResources.OpenId(), // subId
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("CountryAndCity", "Country and City", new[] {"country", "city"}),
                new IdentityResource("Roles", "Roles", new[] { "role" })
            };
        }

        public static IEnumerable<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "User1",
                    Password = "12345",
                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Ali"),
                        new Claim("family_name", "Veli"),
                        new Claim("country", "Turkey"),
                        new Claim("city", "Konya"),
                        new Claim("role", "admin")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "User2",
                    Password = "12345",
                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Ayşe"),
                        new Claim("family_name", "Fatma"),
                        new Claim("country", "Turkey"),
                        new Claim("city", "İzmir"),
                        new Claim("role", "customer")
                    }
                }
            };
        }
    }
}