namespace SalesAPI.Domain.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public int Quantity { get; set; }
    }
}
