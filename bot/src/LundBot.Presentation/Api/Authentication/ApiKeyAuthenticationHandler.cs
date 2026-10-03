using System.Security.Claims;
using System.Text.Encodings.Web;
using LundBot.Presentation.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LundBot.Presentation.Api.Authentication
{
    public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ServerConfig _serverConfig;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IOptions<ServerConfig> serverConfig
        )
            : base(options, logger, encoder)
        {
            _serverConfig = serverConfig.Value;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            const string prefix = "Bearer ";

            if (!Request.Headers.TryGetValue("Authorization", out var authorization))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string value = authorization.ToString();

            if (!value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string apiKey = value[prefix.Length..].Trim();

            if (apiKey != _serverConfig.ApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
            }

            ClaimsIdentity? identity = new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, "ApiClient") },
                Scheme.Name
            );

            ClaimsPrincipal? principal = new ClaimsPrincipal(identity);

            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
        }
    }
}
