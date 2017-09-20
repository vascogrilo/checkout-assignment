using System.Runtime.Serialization;

namespace CheckoutAssignment.Models
{
    [DataContract]
    public class ItemOrder
    {
        [DataMember]
        public Item Item { get; set; }
        [DataMember]
        public uint Amount { get; set; }
    }
}