using Microsoft.Extensions.Primitives;

namespace LundBot.Presentation.Api.Middleware
{
    public sealed class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _hostEnvironment;

        private static readonly HashSet<string> AllowedOrigins = new()
        {
            "https://lundbot69.com",
            "https://discord.lundbot69.com",
            "https://infinitewarfarecommunity.com",
        };

        public CorsMiddleware(RequestDelegate next, IHostEnvironment hostEnvironment)
        {
            _next = next;
            _hostEnvironment = hostEnvironment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string origin = context.Request.Headers.Origin.ToString().ToLowerInvariant();

            if (IsOriginAllowed(origin))
            {
                context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
                context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
                context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
            }

            if (context.Request.Method == HttpMethods.Options)
            {
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            await _next(context).ConfigureAwait(false);
        }

        private bool IsOriginAllowed(string origin)
        {
            if (AllowedOrigins.Contains(origin))
            {
                return true;
            }

            if (_hostEnvironment.IsDevelopment())
            {
                return true;
            }

            return false;
        }
    }
}
