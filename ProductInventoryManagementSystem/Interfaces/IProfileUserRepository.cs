using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Interfaces
{
    public interface IProfileUserRepository
    {
        //GET METHODS
        Task<ICollection<ProfileUser>> GetProfileUsers();
        Task<ProfileUser> GetProfileUserById(int profileUserId);
        Task<bool> ProfileUserExists(int profileUserId);

        //CREATE METHODS
        Task<bool> CreateProfileUser(ProfileUser userCreate);
        Task<bool> Save();

        //UPDATE METHOD
        Task<bool> UpdateProfileUser(ProfileUser userUpdate);

        //DELETE METHOD
        Task<bool> DeleteProfileUser(ProfileUser userDelete);
    }
}
