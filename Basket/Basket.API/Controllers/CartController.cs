using System.Net;
using System.Threading.Tasks;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/basket")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepo _repo;
        private readonly DiscountGrpcService _discountGrpcService;

        public CartController(ICartRepo repo, DiscountGrpcService discountGrpcService)
        {
            _repo = repo;
            _discountGrpcService = discountGrpcService;
        }

        [HttpGet("{username}", Name = "GetCart")]
        [ProducesResponseType(typeof(Cart), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Cart>> GetCart(string username)
        {
            var cart = await _repo.GetCart(username);
            return Ok(cart ?? new Cart(username));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Cart), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Cart>> UpdateCart([FromBody] Cart cart)
        {
            // Communicate with discount gRPC and calculate latest prices of products
            foreach (var item in cart.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            return Ok(await _repo.UpdateCart(cart));
        }

        [HttpDelete("{username}", Name = "DeleteCart")]
        [ProducesResponseType(typeof(Cart), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string username)
        {
            await _repo.DeleteCart(username);
            return Ok();
        }
    }
}