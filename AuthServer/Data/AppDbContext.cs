using AuthServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Data
{
    public class AppDbContext : DbContext
    {
        // Birden fazla db context kullanılıyorsa DbContextOptions'da generic olarak
        // hangisi initialize edilmek isteniyorsa belirtilmeli.
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomUser>().HasData(
                new CustomUser
                {
                    Id = 1,
                    Username = "User1",
                    Email = "User1@mail.com",
                    Password = "12345",
                    City = "Konya"
                },
                new CustomUser
                {
                    Id = 2,
                    Username = "User2",
                    Email = "User2@mail.com",
                    Password = "12345",
                    City = "İzmir"
                },
                new CustomUser
                {
                    Id = 3,
                    Username = "User3",
                    Email = "User3@mail.com",
                    Password = "12345",
                    City = "İstanbul"
                }
            );
        }

        public DbSet<CustomUser> CustomUsers { get; set; }
    }
}