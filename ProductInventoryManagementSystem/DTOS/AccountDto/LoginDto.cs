using System.ComponentModel.DataAnnotations;

namespace ProductInventoryManagementSystem.DTOS.AccountDto
{
    public class LoginDto
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
