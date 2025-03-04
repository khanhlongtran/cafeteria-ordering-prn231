using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CafeteriaOrdering.API.Services;

namespace CafeteriaOrdering.API.Controllers
{
    [Route("api/v1/delivery/orders")]
    [ApiController]
    public class MealDeliveryController : ControllerBase
    {
        private readonly IMealDeliveryService _service;

        public MealDeliveryController(IMealDeliveryService service)
        {
            _service = service;
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
        public void Put(int id, [FromBody] string value)
        {

        }
    }
}