
namespace Lab1_6.Models.Orders
{
    public class OrderRequest
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public string RequestId { get; set; }
    }
}
