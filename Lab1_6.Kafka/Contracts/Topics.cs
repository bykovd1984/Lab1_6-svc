namespace Lab1_6.Kafka.Contracts
{
    public static class Topics
    {
        public static string IdentityServer_UserCreated => "IdentityServer.UserCreated";
        public static string Order_CreateOrder => "Order.CreateOrder";
        public static string Order_OrderRequested => "Order.OrderRequested";
        public static string Order_OrderCreated => "Order.OrderCreated";
        public static string Order_OrderFailed => "Order.OrderFailed";
        public static string Billing_Charged => "Billing.Charged";
        public static string Billing_ChargeFailed => "Billing.ChargeFailed";
    }
}
