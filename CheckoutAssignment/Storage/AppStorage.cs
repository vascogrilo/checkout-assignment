using System.Collections.Generic;
using CheckoutAssignment.Models;

namespace CheckoutAssignment.Storage
{
    public class AppStorage : IApplicationStorage
    {
        private Dictionary<long, Item> _items;
        private Dictionary<long, Basket> _baskets;
        private const int _idSeed = 1;

        public AppStorage()
        {
            _items = new Dictionary<long, Item>();
            _baskets = new Dictionary<long, Basket>();
            CreateLineItem(new Item { Name = "Gibson SG left-handed", Price = 250.99f });
            CreateLineItem(new Item { Name = "Peavey ValveKing Tube amp", Price = 220f });
            CreateBasket(new Basket { Owner = "vasco" });
            CreateBasket(new Basket { Owner = "james", Orders = { new ItemOrder { Item = GetLineItem(1), Amount = 1 } } });
        }

        public bool ContainsLineItem(long id)
        {
            return _items.ContainsKey(id);
        }

        public IEnumerable<Item> GetLineItems()
        {
            return _items.Values;
        }

        public Item GetLineItem(long id)
        {
            if (!_items.TryGetValue(id, out Item item))
                return null;
            return item;
        }

        public Item CreateLineItem(Item item)
        {
            if (item == null)
                return null;
            lock (this)
            {
                var newId = _idSeed + _items.Count;
                item.Id = newId;
                _items.Add(newId, item);
            }
            return item;
        }

        public bool UpdateLineItem(long id, Item item)
        {
            if (item?.Id != id)
                return false;
            if (!_items.ContainsKey(id))
                return false;
            lock (this)
            {
                _items[id] = item;
            }
            return true;
        }

        public bool DeleteLineItem(long id)
        {
            return _items.Remove(id);
        }

        public Basket GetBasket(long id)
        {
            if (!_baskets.TryGetValue(id, out Basket basket))
                return null;
            return basket;
        }

        public bool ContainsBasket(long id)
        {
            return _baskets.ContainsKey(id);
        }

        public IEnumerable<Basket> GetBaskets()
        {
            return _baskets.Values;
        }

        public Basket CreateBasket(Basket basket)
        {
            if (basket == null)
                return null;
            lock (this)
            {
                var newId = _idSeed + _baskets.Count;
                basket.Id = newId;
                if (basket.Orders.TrueForAll(o => ContainsLineItem(o.Item.Id)))
                    _baskets.Add(newId, basket);
                else return null;
            }
            return basket;
        }

        public bool DeleteBasket(long id)
        {
            return _baskets.Remove(id);
        }

        public bool UpdateBasket(long id, Basket basket)
        {
            if (basket?.Id != id)
                return false;
            if (!_baskets.ContainsKey(id))
                return false;
            lock (this)
            {
                // validate orders
                if (basket.Orders.TrueForAll(o => ContainsLineItem(o.Item.Id)))
                    _baskets[id] = basket;
                else return false;
            }
            return true;
        }

        public bool ClearBasket(long basketId)
        {
            if (!_baskets.TryGetValue(basketId, out Basket basket))
                return false;
            basket.Orders.Clear();
            return true;
        }

        public void Dispose()
        {
            _items?.Clear();
            _items = null;
            _baskets?.Clear();
            _baskets = null;
        }
    }
}
