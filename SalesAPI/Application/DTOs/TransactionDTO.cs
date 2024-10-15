using System.ComponentModel.DataAnnotations;

namespace SalesAPI.Application.DTOs
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "At least one article ID is required.")]
        public List<int> ArticleIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "At least one payment is required.")]
        public List<PaymentDTO> Payments { get; set; } = new List<PaymentDTO>();
    }
}
