using System;

namespace CheckoutAssignment.Models
{
    public class ItemOrder
    {
        public Item Item { get; set; }
        public uint Amount { get; set; }
        public DateTime LastModified { get; set; }
    }
}