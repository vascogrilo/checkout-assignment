using System.Collections.Generic;
using System.Linq;
using CheckoutAssignment.Models;
using CheckoutAssignment.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutAssignment.Controllers
{
    [Route("api/v1/items")]
    public class ItemsController : Controller
    {
        private readonly IApplicationStorage _storage;

        public ItemsController(IApplicationStorage storage)
        {
            _storage = storage;
        }

        [HttpGet]
        public IEnumerable<Item> GetAll(string text, string name, float price, float priceAbove, float priceBelow, string sort)
        {
            var items = _storage.GetLineItems();
            // exact name has precedence
            if (!string.IsNullOrEmpty(name))
                items = items.Where(i => i.Name == name);
            else if (!string.IsNullOrEmpty(text))
                items = items.Where(i => i.Name.Contains(text));
            // exact price has precedence
            if (price > 0)
                items = items.Where(i => i.Price.Equals(price));
            else
            {
                if (priceAbove > 0)
                    items = items.Where(i => i.Price > priceAbove);
                if (priceBelow > 0)
                    items = items.Where(i => i.Price < priceBelow);
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
                    case "name":
                        items = desc ? items.OrderByDescending(i => i.Name) : items.OrderBy(i => i.Name);
                        break;
                    case "price":
                        items = desc ? items.OrderByDescending(i => i.Price) : items.OrderBy(i => i.Price);
                        break;
                }
            }
            return items;
        }

        [HttpGet("{id}", Name = "GetLineItem")]
        public IActionResult GetById(long id)
        {
            var item = _storage.GetLineItem(id);
            if (item == null)
                return NotFound();
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Item item)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            item = _storage.CreateLineItem(item);
            return CreatedAtRoute("GetLineItem", new {id = item.Id}, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Item item)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (item == null || item.Id != id)
                return BadRequest();
            if (!_storage.UpdateLineItem(id, item))
                return NotFound();
            return new NoContentResult();
        }
        

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            if (!_storage.DeleteLineItem(id))
                return NotFound();
            return new NoContentResult();
        }

        [HttpPut]
        [Route("{id}/order/{basketId}/{amount}")]
        public IActionResult OrderToBasket(long id, long basketId, uint amount)
        {
            var item = _storage.GetLineItem(id);
            if (item == null)
                return NotFound();
            var basket = _storage.GetBasket(basketId);
            if (basket == null)
                return NotFound();
            if (amount == 0)
                return BadRequest();

            var order = new ItemOrder { Item = item, Amount = amount };
            var existingOrder = basket.Orders.FirstOrDefault(o => o.Item.Id == id);
            if (existingOrder != null)
                existingOrder.Amount += amount;
            else basket.Orders.Add(order);

            if (!_storage.UpdateBasket(basketId, basket))
                return new ForbidResult();

            return NoContent();
        }
    }
}