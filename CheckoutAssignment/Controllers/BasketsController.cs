﻿using System.Collections.Generic;
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
        public IEnumerable<Basket> GetAll()
        {
            return _storage.GetBaskets();
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