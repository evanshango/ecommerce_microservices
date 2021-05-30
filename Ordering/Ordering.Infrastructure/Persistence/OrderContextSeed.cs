using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext context, ILogger<OrderContextSeed> logger)
        {
            if (!context.Orders.Any())
            {
                context.Orders.AddRange(GetPreconfiguredOrders());
                await context.SaveChangesAsync();
                logger.LogInformation($"seeding data associated with context {nameof(OrderContext)}...");
            }
            else
            {
                logger.LogInformation("Orders table already has some data..");
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new()
                {
                    Username = "evans", Firstname = "Evans", Lastname = "Shango", AddressLine = "Unknown Street 1234",
                    EmailAddress = "evansonshango@gmail.com", Country = "Kenya", TotalPrice = 350
                }
            };
        }
    }
}