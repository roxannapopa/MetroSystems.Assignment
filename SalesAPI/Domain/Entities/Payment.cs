namespace SalesAPI.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}
