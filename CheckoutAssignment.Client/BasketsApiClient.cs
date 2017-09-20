using CheckoutAssignment.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutAssignment.Client
{
    /// <summary>
    /// Simple client for accessing CheckoutAssignment Web API.
    /// There are two possible resources to interact with, through different actions, <see cref="Item"/> and <see cref="Basket"/>.
    /// An item represents a simple product and a basket a collection of orders on these products.
    /// </summary>
    public class BasketsApiClient
    {
        private const string BasketsV1BaseUrl = "/api/v1/baskets";
        private const string ItemsV1BaseUrl = "/api/v1/items";
        private HttpClient _httpClient;

        /// <summary>
        /// Creates a new instance of this client which uses internally an <see cref="HttpClient"/>.
        /// Provide the base url without any path, just the address. For instance: http://localhost:1428
        /// </summary>
        /// <param name="httpBaseUrl">The base address where the Web API is listening.</param>
        public BasketsApiClient(string httpBaseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(httpBaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "CheckoutAssignment Web API Client");
        }

        /// <summary>
        /// Retrieves all existing items.
        /// </summary>
        /// <returns>A list of <see cref="Item"/>s</returns>
        public async Task<List<Item>> GetItems()
        {
            var response = await _httpClient.GetStringAsync(ItemsV1BaseUrl);
            return JsonConvert.DeserializeObject<IEnumerable<Item>>(response) as List<Item>;
        }

        /// <summary>
        /// Retrieves all existing baskets.
        /// </summary>
        /// <returns>A list of <see cref="Basket"/>s</returns>
        public async Task<List<Basket>> GetBaskets()
        {
            var response = await _httpClient.GetStringAsync(BasketsV1BaseUrl);
            return JsonConvert.DeserializeObject<IEnumerable<Basket>>(response) as List<Basket>;
        }

        /// <summary>
        /// Retrieves a specific <see cref="Item"/>.
        /// </summary>
        /// <param name="id">The item's id.</param>
        /// <returns>The item if it exists, null otherwise.</returns>
        public async Task<Item> GetItem(long id)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{ItemsV1BaseUrl}/{id}");
                return JsonConvert.DeserializeObject<Item>(response);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        /// <summary>
        /// Retrives a specific <see cref="Basket"/>.
        /// </summary>
        /// <param name="id">The basket's id.</param>
        /// <returns>The basket if it exists, null otherwise.</returns>
        public async Task<Basket> GetBasket(long id)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BasketsV1BaseUrl}/{id}");
                return JsonConvert.DeserializeObject<Basket>(response);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Item"/> with the provided name and price.
        /// </summary>
        /// <param name="name">The name for the new item.</param>
        /// <param name="price">The price for the new item. Price should be positive.</param>
        /// <exception cref="InvalidRequestException">If response returned by server is a BadRequest.</exception>
        /// <exception cref="ForbiddenActionException">If response returned by server is a Forbidden.</exception>
        /// <returns>The new <see cref="Item"/>, with assigned Id if the operation was successful. Null otherwise.</returns>
        public async Task<Item> CreateItem(string name, float price)
        {
            try
            {
                var item = new Item { Name = name, Price = price };
                var response = await _httpClient.PostAsync(ItemsV1BaseUrl, new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new InvalidRequestException(response.ReasonPhrase);
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    throw new ForbiddenActionException(response.ReasonPhrase);

                return JsonConvert.DeserializeObject<Item>(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a new empty <see cref="Basket"/> with the provided owner.
        /// </summary>
        /// <param name="owner">The owner name for the new basket.</param>
        /// <exception cref="InvalidRequestException">If the response returned by the server is a BadRequest.</exception>
        /// <exception cref="ForbiddenActionException">If the response returned by the server is a Forbidden.</exception>
        /// <returns>The new <see cref="Basket"/>, with assigned Id if the operation was successful. Null otherwise.</returns>
        public async Task<Basket> CreateBasket(string owner)
        {
            try
            {
                var basket = new Basket { Owner = owner };
                var response = await _httpClient.PostAsync(BasketsV1BaseUrl, new StringContent(JsonConvert.SerializeObject(basket), Encoding.UTF8, "application/json"));

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new InvalidRequestException(response.ReasonPhrase);
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    throw new ForbiddenActionException(response.ReasonPhrase);

                return JsonConvert.DeserializeObject<Basket>(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        /// <summary>
        /// Updates the provided <see cref="Item"/> server-side.
        /// </summary>
        /// <param name="item">The item to be updated.</param>
        /// <exception cref="InvalidRequestException">If the response returned by the server is a BadRequest.</exception>
        /// <exception cref="ForbiddenActionException">If the response returned by the server is a Forbidden.</exception>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public async Task<bool> UpdateItem(Item item)
        {
            if (item == null)
                return false;
            try
            {
                var response = await _httpClient.PutAsync($"{ItemsV1BaseUrl}/{item.Id}", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"));

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new InvalidRequestException(response.ReasonPhrase);
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    throw new ForbiddenActionException(response.ReasonPhrase);

                if (response.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the provided <see cref="Basket"/> server-side.
        /// </summary>
        /// <param name="item">The basket to be updated.</param>
        /// <exception cref="InvalidRequestException">If the response returned by the server is a BadRequest.</exception>
        /// <exception cref="ForbiddenActionException">If the response returned by the server is a Forbidden.</exception>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public async Task<bool> UpdateBasket(Basket basket)
        {
            if (basket == null)
                return false;
            try
            {
                var response = await _httpClient.PutAsync($"{BasketsV1BaseUrl}/{basket.Id}", new StringContent(JsonConvert.SerializeObject(basket), Encoding.UTF8, "application/json"));

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new InvalidRequestException(response.ReasonPhrase);
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    throw new ForbiddenActionException(response.ReasonPhrase);

                if (response.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to delete the <see cref="Item"/> with the specified Id.
        /// </summary>
        /// <param name="id">The item's id.</param>
        /// <returns>True if the item was present, false otherwise.</returns>
        public async Task<bool> DeleteItem(long id)
        {
            return await DeleteResource(ItemsV1BaseUrl, id);
        }

        /// <summary>
        /// Tries to delete the <see cref="Basket"/> with the specified Id.
        /// </summary>
        /// <param name="id">The basket's id.</param>
        /// <returns>True if the basket was present, false otherwise.</returns>
        public async Task<bool> DeleteBasket(long id)
        {
            return await DeleteResource(BasketsV1BaseUrl, id);
        }

        private async Task<bool> DeleteResource(string path, long id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{path}/{id}");
                if (!response.IsSuccessStatusCode)
                    return false;
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

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
        public async Task<bool> OrderItemToBasket(long itemId, long basketId, int amount)
        {
            try
            {
                var response = await _httpClient.PutAsync($"{ItemsV1BaseUrl}/order/{basketId}/{amount}", new StringContent(null));

                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new InvalidRequestException("Either item or basket do not exist.");
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new InvalidRequestException("Amount must be positive.");

                if (!response.IsSuccessStatusCode)
                    return false;
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to delete a certain order from a basket.
        /// The order will be identified by its item id.
        /// </summary>
        /// <param name="itemId">The order's item id to delete.</param>
        /// <param name="basketId">The basket id in which we want to delete the order from.</param>
        /// <returns>True if the basket existed and the order was present, false otherwise.</returns>
        public async Task<bool> DeleteOrderFromBasket(long itemId, long basketId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"{BasketsV1BaseUrl}/{basketId}/deleteOrder/{itemId}", new StringContent(null));
                if (!response.IsSuccessStatusCode)
                    return false;
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the list of orders from a <see cref="Basket"/>.
        /// </summary>
        /// <param name="basketId">The basket's id.</param>
        /// <returns>True if the basket existed, false otherwise.</returns>
        public async Task<bool> ClearBasket(long basketId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"{BasketsV1BaseUrl}/{basketId}/clear", new StringContent(null));
                if (!response.IsSuccessStatusCode)
                    return false;
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the total price of a <see cref="Basket"/>.
        /// </summary>
        /// <param name="basketId">The basket's id.</param>
        /// <returns>The basket's total price, NaN if the basket does not exist.</returns>
        public async Task<float> GetBasketPrice(long basketId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BasketsV1BaseUrl}/{basketId}/price");
                return float.Parse(response);
            }
            catch (HttpRequestException)
            {
                return float.NaN;
            }
        }

        public class InvalidRequestException : Exception
        {
            public InvalidRequestException(string message) : base(message) { }
        }

        public class ForbiddenActionException : Exception
        {
            public ForbiddenActionException(string message) : base(message) { }
        }
    }
}
