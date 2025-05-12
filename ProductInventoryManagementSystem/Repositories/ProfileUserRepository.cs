using Microsoft.EntityFrameworkCore;
using ProductInventoryManagementSystem.Data;
using ProductInventoryManagementSystem.Interfaces;
using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Repositories
{
    public class ProfileUserRepository : IProfileUserRepository
    {
        private readonly DataContext _dataContext;

        public ProfileUserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CreateProfileUser(ProfileUser userCreate)
        {
            await _dataContext.ProfileUsers.AddAsync(userCreate);
            return await Save();
        }

        public async Task<bool> DeleteProfileUser(ProfileUser userDelete)
        {
            _dataContext.ProfileUsers.Remove(userDelete);
            return await Save();
        }

        public async Task<ProfileUser> GetProfileUserById(int profileUserId)
        {
            var profileUser = await _dataContext.ProfileUsers.FirstOrDefaultAsync(p=>p.Id == profileUserId);
            return profileUser;
        }

       

        public async Task<ICollection<ProfileUser>> GetProfileUsers()
        {
            var profileUsers = await _dataContext.ProfileUsers.OrderBy(p=>p.Id).ToListAsync();
            return profileUsers;
        }

        public async Task<bool> ProfileUserExists(int profileUserId)
        {
            return await _dataContext.ProfileUsers.AnyAsync(p=>p.Id == profileUserId);
        }

        public async Task<bool> Save()
        {
            var saved = await _dataContext.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateProfileUser(ProfileUser userUpdate)
        {
            var user = await _dataContext.ProfileUsers.Where(u=>u.Id == userUpdate.Id).FirstOrDefaultAsync();
            user.PhoneNumber = userUpdate.PhoneNumber;
            user.FirstName = userUpdate.FirstName;
            user.LastName = userUpdate.LastName;
            
            return await Save();
        }
    }
}
