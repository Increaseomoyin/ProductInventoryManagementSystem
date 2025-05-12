using ProductInventoryManagementSystem.DTOS.Category_Dto;
using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Interfaces
{
    public interface ICategoryRepository
    {
        //GET METHODS
        Task<ICollection<Category>> GetCategories();
        Task<Category> GetCategoryById(int categoryId);
        Task<bool> CategoryExists(int categoryId);

        Task<Category> GetCategoryByTitle(string categoryTitle);

        //CREATE METHODS
        Task<bool> CreateCategory(Category categoryCreate);
        Task<bool> Save();
        //UPDATE METHOD
        Task<bool> UpdateCategory(Category categoryUpdate);
        //DELETE METHOD
        Task<bool> DeleteCategory(Category categoryDelete);


    }
}
