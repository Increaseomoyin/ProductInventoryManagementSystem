using ProductInventoryManagementSystem.Helper;
using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Interfaces
{
    public interface IProductRepository
    {
        //GET METHODS
        Task<ICollection<Product>> GetProducts(QueryableProduct query);
        Task<Product> GetProductById(int productId);
        Task<Product> GetProductByName(string productName);
        Task<bool> ProductExists(int productId);

        //CREATE METHODS
        Task<bool> CreateProduct(List<int> categoryIds, Product productCreate);
        Task<bool> Save();
        
        //UPDATE METHOD
        Task<bool> UpdateProduct( List<int> categoryIds, Product productUpdate);

        //DELETE METHOD
        Task<bool> DeleteProduct(Product categoryDelete);
    }
}
