using LundBot.Config;
using LundBot.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace LundBot.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly DeveloperEnvironmentConfig _devConfig;
        private readonly ServerConfig _serverConfig;

        public BaseController(
            IOptions<DeveloperEnvironmentConfig> devConfig,
            IOptions<ServerConfig> serverConfig
        )
        {
            _devConfig = devConfig.Value;
            _serverConfig = serverConfig.Value;
        }

        private protected string GetRequestorIpAddress(HttpRequest request)
        {
            if (EnvironmentUtils.IsDevelopment() && _devConfig.GenerateIpAddresses)
            {
                Random random = new Random();
                return $"{random.Next(1, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(1, 255)}";
            }

            if (
                request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwarded)
                && !StringValues.IsNullOrEmpty(forwarded)
            )
            {
                var first = forwarded.ToString().Split(',').Select(s => s.Trim()).FirstOrDefault();
                if (!string.IsNullOrEmpty(first))
                    return first;
            }

            var remoteIp = request.HttpContext.Connection.RemoteIpAddress;
            if (remoteIp != null)
            {
                return remoteIp.ToString();
            }

            return "0.0.0.0";
        }

        private protected bool HasApiKey()
        {
            string? authorization = Request.Headers.Authorization;

            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return false;
            }

            string apiKey = authorization.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            return apiKey == _serverConfig.ApiKey;
        }
    }
}
