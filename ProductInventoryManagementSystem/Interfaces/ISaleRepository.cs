using ProductInventoryManagementSystem.Helper;
using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Interfaces
{
    public interface ISaleRepository
    {
        //GET METHOD
        Task<ICollection<Sale>> GetSales(QueryableSale query);
        //CREATE METHOD
        Task<bool> CreateSale(Sale saleCreate);
        Task<bool> Save();
        //UPDATE METHOD
        Task<bool> UpdateSale(Sale saleUpdate);
        //DELETE METHOD
        Task<bool> DeleteSale(Sale saleDelete);
    }
}
