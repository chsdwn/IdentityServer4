using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AuthServer.Data;
using AuthServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var sqliteConStr = "Data Source=AuthServer.db";

            services.AddScoped<ICustomUserRepository, CustomUserRepository>();

            services.AddDbContext<AppDbContext>(builder =>
            {
                builder.UseSqlite(sqliteConStr);
            });

            var assemblyName = typeof(Startup).Assembly.GetName().Name;

            services.AddIdentityServer()
                /* ConfigurationDbContext için ayrı PersistedGrantDbContext için ayrı migration oluşturulmalı.
                    ConfigurationDbContext: dotnet ef migrations add Initial -c ConfigurationDbContext
                    PersistedGrantDbContext: dotnet ef migrations add Initial -c PersistedGrantDbContext */
                // ConfigurationDbContext: client, resource ve scope'ları veritabanına kaydeder.
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder
                        => builder.UseSqlite(sqliteConStr, sqliteOptions
                            => sqliteOptions.MigrationsAssembly(assemblyName));
                })
                // PersistedGrantDbContext: refresh token, authorize code'u veritabanına kaydeder.
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder
                        => builder.UseSqlite(sqliteConStr, sqliteOptions
                            => sqliteOptions.MigrationsAssembly(assemblyName));
                })
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                // .AddTestUsers(Config.GetTestUsers().ToList())
                // Geliştirme esnasında public ve private keyi otomatik oluşturur.
                .AddDeveloperSigningCredential()
                .AddProfileService<CustomProfileService>()
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
