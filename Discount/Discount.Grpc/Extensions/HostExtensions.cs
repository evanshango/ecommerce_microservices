using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            if (retry == null) return host;
            var retryForAvailability = retry.Value;

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var config = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<TContext>>();

            try
            {
                logger.LogInformation("Creating table...");
                using var conn = new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));
                conn.Open();

                using var command = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = "DROP TABLE IF EXISTS discount"
                };
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS discount (
                                Id SERIAL PRIMARY KEY, ProductName VARCHAR(24) NOT NULL, Description TEXT, Amount INT
                         )";
                command.ExecuteNonQuery();

                command.CommandText =
                    "INSERT INTO discount (ProductName, Description, Amount) VALUES ('Iphone X', 'Iphone Discount', 150);";
                command.ExecuteNonQuery();

                command.CommandText =
                    "INSERT INTO discount (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100);";
                command.ExecuteNonQuery();

                logger.LogInformation("Finished seeding some data...");
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "Error migrating some data to db");
                if (retryForAvailability < 50)
                {
                    retryForAvailability++;
                    Thread.Sleep(2000);
                    MigrateDatabase<TContext>(host, retryForAvailability);
                }
            }

            return host;
        }
    }
}