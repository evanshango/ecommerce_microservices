using System.Threading.Tasks;
using Discount.API.Entities;

namespace Discount.API.Repositories
{
    public interface IDiscountRepo
    {
        Task<Coupon> GetDiscount(string productName);
        Task<Coupon> CreateDiscount(Coupon coupon);
        Task<bool> UpdateDiscount(Coupon coupon);
        Task<bool> DeleteDiscount(string productName);
    }
}