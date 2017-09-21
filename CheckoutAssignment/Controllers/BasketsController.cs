using System.Collections.Generic;
using System.Linq;
using CheckoutAssignment.Models;
using CheckoutAssignment.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutAssignment.Controllers
{
    [Route("api/v1/baskets")]
    public class BasketsController : Controller
    {
        private readonly IApplicationStorage _storage;

        public BasketsController(IApplicationStorage storage)
        {
            _storage = storage;
        }

        [HttpGet]
        public IEnumerable<Basket> GetAll(string owner, string itemText, long itemId, uint amountsAbove, uint amountsBelow, uint ordersAbove, uint ordersBelow, float price, float priceBelow, float priceAbove, string sort)
        {
            // filtering
            var baskets = _storage.GetBaskets();
            if (!string.IsNullOrEmpty(owner))
                baskets = baskets.Where(b => b.Owner == owner);
            if (!string.IsNullOrEmpty(itemText))
                baskets = baskets.Where(b => b.Orders.Any(o => o.Item.Name.Contains(itemText)));
            if (itemId > 0)
                baskets = baskets.Where(b => b.Orders.Any(o => o.Item.Id == itemId));
            if (amountsAbove > 0)
                baskets = baskets.Where(b => b.Orders.Sum(o => o.Amount) > amountsAbove);
            if (amountsBelow > 0)
                baskets = baskets.Where(b => b.Orders.Sum(o => o.Amount) < amountsBelow);
            if (ordersAbove > 0)
                baskets = baskets.Where(b => b.Orders.Count > ordersAbove);
            if (ordersBelow > 0)
                baskets = baskets.Where(b => b.Orders.Count < ordersBelow);
            // exact price takes precedence
            if (price > 0)
                baskets = baskets.Where(b => b.GetPrice().Equals(price));
            else
            {
                if (priceAbove > 0)
                    baskets = baskets.Where(b => b.GetPrice() > priceAbove);
                if (priceBelow > 0)
                    baskets = baskets.Where(b => b.GetPrice() < priceBelow);
            }
            // ordering
            if (!string.IsNullOrEmpty(sort))
            {
                var desc = false;
                if (sort[0] == '-')
                    desc = true;
                if (desc)
                    sort = sort.Substring(1);
                switch (sort)
                {
                    case "owner":
                        baskets = desc ? baskets.OrderByDescending(b => b.Owner) : baskets.OrderBy(b => b.Owner);
                        break;
                    case "orders":
                        baskets = desc ? baskets.OrderByDescending(b => b.Orders.Count) : baskets.OrderBy(b => b.Orders.Count);
                        break;
                    case "amounts":
                        baskets = desc ? baskets.OrderByDescending(b => b.Orders.Sum(o => o.Amount)) : baskets.OrderBy(b => b.Orders.Sum(o => o.Amount));
                        break;
                    case "price":
                        baskets = desc ? baskets.OrderByDescending(b => b.GetPrice()) : baskets.OrderBy(b => b.GetPrice());
                        break;
                }
            }

            return baskets;
        }

        [HttpGet("{id}", Name = "GetBasket")]
        public IActionResult GetById(long id)
        {
            var basket = _storage.GetBasket(id);
            if (basket == null)
                return NotFound();
            return new ObjectResult(basket);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Basket basket)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (basket == null)
                return BadRequest();
            basket = _storage.CreateBasket(basket);
            return CreatedAtRoute("GetBasket", new { id = basket.Id }, basket);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Basket basket)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (basket?.Id != id)
                return BadRequest();
            if (!_storage.UpdateBasket(id, basket))
                return NotFound();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            if (!_storage.DeleteBasket(id))
                return NotFound();
            return new NoContentResult();
        }

        [HttpPut]
        [Route("{id}/clear")]
        public IActionResult Clear(long id)
        {
            if (!_storage.ClearBasket(id))
                return NotFound();
            return NoContent();
        }

        [HttpPut]
        [Route("{id}/deleteOrder/{itemId}")]
        public IActionResult DeleteOrderFromBasket(long id, long itemId)
        {
            var basket = _storage.GetBasket(id);
            if (basket == null)
                return NotFound();
            var order = basket.Orders.FirstOrDefault(o => o.Item.Id == itemId);
            if (order == null)
                return NotFound();
            basket.Orders.Remove(order);
            _storage.UpdateBasket(id, basket);
            return new ObjectResult(basket);
        }

        [HttpGet]
        [Route("{id}/price")]
        public IActionResult GetBasketTotalPrice(long id)
        {
            var basket = _storage.GetBasket(id);
            if (basket == null)
                return NotFound();
            return Ok(basket.GetPrice());
        }

        private static bool AreOrdersValid(IEnumerable<ItemOrder> orders, IApplicationStorage storage)
        {
            return orders.All(o => storage.ContainsLineItem(o.Item.Id) && storage.GetLineItem(o.Item.Id) == o.Item && o.Amount > 0);
        }
    }
}