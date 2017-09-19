using System;
using System.Collections.Generic;
using CheckoutAssignment.Models;

namespace CheckoutAssignment.Storage
{
    /// <summary>
    /// Basic behavior for a storage layer for <see cref="Item"/> and <see cref="Basket"/> resources.
    /// </summary>
    public interface IApplicationStorage : IDisposable
    {
        /// <summary>
        /// Retrieves all <see cref="Item"/>s.
        /// </summary>
        /// <returns>An enumeration of all items.</returns>
        IEnumerable<Item> GetLineItems();
        /// <summary>
        /// Retrieves a specific <see cref="Item"/>.
        /// </summary>
        /// <param name="id">The item's id.</param>
        /// <returns>The item, if found, null otherwise.</returns>
        Item GetLineItem(long id);
        /// <summary>
        /// Checks for the presence of a <see cref="Item"/> with a specific id.
        /// </summary>
        /// <param name="id">The item id to be checked.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        bool ContainsLineItem(long id);
        /// <summary>
        /// Creates a new <see cref="Item"/> in the storage.
        /// Assigns a new id to the provided <see cref="item"/>.
        /// </summary>
        /// <param name="item">The item to be created.</param>
        /// <returns>The created item, with its new id.</returns>
        Item CreateLineItem(Item item);
        /// <summary>
        /// Updates an existing <see cref="Item"/> according to the provided <see cref="item"/>.
        /// </summary>
        /// <param name="id">The id of the item that must be updated.</param>
        /// <param name="item">The item containing the new changes.</param>
        /// <returns>True if the item exists and <see cref="id"/> matches <see cref="item"/>.Id. False otherwise.</returns>
        bool UpdateLineItem(long id, Item item);
        /// <summary>
        /// Deletes an existing <see cref="Item"/>.
        /// </summary>
        /// <param name="id">The id of the item to be deleted.</param>
        /// <returns>True if the item existed, false otherwise.</returns>
        bool DeleteLineItem(long id);
        /// <summary>
        /// Retrieves all <see cref="Basket"/>s.
        /// </summary>
        /// <returns>An enumeration of all baskets.</returns>
        IEnumerable<Basket> GetBaskets();
        /// <summary>
        /// Retrieves a specific <see cref="Basket"/>.
        /// </summary>
        /// <param name="id">The id of the basket.</param>
        /// <returns>The basket, if it exists, null otherwise.</returns>
        Basket GetBasket(long id);
        /// <summary>
        /// Checks for the presence of a <see cref="Basket"/> with a specific id.
        /// </summary>
        /// <param name="id">The basket id to be checked.</param>
        /// <returns>True if the basket exists, false otherwise.</returns>
        bool ContainsBasket(long id);
        /// <summary>
        /// Creates a new <see cref="Basket"/> in the storage.
        /// Filters the list of orders inside the <see cref="basket"/> to orders that have a positive amount and <see cref="Item"/>s that exist.
        /// Assigns a new id to <see cref="basket"/>.
        /// </summary>
        /// <param name="basket">The basket to be created.</param>
        /// <returns>The new basket, with its new id.</returns>
        Basket CreateBasket(Basket basket);
        /// <summary>
        /// Updates an existing <see cref="Basket"/> according to the provided <see cref="basket"/>.
        /// </summary>
        /// <param name="id">The id of the basket to be updated.</param>
        /// <param name="basket">The basket containing the new changes.</param>
        /// <returns>True if the item exists and <see cref="id"/> matches <see cref="basket"/>.Id. False otherwise.</returns>
        bool UpdateBasket(long id, Basket basket);
        /// <summary>
        /// Deletes an existing <see cref="Basket"/>.
        /// </summary>
        /// <param name="id">The id of the basket to be deleted.</param>
        /// <returns>True if the basket existed, false otherwise.</returns>
        bool DeleteBasket(long id);
        /// <summary>
        /// Clears all <see cref="ItemOrder"/> in an existing <see cref="Basket"/>.
        /// </summary>
        /// <param name="basketId">The basket id.</param>
        /// <returns>True if the basket exists, false otherwise.</returns>
        bool ClearBasket(long basketId);
    }
}