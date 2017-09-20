using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckoutAssignment.Models
{
    public class Basket
    {
        public long Id { get; set; }
        public string Owner { get; set; }
        public List<ItemOrder> Orders { get; set; } = new List<ItemOrder>();
        public DateTime LastModified { get; set; }

        public float GetPrice()
        {
            return Orders.Sum(o => o.Item.Price * o.Amount);
        }
    }
}