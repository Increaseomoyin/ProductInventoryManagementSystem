using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.DTOS.Product_Dto
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
