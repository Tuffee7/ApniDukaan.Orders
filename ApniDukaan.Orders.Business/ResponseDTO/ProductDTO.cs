using System;
using System.Collections.Generic;
using System.Text;

namespace ApniDukaan.Orders.Business.ResponseDTO
{
    public class ProductDTO
    {
        public Guid ProductID { get; set; }

        public string? ProductName { get; set; }

        public string? Category { get; set; }

        public double Price { get; set; }

        public int QuantityInStock { get; set; }
    }
}
