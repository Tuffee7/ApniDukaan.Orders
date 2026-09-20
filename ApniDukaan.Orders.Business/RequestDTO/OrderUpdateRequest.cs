namespace ApniDukaan.Orders.Business.RequestDTO
{
    using System;
    using System.Collections.Generic;

    public class OrderUpdateRequest
    {
        public Guid OrderID { get; set; }

        public Guid UserID { get; set; }

        public DateTime OrderDate { get; set; }

        public List<OrderItemUpdateRequest> OrderItems { get; set; } = new List<OrderItemUpdateRequest>();
    }
}
