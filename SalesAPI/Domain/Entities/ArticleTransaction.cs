namespace SalesAPI.Domain.Entities
{
    public class ArticleTransaction
    {
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}