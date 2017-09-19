using CheckoutAssignment.Models;
using CheckoutAssignment.Storage;
using Xunit;

namespace CheckoutAssignment.Test
{
    public class ApplicationStorageTest
    {
        private static IApplicationStorage NewApplicationStorage()
        {
            return new AppStorage();
        }

        [Fact]
        public void CreateLineItemsDifferentId()
        {
            using (var storage = NewApplicationStorage())
            {
                var item1 = new Item();
                item1 = storage.CreateLineItem(item1);
                Assert.NotNull(item1);
                var item2 = new Item();
                item2 = storage.CreateLineItem(item2);
                Assert.NotNull(item2);
                Assert.NotEqual(item1.Id, item2.Id);
            }
        }

        [Fact]
        public void CreateLineItem()
        {
            using (var storage = NewApplicationStorage())
            {
                var item = new Item { Name = "Gibson SG", Price = 345.56f };
                item = storage.CreateLineItem(item);
                Assert.NotNull(item);
                Assert.Equal("Gibson SG", item.Name);
                Assert.Equal(345.56f, item.Price, 2);
            }
        }

        [Fact]
        public void UpdateLineItem()
        {
            using (var storage = NewApplicationStorage())
            {
                var item = new Item { Name = "Gibson SG", Price = 345.56f };
                item = storage.CreateLineItem(item);
                Assert.NotNull(item);
                Assert.Equal("Gibson SG", item.Name);
                Assert.Equal(345.56f, item.Price, 2);

                item.Price = 34.5f;
                item.Name = "Gibson SG broken";
                Assert.True(storage.UpdateLineItem(item.Id, item));
                item = storage.GetLineItem(item.Id);

                Assert.Equal("Gibson SG broken", item.Name);
                Assert.Equal(34.5f, item.Price, 2);
            }
        }

        [Fact]
        public void UpdateLineItemInvalidId()
        {
            using (var storage = NewApplicationStorage())
            {
                var item = new Item { Name = "Gibson SG", Price = 345.56f };
                item = storage.CreateLineItem(item);
                Assert.NotNull(item);
                Assert.Equal("Gibson SG", item.Name);
                Assert.Equal(345.56f, item.Price, 2);

                item.Price = 34.5f;
                item.Name = "Gibson SG broken";
                Assert.False(storage.UpdateLineItem(34, item));

                var id = item.Id;
                item.Id = 33;
                Assert.False(storage.UpdateLineItem(id, item));
            }
        }

        [Fact]
        public void GetAllLineItems()
        {
            using (var storage = NewApplicationStorage())
            {
                var item1 = new Item { Name = "Gibson SG", Price = 345.56f };
                item1 = storage.CreateLineItem(item1);
                Assert.NotNull(item1);
                Assert.Equal("Gibson SG", item1.Name);
                Assert.Equal(345.56f, item1.Price, 2);

                var item2 = new Item { Name = "Peavey ValveKing", Price = 220.3f };
                item2 = storage.CreateLineItem(item2);
                Assert.NotNull(item2);
                Assert.Equal("Peavey ValveKing", item2.Name);
                Assert.Equal(220.3f, item2.Price, 2);

                var items = storage.GetLineItems();
                Assert.Contains(item1, items);
                Assert.Contains(item2, items);
            }
        }

        [Fact]
        public void ContainsLineItem()
        {
            using (var storage = NewApplicationStorage())
            {
                var item1 = new Item { Name = "Gibson SG", Price = 345.56f };
                item1 = storage.CreateLineItem(item1);
                Assert.NotNull(item1);
                Assert.Equal("Gibson SG", item1.Name);
                Assert.Equal(345.56f, item1.Price, 2);

                Assert.True(storage.ContainsLineItem(item1.Id));
                Assert.False(storage.ContainsLineItem(42));
            }
        }

        [Fact]
        public void DeleteLineItem()
        {
            using (var storage = NewApplicationStorage())
            {
                var item1 = new Item { Name = "Gibson SG", Price = 345.56f };
                item1 = storage.CreateLineItem(item1);
                Assert.NotNull(item1);
                Assert.Equal("Gibson SG", item1.Name);
                Assert.Equal(345.56f, item1.Price, 2);

                var item2 = new Item { Name = "Peavey ValveKing", Price = 220.3f };
                item2 = storage.CreateLineItem(item2);
                Assert.NotNull(item2);
                Assert.Equal("Peavey ValveKing", item2.Name);
                Assert.Equal(220.3f, item2.Price, 2);

                Assert.True(storage.DeleteLineItem(item1.Id));
                Assert.False(storage.DeleteLineItem(42));

                Assert.Contains(item2, storage.GetLineItems());
            }
        }

        [Fact]
        public void CreateBasketsDifferentId()
        {
            using (var storage = NewApplicationStorage())
            {
                var basket1 = new Basket();
                basket1 = storage.CreateBasket(basket1);
                Assert.NotNull(basket1);
                var basket2 = new Basket();
                basket2 = storage.CreateBasket(basket2);
                Assert.NotNull(basket2);
                Assert.NotEqual(basket1.Id, basket2.Id);
            }
        }

