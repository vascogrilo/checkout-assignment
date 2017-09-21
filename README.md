# Checkout.com coding assignment

The first part consisted of implementing a REST Web API for a basket of items that can be updated whenever and processing of this basket would happen by another component (outside of the scope of this assignment).

The second part of the assingment consisted in implementing a .NET client that could leverage this API and server as a basis for third parties interacting with the Web API.

I grabbed this opportunity to finally try out the recently new .NET Core framework and ASP.NET Core. I've been hearing good things about it and jumped into some tutorials to do this. This new framework is interesting and exciting and I absolutely loved to learn this (thanks!). Maybe it took some extra time to acomplish some things but the trade-off was worth it to learn something new.

## 1. Data models

This is a coding assignment for a recruitment process so my key assumption here is just to keep it simple and to the point.

### 1.1. Item

An item is the basis for a product order inside a basket. It contains the following attributes:
```csharp
public class Item
{
    public long Id { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public DateTime LastModified { get;set; }
}
```

#### 1.1.1. Validation
* Name is required
* Price must be positive

### 1.2. ItemOrder

Orders are what will be inside a basket, as common sense will state, it's basically a product reference and an amount.
```csharp
public class ItemOrder
{
    public Item Item { get; set; }
    public uint Amount { get; set; }
    public DateTime LastModified { get; set; }
}
```

#### 1.2.1. Validation
* Item must be present in storage and the contents of this instance equal to the one it references through the Id.
* Amount must be positive.

### 1.3. Basket

This is the main model for this assignment. Creating and interacting with baskets is the key functionality here.
It contains the following attributes:
```csharp
public class Basket
{
    public long Id { get; set; }
    public string Owner { get; set; }
    public List<ItemOrder> Orders { get; set; }
    public DateTime LastModified { get; set; }
}
```

I decided to not implement an elaborate concept of 'Owner' to keep it simple and cut time, in my opinion a simple string is enough for this purpose.

#### 1.3.1. Validation
* Owner is required.
* Orders must all be valid according to ItemOrder validation rules

## 2. Storage

For the sake of this scope everything is stored in memory. Controllers interact with an **IApplicationStorage** and it would be really simple to replace the current implementation with a different one.

### 2.1 IApplicationStorage

The basic Api was outlined as follows:

```csharp
IEnumerable<Item> GetLineItems();
Item GetLineItem(long id);
bool ContainsLineItem(long id);
Item CreateLineItem(Item item);
bool UpdateLineItem(long id, Item item);
bool DeleteLineItem(long id);
IEnumerable<Basket> GetBaskets();
Basket GetBasket(long id);
bool ContainsBasket(long id);
Basket CreateBasket(Basket basket);
bool UpdateBasket(long id, Basket basket);
bool DeleteBasket(long id);
bool ClearBasket(long basketId);
```

Required traits:
* Creating a new item must assign a new id to it as well as set the initial timestamp on it.
* Creating a new basket must assign a new id to it as well as set the initial timestamp on it.
* Updating an existing item must update all orders that contain a reference to it and change the item's and order's last modified property.
* Deleting an existing item must remove all orders that contain a reference to it and change the order's last modified property.
* Updating an existing basket must change the last modified timestamp property.
* Clearing a basket's orders must change the last modified timestamp property.

#### 2.1.1 Design comments

I'm not a big fan of having an object reference of **Item** inside **ItemOrder** since it forces me to search for these instances and update them accordingly when updates to items are issued, or deletions occurs, etc. This is very error-prone. This was done because I wanted to clearly see the items when querying baskets.

Still, I feel the best approach would be to have only item ids in the orders and separate clearly what is the model that gets persisted and the actual view model that gets returned via the Api. I did not go down this road due to time constraints, I followed a more pragmatic path.

## 3. REST Api

No authentication here, keep it simple.

### 3.1. /api/v1/items

The following actions are allowed on this resource:

#### 3.1.1. GET /api/v1/items
Retrieves all items.

Accepts query params for filtering:
* **text** finds items that contains a certain text
* **name** finds items with a specific name
* **price** finds items with a specific price
* **priceAbove** finds items above a certain price
* **priceBelow** finds items below a certain price

Accepts query params for ordering:
* **sort** must be a valid field name, preprended with '-' for descending order.

#### 3.1.2. GET /api/v1/items/{id}
Retrieves item with specific **id**.

#### 3.1.3. POST /api/v1/items
Creates a new item.

