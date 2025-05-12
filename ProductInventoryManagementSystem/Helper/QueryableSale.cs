namespace ProductInventoryManagementSystem.Helper
{
    public class QueryableSale
    {
        public int? ProfileUserId { get; set; } 

        public int? Quantity { get; set; }
        public int PageSize { get; set; } = 6;
        //public int Category { get; set; }
        public int PageNumber { get; set; } = 1;
    }
}
