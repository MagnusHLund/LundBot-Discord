using DSharpPlus;
using LundBot.Config;
using LundBot.Data;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Middleware;
using LundBot.Repositories;
using LundBot.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LundBot
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            RegisterLogger();
            var builder = WebApplication.CreateBuilder(args);

            SetupDatabase(builder);
            RegisterConfiguration(builder);
            RegisterServices(builder.Services);
            RegisterRepositories(builder.Services);
            RegisterBackgroundServices(builder.Services);

            RegisterBotConfiguration(builder);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            RegisterMiddleware(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.UseCors();

            app.Run();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<ICommandsService, CommandsService>();
            services.AddSingleton<ILeaderboardService, LeaderboardService>();
            services.AddSingleton<IMessageService, MessageService>();

            services.AddScoped<IWebsiteTrafficService, WebsiteTrafficService>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IWebsiteTrafficRepository, WebsiteTrafficRepository>();
            services.AddScoped<ILeaderboardsRepository, LeaderboardsRepository>();
            services.AddScoped<IUpvotingLeaderboardRepository, UpvotingLeaderboardRepository>();
            services.AddScoped<ILeaderboardScoresRepository, LeaderboardScoresRepository>();
            services.AddScoped<ILeaderboardMessagesRepository, LeaderboardMessagesRepository>();
            services.AddScoped<
                IWebsiteTrafficMessagesRepository,
                WebsiteTrafficMessagesRepository
            >();
        }

        private static void RegisterBackgroundServices(IServiceCollection services)
        {
            services.AddHostedService<BackgroundServices.DiscordBotBackgroundService>();
        }

        private static void RegisterMiddleware(WebApplication app)
        {
            app.UseMiddleware<CorsMiddleware>();
        }

        private static void RegisterBotConfiguration(WebApplicationBuilder builder)
        {
            string discordToken = builder.Configuration["Discord:Token"] ?? "";

            builder.Services.AddSingleton(provider =>
            {
                var config = new DiscordConfiguration
                {
                    Token = discordToken,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged,
                    LoggerFactory = new LoggerFactory().AddSerilog(),
                    AutoReconnect = true,
                    ReconnectIndefinitely = true,
                };

                return new DiscordClient(config);
            });
        }

        private static void RegisterLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(
                    new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile("appsettings.Production.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build()
                )
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        private static void RegisterConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.Configure<DiscordConfig>(builder.Configuration.GetSection("Discord"));
        }

        private static void SetupDatabase(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetSection("Database")["ConnectionString"];

            builder.Services.AddDbContext<LundBotDiscordDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
        }
    }
}
