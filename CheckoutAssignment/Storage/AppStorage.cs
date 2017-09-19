using System.Collections.Generic;
using CheckoutAssignment.Models;

namespace CheckoutAssignment.Storage
{
    /// <summary>
    /// Basic in-memory tables for <see cref="Basket"/> and <see cref="Item"/>.
    /// Implements <see cref="IApplicationStorage"/> behaviour.
    /// </summary>
    public class AppStorage : IApplicationStorage
    {
        private Dictionary<long, Item> _items;
        private Dictionary<long, Basket> _baskets;
        private const int _idSeed = 1;

        /// <summary>
        /// Creates new instance of <see cref="Basket"/> and <see cref="Item"/> tables.
        /// Also creates stub data to start of with (for now).
        /// </summary>
        public AppStorage()
        {
            _items = new Dictionary<long, Item>();
            _baskets = new Dictionary<long, Basket>();
            CreateLineItem(new Item { Name = "Gibson SG left-handed", Price = 250.99f });
            CreateLineItem(new Item { Name = "Peavey ValveKing Tube amp", Price = 220f });
            CreateBasket(new Basket { Owner = "vasco" });
            CreateBasket(new Basket { Owner = "james", Orders = { new ItemOrder { Item = GetLineItem(1), Amount = 1 } } });
        }

        /// <summary>
        /// Checks for the presence of a <see cref="Item"/> with a specific id.
        /// </summary>
        /// <param name="id">The item id to be checked.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public bool ContainsLineItem(long id)
        {
            return _items.ContainsKey(id);
        }

        /// <summary>
        /// Retrieves all <see cref="Item"/>s.
        /// </summary>
        /// <returns>An enumeration of all items.</returns>
        public IEnumerable<Item> GetLineItems()
        {
            return _items.Values;
        }

        /// <summary>
        /// Retrieves a specific <see cref="Item"/>.
        /// </summary>
        /// <param name="id">The item's id.</param>
        /// <returns>The item, if found, null otherwise.</returns>
        public Item GetLineItem(long id)
        {
            if (!_items.TryGetValue(id, out Item item))
                return null;
            return item;
        }

        /// <summary>
        /// Creates a new <see cref="Item"/> in the storage.
        /// Assigns a new id to the provided <see cref="item"/>.
        /// </summary>
        /// <param name="item">The item to be created.</param>
        /// <returns>The created item, with its new id.</returns>
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

        /// <summary>
        /// Updates an existing <see cref="Item"/> according to the provided <see cref="item"/>.
        /// </summary>
        /// <param name="id">The id of the item that must be updated.</param>
        /// <param name="item">The item containing the new changes.</param>
        /// <returns>True if the item exists and <see cref="id"/> matches <see cref="item"/>.Id. False otherwise.</returns>
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

        /// <summary>
        /// Deletes an existing <see cref="Item"/>.
        /// </summary>
        /// <param name="id">The id of the item to be deleted.</param>
        /// <returns>True if the item existed, false otherwise.</returns>
        public bool DeleteLineItem(long id)
        {
            return _items.Remove(id);
        }

        /// <summary>
        /// Retrieves a specific <see cref="Basket"/>.
        /// </summary>
        /// <param name="id">The id of the basket.</param>
        /// <returns>The basket, if it exists, null otherwise.</returns>
        public Basket GetBasket(long id)
        {
            if (!_baskets.TryGetValue(id, out Basket basket))
                return null;
            return basket;
        }

        /// <summary>
        /// Checks for the presence of a <see cref="Basket"/> with a specific id.
        /// </summary>
        /// <param name="id">The basket id to be checked.</param>
        /// <returns>True if the basket exists, false otherwise.</returns>
        public bool ContainsBasket(long id)
        {
            return _baskets.ContainsKey(id);
        }

        /// <summary>
        /// Retrieves all <see cref="Basket"/>s.
        /// </summary>
        /// <returns>An enumeration of all baskets.</returns>
        public IEnumerable<Basket> GetBaskets()
        {
            return _baskets.Values;
        }

        /// <summary>
        /// Creates a new <see cref="Basket"/> in the storage.
        /// Filters the list of orders inside the <see cref="basket"/> to orders that have a positive amount and <see cref="Item"/>s that exist.
        /// Assigns a new id to <see cref="basket"/>.
        /// </summary>
        /// <param name="basket">The basket to be created.</param>
        /// <returns>The new basket, with its new id.</returns>
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

        /// <summary>
        /// Deletes an existing <see cref="Basket"/>.
        /// </summary>
        /// <param name="id">The id of the basket to be deleted.</param>
        /// <returns>True if the basket existed, false otherwise.</returns>
        public bool DeleteBasket(long id)
        {
            return _baskets.Remove(id);
        }

        /// <summary>
        /// Updates an existing <see cref="Basket"/> according to the provided <see cref="basket"/>.
        /// </summary>
        /// <param name="id">The id of the basket to be updated.</param>
        /// <param name="basket">The basket containing the new changes.</param>
        /// <returns>True if the item exists and <see cref="id"/> matches <see cref="basket"/>.Id. False otherwise.</returns>
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

        /// <summary>
        /// Clears all <see cref="ItemOrder"/> in an existing <see cref="Basket"/>.
        /// </summary>
        /// <param name="basketId">The basket id.</param>
        /// <returns>True if the basket exists, false otherwise.</returns>
        public bool ClearBasket(long basketId)
        {
            if (!_baskets.TryGetValue(basketId, out Basket basket))
                return false;
            basket.Orders.Clear();
            return true;
        }

        /// <summary>
        /// Clears in-memory tables.
        /// </summary>
        public void Dispose()
        {
            _items?.Clear();
            _items = null;
            _baskets?.Clear();
            _baskets = null;
        }
    }
}
