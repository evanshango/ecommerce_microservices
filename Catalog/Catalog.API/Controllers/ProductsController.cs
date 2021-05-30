using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepo _repo;
        private readonly ILogger _logger;

        public ProductsController(IProductRepo repo, ILogger<ProductsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repo.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _repo.GetProduct(id);
            if (product != null) return Ok(product);
            _logger.LogError($"Product with id: {id}, not found");
            return NotFound();
        }

        // [Route("[action]/{name}", Name = "ProductsByName")]
        [HttpGet("{name}", Name = "ProductsByName")]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> ProductsByName(string name)
        {
            var products = await _repo.GetProductsByName(name);
            return Ok(products);
        }


        // [Route("[action]/{category}", Name = "ProductsByCategory")]
        [HttpGet("category/{category}", Name = "ProductsByCategory")]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> ProductsByCategory(string category)
        {
            var products = await _repo.GetProductsByCategory(category);
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repo.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new {id = product.Id}, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product product)
        {
            if (await _repo.GetProduct(product.Id) == null) return NotFound();
            var response = await _repo.UpdateProduct(product);
            if (!response) return BadRequest("Something went wrong");
            return Ok(await _repo.GetProduct(product.Id));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (await _repo.GetProduct(id) == null) return NotFound();
            var response = await _repo.DeleteProduct(id);
            if (!response) return BadRequest("Something went wrong");
            return Ok("Product removed successfully");
        }
    }
}