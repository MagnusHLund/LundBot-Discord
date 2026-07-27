using LundBot.Config;
using LundBot.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace LundBot.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly DeveloperEnvironmentConfig _devConfig;

        public BaseController(IOptions<DeveloperEnvironmentConfig> devConfig)
        {
            _devConfig = devConfig.Value;
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
    }
}