#### 3.1.4. PUT /api/v1/items/{id}
Updates an existing item, identified by **id**.

#### 3.1.5. DELETE /api/v1/items/{id}
Deletes item with specific **id**.

#### 3.1.6. PUT /api/v1/items/{id}/order/{basketId}/{amount}
Creates or merges a new order of **amount** of item with specific **id** to basket with specific **basketId**.
This allows for conveniently adding orders to a basket instead of having to update the basket resource all the time.

### 3.2. /api/v1/baskets

The following actions are allowed on this resource:

#### 3.2.1. GET /api/v1/baskets
Retrieves all baskets.

Accepts query params for filtering:
* **owner** finds baskets with a specific owner
* **itemText** finds baskets with items in orders with a certain text
* **itemId** finds baskets with orders that have a certain item id
* **amountsAbove** finds baskets with amounts of orders above a certain value
* **amounsBelow** finds baskets with amounts of orders below a certain value
* **ordersAbove** finds baskets with number of orders above a certain value
* **ordersBelow** finds baskets with number of orders below a certain value
* **price** finds baskets with a certain price
* **priceAbove** finds baskets above a certain price
* **priceBelow** finds baskets below a certain price

Accepts query params for ordering:
* **sort** must be a valid field name, preprended with '-' for descending order.

#### 3.2.2. GET /api/v1/baskets/{id}
Retrieves basket with specific **id**.

#### 3.2.3. POST /api/v1/baskets
Creates a new basket.

#### 3.2.4. PUT /api/v1/baskets/{id}
Updates an existing basket, identified by **id**.

#### 3.2.5. DELETE /api/v1/baskets/{id}
Deletes basket with specific **id**.

#### 3.2.6. PUT /api/v1/baskets/{id}/clear
Clears all orders from a basket with specific **id**.

#### 3.2.7. PUT /api/v1/baskets/{id}/deleteOrder/{itemId}
Removes the order for a specific **itemId** within the basket.

#### 3.2.8 GET /api/v1/baskets/{id}/price
Retrieves the total price of all orders within the basket.

## 4. .NET Client

Under _CheckoutAssignment.Client_ I have implemented a .NET client to leverage the REST Web Api and allow clients to use this in their code.
The client uses _HttpClient_ to connect to the remote server and issue requests.

The following api is available in the client:

