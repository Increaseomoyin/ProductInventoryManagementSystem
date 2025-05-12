using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.DTOS
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
