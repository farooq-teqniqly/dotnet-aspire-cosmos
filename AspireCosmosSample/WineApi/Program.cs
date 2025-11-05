using FluentValidation;
using WineApi.Middleware;

namespace WineApi;

public sealed class Program
{
    private Program() { }

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();

        var parsed = bool.TryParse(builder.Configuration["Cosmos:UseEmulator"], out var value);
        var useEmulator = parsed && value;

        builder.AddCosmos(useEmulator);
        builder.AddRepositories();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            if (useEmulator)
            {
                await app.CreateLocalCosmosDatabaseAsync().ConfigureAwait(false);
            }
        }

        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync();
    }
}
