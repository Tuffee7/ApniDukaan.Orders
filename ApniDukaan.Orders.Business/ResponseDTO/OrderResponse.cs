namespace ApniDukaan.Orders.Business.ResponseDTO
{
    using System;
    using System.Collections.Generic;

    public class OrderResponse
    {
        public Guid OrderID { get; set; }

        public Guid UserID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalBill { get; set; }

        public List<OrderItemResponse> OrderItems { get; set; } = new List<OrderItemResponse>();
    }
}
