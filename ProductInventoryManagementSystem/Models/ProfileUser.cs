namespace ProductInventoryManagementSystem.Models
{
    public class ProfileUser
    {
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
       
        public ICollection<Sale>? Sales { get; set; }
    }
}
