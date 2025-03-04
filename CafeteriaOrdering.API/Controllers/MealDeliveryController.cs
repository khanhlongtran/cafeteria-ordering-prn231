using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CafeteriaOrdering.API.Controllers
{
    [Route("api/v1/delivery/orders")]
    [ApiController]
    public class MealDeliveryController : ControllerBase
    {
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