        [Fact]
        public void CreateEmptyBasket()
        {
            using (var storage = NewApplicationStorage())
            {
                var basket = new Basket { Owner = "vasco" };
                basket = storage.CreateBasket(basket);
                Assert.NotNull(basket);
                Assert.Equal("vasco", basket.Owner);
                Assert.Empty(basket.Orders);
            }
        }

        [Fact]
        public void CreateBasketWithInvalidOrders()
        {
            using (var storage = NewApplicationStorage())
            {
                var item1 = new Item();
                var order1 = new ItemOrder { Item = item1, Amount = 1 };
                var basket = new Basket { Owner = "me", Orders = { order1 } };
                basket = storage.CreateBasket(basket);
                Assert.Null(basket);
            }
        }

        [Fact]
        public void CreateBasketWithValidOrders()
        {
            using (var storage = NewApplicationStorage())
            {
                var item1 = new Item { Name = "Gibson SG", Price = 345.56f };
                item1 = storage.CreateLineItem(item1);
                var item2 = new Item { Name = "Peavey ValveKing", Price = 220.3f };
                item2 = storage.CreateLineItem(item2);
                var item3 = new Item { Name = "Big Muff Pi", Price = 20f };
                item3 = storage.CreateLineItem(item3);

                var order1 = new ItemOrder { Item = item1, Amount = 1 };
                var order2 = new ItemOrder { Item = item2, Amount = 2 };
                var basket = new Basket { Owner = "me", Orders = { order1, order2 } };
                basket = storage.CreateBasket(basket);
                Assert.NotNull(basket);

                Assert.Equal("me", basket.Owner);
                Assert.Equal(2, basket.Orders.Count);
                Assert.Contains(order1, basket.Orders);
                Assert.Contains(order2, basket.Orders);
            }
        }

        [Fact]
        public void UpdateBasket()
        {
            using (var storage = NewApplicationStorage())
            {
                var basket = new Basket { Owner = "me" };
                basket = storage.CreateBasket(basket);
                Assert.NotNull(basket);

                basket.Owner = "vasco";
                Assert.True(storage.UpdateBasket(basket.Id, basket));
            }
        }

        [Fact]
        public void UpdateBasketInvalidId()
        {
            using (var storage = NewApplicationStorage())
            {
                var basket = new Basket { Owner = "me" };
                basket = storage.CreateBasket(basket);
                Assert.NotNull(basket);

                basket.Owner = "vasco";
                Assert.False(storage.UpdateBasket(23, basket));

                var id = basket.Id;
                basket.Id = 32;
                Assert.False(storage.UpdateBasket(id, basket));
            }
        }

        [Fact]
        public void GetAllBaskets()
        {
            using (var storage = NewApplicationStorage())
            {
                var basket1 = new Basket { Owner = "me" };
                basket1 = storage.CreateBasket(basket1);
                Assert.NotNull(basket1);
                var basket2 = new Basket { Owner = "vasco" };
                basket2 = storage.CreateBasket(basket2);
                Assert.NotNull(basket2);

                var baskets = storage.GetBaskets();
                Assert.Contains(basket1, baskets);
                Assert.Contains(basket2, baskets);
            }
        }

        [Fact]
        public void ContainsBasket()
        {
            using (var storage = NewApplicationStorage())
            {
                var basket1 = new Basket { Owner = "me" };
                basket1 = storage.CreateBasket(basket1);
                Assert.NotNull(basket1);

                Assert.True(storage.ContainsBasket(basket1.Id));
                Assert.False(storage.ContainsBasket(23));
            }
        }

        [Fact]
        public void DeleteBasket()
        {
            using (var storage = NewApplicationStorage())
            {
                var basket1 = new Basket { Owner = "me" };
                basket1 = storage.CreateBasket(basket1);
                Assert.NotNull(basket1);
                var basket2 = new Basket { Owner = "vasco" };
                basket2 = storage.CreateBasket(basket2);
                Assert.NotNull(basket2);

                Assert.True(storage.DeleteBasket(basket1.Id));
                Assert.False(storage.DeleteBasket(23));
                Assert.Null(storage.GetBasket(basket1.Id));
            }
        }

        [Fact]
        public void ClearBasket()
        {
            using (var storage = NewApplicationStorage())
            {
                var item1 = new Item { Name = "Gibson SG", Price = 345.56f };
                item1 = storage.CreateLineItem(item1);
                var item2 = new Item { Name = "Peavey ValveKing", Price = 220.3f };
                item2 = storage.CreateLineItem(item2);
                var item3 = new Item { Name = "Big Muff Pi", Price = 20f };
                item3 = storage.CreateLineItem(item3);

                var order1 = new ItemOrder { Item = item1, Amount = 1 };
                var order2 = new ItemOrder { Item = item2, Amount = 2 };
                var basket = new Basket { Owner = "me", Orders = { order1, order2 } };
                basket = storage.CreateBasket(basket);
                Assert.NotNull(basket);
                Assert.True(storage.ClearBasket(basket.Id));
                Assert.False(storage.ClearBasket(334));
                basket = storage.GetBasket(basket.Id);
                Assert.NotNull(basket);
                Assert.Empty(basket.Orders);
            }
        }
    }
}
