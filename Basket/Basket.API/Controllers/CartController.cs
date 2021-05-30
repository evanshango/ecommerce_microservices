using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/basket")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepo _repo;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public CartController(ICartRepo repo, DiscountGrpcService discountGrpcService, IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _repo = repo;
            _discountGrpcService = discountGrpcService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
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

        [HttpPost("checkout")]
        [ProducesResponseType((int) HttpStatusCode.Accepted)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] CartCheckout cartCheckout)
        {
            var basket = await _repo.GetCart(cartCheckout.Username);
            if (basket == null) return BadRequest();

            var eventMsg = _mapper.Map<BasketCheckoutEvent>(cartCheckout);
            eventMsg.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMsg);

            await _repo.DeleteCart(basket.Username);
            return Accepted();
        }
    }
}