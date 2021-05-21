using System.Threading.Tasks;
using Basket.API.Entities;

namespace Basket.API.Repositories
{
    public interface ICartRepo
    {
        Task<Cart> GetCart(string username);
        Task<Cart> UpdateCart(Cart cart);
        Task<bool> DeleteCart(string username);
    }
}