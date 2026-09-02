using LundBot.Application;
using LundBot.Infrastructure;
using LundBot.Infrastructure.Persistence;

namespace LundBot.Presentation
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.AddLogger();

            builder.Services.AddApplication();
            builder.Services.AddPresentation(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);

            WebApplication app = builder.Build();

            await app.ApplyDatabaseMigrationsAsync();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.MapControllers();
            app.AddMiddleware();

            app.Run();
        }
    }
}
