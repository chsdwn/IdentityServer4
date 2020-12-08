using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace AuthServer.Seed
{
    public static class IdentityServerSeedData
    {
        public static void Seed(ConfigurationDbContext dbContext)
        {
            if (!dbContext.Clients.Any())
                foreach (var client in Config.GetClients())
                    dbContext.Clients.Add(client.ToEntity());

            if (!dbContext.ApiResources.Any())
                foreach (var apiResource in Config.GetApiResources())
                    dbContext.ApiResources.Add(apiResource.ToEntity());

            if (!dbContext.ApiScopes.Any())
                foreach (var apiScope in Config.GetApiScopes())
                    dbContext.ApiScopes.Add(apiScope.ToEntity());

            if (!dbContext.IdentityResources.Any())
                foreach (var identityResource in Config.GetIdentityResources())
                    dbContext.IdentityResources.Add(identityResource.ToEntity());

            // Sadece uygulama başlarken ve veri yoksa çalışacağı için async olmasına gerek yok.
            dbContext.SaveChanges();
        }
    }
}