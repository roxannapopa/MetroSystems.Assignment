namespace SalesAPI.Domain.Entities
{
    public class Transaction
    {
        public Transaction()
        {
            TransactionArticles = new List<ArticleTransaction>();
            Payments = new List<Payment>();
        }

        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<ArticleTransaction> TransactionArticles { get; set; } = new List<ArticleTransaction>();
        
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

}
