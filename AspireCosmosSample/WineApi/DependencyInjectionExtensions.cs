using Microsoft.Azure.Cosmos;
using WineApi.Repositories;

namespace WineApi
{
    internal static class DependencyInjectionExtensions
    {
        internal static WebApplicationBuilder AddCosmos(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton(sp =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();

                var connectionString =
                    cfg.GetConnectionString("Cosmos")
                    ?? throw new InvalidOperationException(
                        "Cosmos connection string setting not configured"
                    );

                return new CosmosClient(
                    connectionString,
                    new CosmosClientOptions
                    {
                        SerializerOptions = new CosmosSerializationOptions
                        {
                            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                        },
                    }
                );
            });

            builder.Services.AddKeyedSingleton<Container>(
                "Wineries",
                (sp, _) =>
                {
                    var client = sp.GetRequiredService<CosmosClient>();
                    return client.GetContainer("envino", "Wineries");
                }
            );

            return builder;
        }

        internal static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IWineryRepository, WineryRepository>();

            return builder;
        }
    }
}
