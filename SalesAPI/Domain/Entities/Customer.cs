namespace SalesAPI.Domain.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }        

        public ICollection<Transaction> Transactions { get; set; }
    }
}
