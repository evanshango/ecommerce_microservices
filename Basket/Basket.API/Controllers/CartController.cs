using System.Net;
using System.Threading.Tasks;
using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepo _repo;

        public CartController(ICartRepo repo)
        {
            _repo = repo;
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
            return Ok(await _repo.UpdateCart(cart));
        }

        [HttpDelete("{username}")]
        [ProducesResponseType(typeof(Cart), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string username)
        {
            await _repo.DeleteCart(username);
            return Ok();
        }
    }
}