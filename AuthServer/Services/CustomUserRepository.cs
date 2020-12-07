using System.Linq;
using System.Threading.Tasks;
using AuthServer.Data;
using AuthServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Services
{
    public class CustomUserRepository : ICustomUserRepository
    {
        private readonly AppDbContext _dbContext;

        public CustomUserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CustomUser> FindByEmailAsync(string email)
        {
            return await _dbContext.CustomUsers.FirstOrDefaultAsync(c => c.Email.Equals(email));
        }

        public async Task<CustomUser> FindByIdAsync(int id)
        {
            return await _dbContext.CustomUsers.FindAsync(id);
        }

        public async Task<bool> ValidateAsync(string email, string password)
        {
            return await _dbContext.CustomUsers
                .AnyAsync(c => c.Email.Equals(email) && c.Password.Equals(password));
        }
    }
}