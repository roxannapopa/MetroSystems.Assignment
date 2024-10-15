using System.ComponentModel.DataAnnotations;

namespace SalesAPI.Application.DTOs
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(256, ErrorMessage = "Customer name cannot exceed 256 characters.")]
        public string Name { get; set; }
    }
}
