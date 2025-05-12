using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.DTOS.Sale_Dto
{
    public class CreateSaleDto
    {
        public int ProductId { get; set; }
        public int? ProfileUserId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateSold { get; set; }
    }
}
