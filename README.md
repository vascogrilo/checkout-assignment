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

### 3. REST Api

No authentication here, keep it simple.

#### 3.1. /api/v1/items

The following actions are allowed on this resource:

##### 3.1.1. GET /api/v1/items
Retrieves all items.

Accepts query params for filtering:
* **text** finds items that contains a certain text
* **name** finds items with a specific name
* **price** finds items with a specific price
* **priceAbove** finds items above a certain price
* **priceBelow** finds items below a certain price

Accepts query params for ordering:
* **sort** must be a valid field name, preprended with '-' for descending order.

##### 3.1.2. GET /api/v1/items/{id}
Retrieves item with specific **id**.

##### 3.1.3. POST /api/v1/items
Creates a new item.

##### 3.1.4. PUT /api/v1/items/{id}
Updates an existing item, identified by **id**.

##### 3.1.5. DELETE /api/v1/items/{id}
Deletes item with specific **id**.

##### 3.1.6. PUT /api/v1/items/{id}/order/{basketId}/{amount}
Creates or merges a new order of **amount** of item with specific **id** to basket with specific **basketId**.
This allows for conveniently adding orders to a basket instead of having to update the basket resource all the time.

#### 3.2. /api/v1/baskets

The following actions are allowed on this resource:

##### 3.2.1. GET /api/v1/baskets
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

##### 3.2.2. GET /api/v1/baskets/{id}
Retrieves basket with specific **id**.

##### 3.2.3. POST /api/v1/baskets
Creates a new basket.

##### 3.2.4. PUT /api/v1/baskets/{id}
Updates an existing basket, identified by **id**.

##### 3.2.5. DELETE /api/v1/baskets/{id}
Deletes basket with specific **id**.

##### 3.2.6. PUT /api/v1/baskets/{id}/clear
Clears all orders from a basket with specific **id**.

##### 3.2.7. PUT /api/v1/baskets/{id}/deleteOrder/{itemId}
Removes the order for a specific **itemId** within the basket.

##### 3.2.8 GET /api/v1/baskets/{id}/price
Retrieves the total price of all orders within the basket.