```csharp
/// <summary>
/// Retrieves all existing items.
/// </summary>
/// <returns>A list of <see cref="Item"/>s</returns>
public async Task<List<Item>> GetItems();

/// <summary>
/// Retrieves all existing items.
/// Allows for filtering with <see cref="ItemFilteringSpec"/> and ordering by field name.
/// For descending order preprend the field name with '-'.
/// </summary>
/// <returns>A list of <see cref="Item"/>s</returns>
public async Task<List<Item>> GetItems(ItemFilteringSpec filters, string sortByFieldName = null);

/// <summary>
/// Retrieves all existing baskets.
/// </summary>
/// <returns>A list of <see cref="Basket"/>s</returns>
public async Task<List<Basket>> GetBaskets();

/// <summary>
/// Retrieves all existing baskets.
/// Allows for filtering with <see cref="BasketFilteringSpec"/> and ordering by field name.
/// For descending order preprend the field name with '-'.
/// </summary>
/// <returns>A list of <see cref="Basket"/>s</returns>
public async Task<List<Basket>> GetBaskets(BasketFilteringSpec filters, string sortByFieldName = null);

/// <summary>
/// Retrieves a specific <see cref="Item"/>.
/// </summary>
/// <param name="id">The item's id.</param>
/// <returns>The item if it exists, null otherwise.</returns>
public async Task<Item> GetItem(long id);

/// <summary>
/// Retrives a specific <see cref="Basket"/>.
/// </summary>
/// <param name="id">The basket's id.</param>
/// <returns>The basket if it exists, null otherwise.</returns>
public async Task<Basket> GetBasket(long id);

/// <summary>
/// Creates a new <see cref="Item"/> with the provided name and price.
/// </summary>
/// <param name="name">The name for the new item.</param>
/// <param name="price">The price for the new item. Price should be positive.</param>
/// <exception cref="InvalidRequestException">If response returned by server is a BadRequest.</exception>
/// <exception cref="ForbiddenActionException">If response returned by server is a Forbidden.</exception>
/// <returns>The new <see cref="Item"/>, with assigned Id if the operation was successful. Null otherwise.</returns>
public async Task<Item> CreateItem(string name, float price);

/// <summary>
/// Creates a new empty <see cref="Basket"/> with the provided owner.
/// </summary>
/// <param name="owner">The owner name for the new basket.</param>
/// <exception cref="InvalidRequestException">If the response returned by the server is a BadRequest.</exception>
/// <exception cref="ForbiddenActionException">If the response returned by the server is a Forbidden.</exception>
/// <returns>The new <see cref="Basket"/>, with assigned Id if the operation was successful. Null otherwise.</returns>
public async Task<Basket> CreateBasket(string owner);

/// <summary>
/// Updates the provided <see cref="Item"/> server-side.
/// </summary>
/// <param name="item">The item to be updated.</param>
/// <exception cref="InvalidRequestException">If the response returned by the server is a BadRequest.</exception>
/// <exception cref="ForbiddenActionException">If the response returned by the server is a Forbidden.</exception>
/// <returns>True if the operation was successful, false otherwise.</returns>
public async Task<bool> UpdateItem(Item item);

/// <summary>
/// Updates the provided <see cref="Basket"/> server-side.
/// </summary>
/// <param name="item">The basket to be updated.</param>
/// <exception cref="InvalidRequestException">If the response returned by the server is a BadRequest.</exception>
/// <exception cref="ForbiddenActionException">If the response returned by the server is a Forbidden.</exception>
/// <returns>True if the operation was successful, false otherwise.</returns>
public async Task<bool> UpdateBasket(Basket basket);

/// <summary>
/// Tries to delete the <see cref="Item"/> with the specified Id.
/// </summary>
/// <param name="id">The item's id.</param>
/// <returns>True if the item was present, false otherwise.</returns>
public async Task<bool> DeleteItem(long id);

/// <summary>
/// Tries to delete the <see cref="Basket"/> with the specified Id.
/// </summary>
/// <param name="id">The basket's id.</param>
/// <returns>True if the basket was present, false otherwise.</returns>
public async Task<bool> DeleteBasket(long id);

/// <summary>
/// Tries to register a new order for an <see cref="Item"/> in the desired <see cref="Basket"/>.
/// Item is identified by it's id and basket as well.
/// If an order already exists for this item, the amounts will be summed.
/// Otherwise a new item will be placed inside the basket.
/// Ensure item and basket exist.
/// </summary>
/// <param name="itemId">The item's id.</param>
/// <param name="basketId">The basket's id.</param>
/// <param name="amount">The order amount for the item.</param>
/// <exception cref="InvalidRequestException">If either the item or basket don't exist or the amount specified was not positive.</exception>
/// <returns>True if registering the order was a success, false otherwise.</returns>
public async Task<bool> OrderItemToBasket(long itemId, long basketId, int amount);

/// <summary>
/// Tries to delete a certain order from a basket.
/// The order will be identified by its item id.
/// </summary>
/// <param name="itemId">The order's item id to delete.</param>
/// <param name="basketId">The basket id in which we want to delete the order from.</param>
/// <returns>True if the basket existed and the order was present, false otherwise.</returns>
public async Task<bool> DeleteOrderFromBasket(long itemId, long basketId);

/// <summary>
/// Clears the list of orders from a <see cref="Basket"/>.
/// </summary>
/// <param name="basketId">The basket's id.</param>
/// <returns>True if the basket existed, false otherwise.</returns>
public async Task<bool> ClearBasket(long basketId);

/// <summary>
/// Retrieves the total price of a <see cref="Basket"/>.
/// </summary>
/// <param name="basketId">The basket's id.</param>
/// <returns>The basket's total price, NaN if the basket does not exist.</returns>
public async Task<float> GetBasketPrice(long basketId);
```

And the filtering specifications:

```chsarp
public class ItemFilteringSpec
{
    public string HasText { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public float PriceAbove { get; set; }
    public float PriceBelow { get; set; }
}

public class BasketFilteringSpec
{
    public string Owner { get; set; }
    public string HasItemText { get; set; }
    public long HasItemId { get; set; }
    public uint AmountsAbove { get; set; }
    public uint AmountsBelow { get; set; }
    public uint OrdersAbove { get; set; }
    public uint OrdersBelow { get; set; }
    public float Price { get; set; }
    public float PriceAbove { get; set; }
    public float PriceBelow { get; set; }
}
```
