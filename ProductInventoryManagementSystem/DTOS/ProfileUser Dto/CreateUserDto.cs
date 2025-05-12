using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.DTOS
{
    public class CreateUserDto
    {
        public string? AppUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
