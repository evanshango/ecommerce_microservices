using System.Threading.Tasks;
using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepo : IDiscountRepo
    {
        private readonly NpgsqlConnection _conn;

        public DiscountRepo(IConfiguration config)
        {
            _conn = new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var coupon = await _conn.QueryFirstOrDefaultAsync<Coupon>(
                "SELECT * FROM discount WHERE ProductName = @ProductName", new {ProductName = productName}
            );

            if (coupon == null)
                return new Coupon
                {
                    ProductName = "No Discount", Amount = 0, Description = "No Discount Description"
                };
            return coupon;
        }

        public async Task<Coupon> CreateDiscount(Coupon coupon)
        {
            var affected = await _conn.ExecuteAsync(
                "INSERT INTO discount (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                new {coupon.ProductName, coupon.Description, coupon.Amount}
            );
            if (affected != 0) return await GetDiscount(coupon.ProductName);
            return new Coupon
            {
                ProductName = "No Discount", Amount = 0, Description = "No Discount Description"
            };
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var affected = await _conn.ExecuteAsync(
                "UPDATE discount SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id=@Id",
                new {coupon.ProductName, coupon.Description, coupon.Amount, coupon.Id}
            );
            return affected != 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var affected = await _conn.ExecuteAsync(
                "DELETE FROM discount WHERE ProductName=@ProductName", new {ProductName = productName}
            );
            return affected != 0;
        }
    }
}