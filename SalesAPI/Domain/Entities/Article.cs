namespace SalesAPI.Domain.Entities
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public ICollection<ArticleTransaction> TransactionArticles { get; set; }
    }
}
