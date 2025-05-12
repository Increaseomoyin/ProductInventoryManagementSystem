namespace ProductInventoryManagementSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }
        public ICollection<Sale>? Sales { get; set; }
    }
}
