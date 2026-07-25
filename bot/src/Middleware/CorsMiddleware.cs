using LundBot.Utils;

namespace LundBot.Middleware
{
    public sealed class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorsMiddleware> _logger;

        private static readonly HashSet<string> AllowedOrigins = new()
        {
            "https://lundbot.com",
            "https://infinitewarfarecommunity.com",
        };

        public CorsMiddleware(RequestDelegate next, ILogger<CorsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string origin = context.Request.Headers.Origin.ToString().ToLowerInvariant();

            if (IsOriginAllowed(origin))
            {
                context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
                context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
                context.Response.Headers.Append(
                    "Access-Control-Allow-Methods",
                    "GET, POST, PUT, DELETE, OPTIONS"
                );
                context.Response.Headers.Append(
                    "Access-Control-Allow-Headers",
                    "Content-Type, Authorization"
                );
            }

            if (context.Request.Method == HttpMethods.Options)
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            await _next(context).ConfigureAwait(false);
        }

        private static bool IsOriginAllowed(string origin)
        {
            if (AllowedOrigins.Contains(origin))
            {
                return true;
            }

            if (EnvironmentUtils.IsDevelopment())
            {
                return true;
            }

            return false;
        }
    }
}
