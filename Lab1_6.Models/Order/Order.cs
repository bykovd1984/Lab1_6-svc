namespace Lab1_6.Models.Order
{
    public class Order
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public int Sum { get; set; }

        public OrderStatus Status { get; set; }
    }
}
