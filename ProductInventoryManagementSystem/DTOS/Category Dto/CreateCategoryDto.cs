using System.ComponentModel.DataAnnotations;

namespace ProductInventoryManagementSystem.DTOS.Category_Dto
{
    public class CreateCategoryDto
    {
        [Required]
        public string? Title { get; set; }

    }
}
