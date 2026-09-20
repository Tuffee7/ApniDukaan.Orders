namespace ApniDukaan.Orders.Business.ResponseDTO
{
    using System;

    public class OrderItemResponse
    {
        public Guid ProductID { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
