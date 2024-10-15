using System.ComponentModel.DataAnnotations;

namespace SalesAPI.Application.DTOs
{
    public class ArticleDTO
    {
        public int ArticleId { get; set; }

        [Required(ErrorMessage = "Article name is required.")]
        [StringLength(100, ErrorMessage = "Article name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Article price is required.")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Article price cannot be negative.")]
        public decimal Price { get; set; }
    }
}
