using Microsoft.EntityFrameworkCore;
using ProductInventoryManagementSystem.Data;
using ProductInventoryManagementSystem.Helper;
using ProductInventoryManagementSystem.Interfaces;
using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DataContext _dataContext;

        public SaleRepository(DataContext dataContext)
        {
           _dataContext = dataContext;
        }

        public async Task<bool> CreateSale(Sale saleCreate)
        {   
            var correctProduct = await _dataContext.Products.AnyAsync(p=>p.Id == saleCreate.ProductId);
            var correctUser = await _dataContext.ProfileUsers.AnyAsync(u => u.Id == saleCreate.ProfileUserId);
            if (!correctProduct || !correctUser)
            {
                throw new InvalidOperationException("Either the user or the product does not exist.");
            }
            await _dataContext.AddAsync(saleCreate);
            return await Save();


        }

        public async Task<bool> DeleteSale(Sale saleDelete)
        {
            _dataContext.Sales.Remove(saleDelete);
            return await Save();
        }

        public async Task<ICollection<Sale>> GetSales(QueryableSale query)
        {
            var sales = _dataContext.Sales.OrderBy(s => s.Id).AsQueryable();
            
            if (query.ProfileUserId != null)
            {
                sales = sales.Where(s => s.ProfileUserId.Equals(query.ProfileUserId));
            }
            if (query.Quantity != null)
            {
                sales = sales.Where(s=>s.Quantity >= query.Quantity);
            }
            
            var skipNumber = (query.PageNumber - 1) * query.PageSize;
            return await sales.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await _dataContext.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> UpdateSale(Sale saleUpdate)
        {
            var product = await _dataContext.Products.AnyAsync(p => p.Id == saleUpdate.ProductId);
            var user = await _dataContext.ProfileUsers.AnyAsync(p => p.Id == saleUpdate.ProfileUserId);
            if (!product || !user)
                throw new Exception();

            _dataContext.Sales.Update(saleUpdate);
            return await Save();
        }
    }
}
