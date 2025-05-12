namespace ProductInventoryManagementSystem.Helper
{
    public class QueryableProduct
    {
        public decimal? Price { get; set; }
        public int PageSize { get; set; } = 6;
        //public int Category { get; set; }
        public int PageNumber { get; set; } = 1;
    }
}
