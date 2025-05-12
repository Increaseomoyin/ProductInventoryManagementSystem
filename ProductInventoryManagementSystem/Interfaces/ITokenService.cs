using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser appUser);
    }
}
