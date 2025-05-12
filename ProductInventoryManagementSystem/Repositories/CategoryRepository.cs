using Microsoft.EntityFrameworkCore;
using ProductInventoryManagementSystem.Data;
using ProductInventoryManagementSystem.DTOS.Category_Dto;
using ProductInventoryManagementSystem.Interfaces;
using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _dataContext;

        public CategoryRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CategoryExists(int categoryId)
        {
            return await _dataContext.Categories.AnyAsync(c=>c.Id ==  categoryId);
        }

        public async Task<bool> CreateCategory(Category categoryCreate)
        {
            await _dataContext.AddAsync(categoryCreate);
            return await Save();



        }

        public async Task<bool> DeleteCategory(Category categoryDelete)
        {
            _dataContext.Remove(categoryDelete);
            return await Save();
        }

        public async Task<ICollection<Category>> GetCategories()
        {
            var categories = await _dataContext.Categories.OrderBy(c => c.Id).ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            var category = await _dataContext.Categories.Where(c => c.Id == categoryId).FirstOrDefaultAsync();
            return category;
        }

        public async Task<Category> GetCategoryByTitle(string categoryTitle)
        {
            var category = await _dataContext.Categories.FirstOrDefaultAsync(c=>c.Title == categoryTitle);
            return category;
        }

        public async Task<bool> Save()
        { 
            var saved = await _dataContext.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateCategory(Category categoryUpdate)
        {
            _dataContext.Categories.Update(categoryUpdate);
            return await Save();
        }
    }
}
