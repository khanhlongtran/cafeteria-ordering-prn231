using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CafeteriaOrdering.API.Models;
using CafeteriaOrdering.API.Constants;

namespace CafeteriaOrdering.API.Controllers
{
    [Route("api/v1/delivery/orders")]
    [ApiController]
    public class MealDeliveryController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _context;

        public MealDeliveryController(CafeteriaOrderingDBContext context)
        {
            _context = context;
        }

        // GET: api/v1/delivery/orders
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "meal1", "meal2" };
        }

        // GET: api/v1/delivery/orders/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult<string> Get(int id)
        {
            return "meal" + id;
        }

        // PUT: api/v1/delivery/orders/5
        [HttpPut("{id}")]
        public ActionResult<Order> Put(int id, [FromBody] string status)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            if (OrderConstants.isValidStatusForDeliver(status))
            {
                return BadRequest("Invalid status for deliverer");
            }

            if (status == OrderConstants.OrderStatus.REQUEST_DELIVERY.ToString())
            {
                order.Status = OrderConstants.OrderStatus.DELIVERY_ACCEPTED.ToString();
            }
            else if (status == OrderConstants.OrderStatus.DELIVERY_ACCEPTED.ToString())
            {
                order.Status = OrderConstants.OrderStatus.DELIVERY_IN_PROGRESS.ToString();
            }
            else if (status == OrderConstants.OrderStatus.DELIVERY_IN_PROGRESS.ToString())
            {
                order.Status = OrderConstants.OrderStatus.COMPLETED.ToString();
            }
            else
            {
                return BadRequest("Invalid status");
            }

            _context.SaveChanges();

            return Ok("Successful");
        }
    }
}