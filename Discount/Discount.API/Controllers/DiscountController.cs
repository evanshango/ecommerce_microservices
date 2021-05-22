using System.Net;
using System.Threading.Tasks;
using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController: ControllerBase
    {
        private readonly IDiscountRepo _repo;

        public DiscountController(IDiscountRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("{productName}", Name = "GetDiscount")]
        [ProducesResponseType(typeof(Coupon), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {
            var coupon = await _repo.GetDiscount(productName);
            return Ok(coupon);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Coupon), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
        {
            var cp = await _repo.CreateDiscount(coupon);
            return Ok(cp);
            // return CreatedAtRoute("GetDiscount", new {productName = coupon.ProductName}, coupon);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Coupon), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
        {
            var result = await _repo.UpdateDiscount(coupon);
            if (!result) return BadRequest("Something went wrong");
            return Ok(await _repo.GetDiscount(coupon.ProductName));
        }

        [HttpDelete("{productName}", Name = "DeleteDiscount")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteDiscount(string productName)
        {
            var result = await _repo.DeleteDiscount(productName);
            if (!result) return BadRequest("An error occurred. Please try again later");
            return Ok("Coupon deleted successfully");
        }
    }
}