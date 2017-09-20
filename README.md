# Checkout.com coding assignment

The first part consisted of implementing a REST Web API for a basket of items that can be updated whenever and processing of this basket would happen by another component (outside of the scope of this assignment).

The second part of the assingment consisted in implementing a .NET client that could leverage this API and server as a basis for third parties interacting with the Web API.

I grabbed this opportunity to finally try out the recently new .NET Core framework and ASP.NET Core. I've been hearing good things about it and jumped into some tutorials to do this. This new framework is pretty interesting and exciting and I absolutely loved to learn this (thanks!). Maybe it took some extra time to acomplish some things but the trade-off was worth it to learn something new.

## 1. Data models

This is a coding assignment for a recruitment process so my key assumption here is just to keep it simple and to the point.

### 1.1. Item

An item is the basis for a product order inside a basket. It contains the following attributes:
* ```public long Id { get; set; }```
* ```public string Name { get; set; }```
* ```public float Price { get; set; }```
* ```public DateTime LastModified { get; set; }```

### 1.2. ItemOrder

Orders are what will be inside a basket, as common sense will state, it's basically a product reference and an amount.
* ```public Item Item { get; set; }```
* ```public uint Amount { get; set; }```
* ```public DateTime LastModified { get; set; }```

### 1.3. Basket

This is the main model for this assignment. Creating and interacting with baskets is the key functionality here.
It contains the following attributes:
* ```public long Id { get; set; }```
* ```public string Owner { get; set; }```
* ```public List<ItemOrder> Orders { get; set; }```
* ```public DateTime LastModified { get; set; }```

I decided to not implement an elaborate concept of 'Owner' to keep it simple and cut time, in my opinion a simple string is enough for this purpose.

## 2. Storage

For the sake of this scope everything is stored in memory. Controllers interact with an **IApplicationStorage** and it would be really simple to replace the current implementation with a different one.

### 2.1 IApplicationStorage

The basic Api was outlined as follows:
* ```IEnumerable<Item> GetLineItems();```
* ```Item GetLineItem(long id);```
* ```bool ContainsLineItem(long id);```
* ```Item CreateLineItem(Item item);```
* ```bool UpdateLineItem(long id, Item item);```
* ```bool DeleteLineItem(long id);```
* ```IEnumerable<Basket> GetBaskets();```
* ```Basket GetBasket(long id);```
* ```bool ContainsBasket(long id);```
* ```Basket CreateBasket(Basket basket);```
* ```bool UpdateBasket(long id, Basket basket);```
* ```bool DeleteBasket(long id);```
* ```bool ClearBasket(long basketId);```

