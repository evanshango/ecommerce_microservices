using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Basket.API.Repositories
{
    public class CartRepo : ICartRepo
    {
        private readonly IDatabase _database;

        public CartRepo(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<Cart> GetCart(string username)
        {
            var cart = await _database.StringGetAsync(username);
            return string.IsNullOrEmpty(cart) ? null : JsonSerializer.Deserialize<Cart>(cart);
        }

        public async Task<Cart> UpdateCart(Cart cart)
        {
            var created = await _database.StringSetAsync(
                cart.Username, JsonSerializer.Serialize(cart), TimeSpan.FromDays(15)
            );
            if (!created) return null;
            return await GetCart(cart.Username);
        }

        public async Task<bool> DeleteCart(string username)
        {
            return await _database.KeyDeleteAsync(username);
        }
    }
}