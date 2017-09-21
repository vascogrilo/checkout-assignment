using CheckoutAssignment.Client;
using CheckoutAssignment.Models;
using System;
using static CheckoutAssignment.Client.BasketsApiClient;

namespace CheckoutAssignment.ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test(args);
            Console.ReadLine();
        }

        public static async void Test(string[] args)
        {
            var client = new BasketsApiClient(args[0]);
            foreach (var item in await client.GetItems())
            {
                PrintItem(item, "");
                Console.WriteLine("");
            }
            foreach (var basket in await client.GetBaskets())
            {
                PrintBasket(basket);
                Console.WriteLine("");
            }
            Console.WriteLine(await client.GetItem(3) == null);
            Console.WriteLine("");
            PrintBasket(await client.GetBasket(2));
            PrintItem(await client.CreateItem("Guitar Picks, box of 8", 2f), "");

            var itemFilter = new ItemFilteringSpec { HasText = "Guitar" };
            foreach(var item in await client.GetItems(itemFilter))
                PrintItem(item, "");

            itemFilter = new ItemFilteringSpec { PriceAbove = 220 };
            foreach (var item in await client.GetItems(itemFilter))
                PrintItem(item, "");

            var basketFilter = new BasketFilteringSpec { OrdersAbove = 1 };
            foreach (var basket in await client.GetBaskets(basketFilter))
                PrintBasket(basket);
        }

        private static void PrintItem(Item item, string prefix)
        {
            Console.WriteLine($"{prefix}Item {item.Id}");
            Console.WriteLine($"{prefix}\tName: {item.Name}");
            Console.WriteLine($"{prefix}\tPrice: {item.Price}");
        }

        private static void PrintBasket(Basket basket)
        {
            Console.WriteLine($"Basket {basket.Id}");
            Console.WriteLine($"\tOwner: {basket.Owner}");
            Console.WriteLine($"\tOrders:");
            foreach (var order in basket.Orders)
            {
                Console.WriteLine($"\t\tAmount: {order.Amount}");
                PrintItem(order.Item, "\t\t");
            }
        }
    }
}
