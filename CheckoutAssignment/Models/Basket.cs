using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CheckoutAssignment.Models
{
    public class Basket
    {
        public long Id { get; set; }
        public string Owner { get; set; }
        public List<ItemOrder> Orders { get; set; } = new List<ItemOrder>();
    }
}