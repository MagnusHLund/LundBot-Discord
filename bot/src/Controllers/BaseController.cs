using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace LundBot.Controllers
{
    public class BaseController : ControllerBase
    {
        private protected static string getRequestorIpAddress(HttpRequest request)
        {
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
