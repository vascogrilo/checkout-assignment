using System;

namespace CheckoutAssignment.Models
{
    public class Item : IEquatable<Item>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }

        public bool Equals(Item other)
        {
            return Id == other.Id && Name == other.Name && Price.Equals(other.Price);
        }
    }
}