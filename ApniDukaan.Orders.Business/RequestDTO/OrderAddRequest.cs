namespace ApniDukaan.Orders.Business.RequestDTO
{
    using System;
    using System.Collections.Generic;

    public class OrderAddRequest
    {
        public Guid UserID { get; set; }

        public DateTime OrderDate { get; set; }

        public List<OrderItemAddRequest> OrderItems { get; set; } = new List<OrderItemAddRequest>();
    }
}
