using Microsoft.EntityFrameworkCore;
using ProductInventoryManagementSystem.Data;
using ProductInventoryManagementSystem.DTOS.Product_Dto;
using ProductInventoryManagementSystem.Helper;
using ProductInventoryManagementSystem.Interfaces;
using ProductInventoryManagementSystem.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProductInventoryManagementSystem.Repositories
{
    public class ProductRepository :IProductRepository
    {
        private readonly DataContext _dataContext;

        public ProductRepository(DataContext dataContext )
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CreateProduct(List<int> categoryIds, Product productCreate)
        {
            await _dataContext.Products.AddAsync(productCreate);
            await _dataContext.SaveChangesAsync();

            foreach(var Id in categoryIds)
            {
                var category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == Id);
                if (category != null)
                {
                    var productCategories = new ProductCategory()
                    { 
                        CategoryId = Id,
                        ProductId = productCreate.Id
                    };
                    await _dataContext.ProductCategories.AddAsync(productCategories);
                }
            }
            return await Save();
        }

        public async Task<bool> DeleteProduct(Product categoryDelete)
        {
            _dataContext.Products.Remove(categoryDelete);
            return await Save();
        }

        public async Task<Product> GetProductById(int productId)
        {
            var product = await _dataContext.Products.Where(p=>p.Id == productId).FirstOrDefaultAsync();
            return product;
        }

        public async Task<Product> GetProductByName(string productName)
        {
            var product = await _dataContext.Products.Where(p=>p.Name == productName).FirstOrDefaultAsync();
            return product;
        }

        public async Task<ICollection<Product>> GetProducts(QueryableProduct query)
        {
            var products =  _dataContext.Products.OrderBy(p => p.Id).AsQueryable();
            if (query.Price !=null )
            {
                products = products.Where(p=>p.Price >= query.Price);
            }
            var skipNumber = (query.PageNumber - 1) * query.PageSize;
            return await products.Skip(skipNumber).Take(query.PageSize).ToListAsync();

        }

        public async Task<bool> ProductExists(int productId)
        {
            return await _dataContext.Products.AnyAsync(p=>p.Id == productId);
        }

        public async Task<bool> Save()
        {
            var saved = await _dataContext.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateProduct(List<int> categoryIds, Product productUpdate)
        {
            _dataContext.Products.Update(productUpdate);
            // Only update product categories if new category IDs were provided
            if (categoryIds != null && categoryIds.Any())
            {
                // Remove existing product-category links
                var existingProductCategories = await _dataContext.ProductCategories
                    .Where(pc => pc.ProductId == productUpdate.Id)
                    .ToListAsync();

                _dataContext.ProductCategories.RemoveRange(existingProductCategories);

            }
            foreach (var Id in categoryIds)
            {
                var category = await _dataContext.Categories.Where(c => c.Id == Id).FirstOrDefaultAsync();
                if (category != null)
                {
                    var productCategory = new ProductCategory()
                    {
                        ProductId = productUpdate.Id,
                        CategoryId = Id
                    };
                    await _dataContext.ProductCategories.AddAsync(productCategory);
                }
            }
            return await Save();
        }
    }
}
