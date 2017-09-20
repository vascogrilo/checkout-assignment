using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CheckoutAssignment.Models
{
    [DataContract]
    public class Basket
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Owner { get; set; }
        [DataMember]
        public List<ItemOrder> Orders { get; set; } = new List<ItemOrder>();

        public float GetPrice()
        {
            return Orders.Sum(o => o.Item.Price * o.Amount);
        }
    }
}