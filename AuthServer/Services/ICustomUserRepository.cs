using System.Threading.Tasks;
using AuthServer.Models;

namespace AuthServer.Services
{
    public interface ICustomUserRepository
    {
        Task<bool> ValidateAsync(string email, string password);
        Task<CustomUser> FindByIdAsync(int id);
        Task<CustomUser> FindByEmailAsync(string email);
    }
}