using Microsoft.Azure.Cosmos;
using WineApi.Repositories;

namespace WineApi
{
    internal static class DependencyInjectionExtensions
    {
        internal static WebApplicationBuilder AddCosmos(
            this WebApplicationBuilder builder,
            bool useEmulator
        )
        {
            builder.Services.AddSingleton(sp =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();

                var connectionString =
                    cfg.GetConnectionString("Cosmos")
                    ?? throw new InvalidOperationException(
                        "Cosmos connection string setting not configured"
                    );

                var cosmosClientOptions = new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                    },
                };

                if (!useEmulator)
                {
                    return new CosmosClient(connectionString, cosmosClientOptions);
                }

                var insecureHandler = new HttpClientHandler
                {
#pragma warning disable S4830
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
#pragma warning restore S4830
                };

                cosmosClientOptions.HttpClientFactory = () => new HttpClient(insecureHandler);
                cosmosClientOptions.LimitToEndpoint = true;
                cosmosClientOptions.ConnectionMode = ConnectionMode.Gateway;

                return new CosmosClient(connectionString, cosmosClientOptions);
            });

            builder.Services.AddKeyedSingleton<Container>(
                CosmosDbConstants.Containers.Wineries,
                (sp, _) =>
                {
                    var client = sp.GetRequiredService<CosmosClient>();
                    return client.GetContainer(
                        CosmosDbConstants.Database,
                        CosmosDbConstants.Containers.Wineries
                    );
                }
            );

            return builder;
        }

        internal static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IWineryRepository, WineryRepository>();

            return builder;
        }

        internal static async Task CreateLocalCosmosDatabaseAsync(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            using (var scope = app.Services.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<CosmosClient>();

                var database = await client
                    .CreateDatabaseIfNotExistsAsync("envino", throughput: 400)
                    .ConfigureAwait(false);

                app.Logger.LogInformation("Created Cosmos database");

                await CreateContainer(database, CosmosDbConstants.Containers.Wineries, "/id", app)
                    .ConfigureAwait(false);
            }
        }

        private static async Task CreateContainer(
            Database database,
            string id,
            string partitionKeyPath,
            WebApplication app
        )
        {
            await database
                .CreateContainerIfNotExistsAsync(id, partitionKeyPath)
                .ConfigureAwait(false);

            app.Logger.LogInformation("Created container {ContainerId}", id);
        }
    }
}
