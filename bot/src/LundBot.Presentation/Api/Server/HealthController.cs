using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Presentation.Api.Server
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class HealthController : AbstractBaseController
    {
        private readonly ServerConfig _serverConfig;

        public HealthController(IOptions<DeveloperEnvironmentConfig> devConfig, IOptions<ServerConfig> serverConfig)
            : base(devConfig, serverConfig)
        {
            _serverConfig = serverConfig.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            if (!HasApiKey())
            {
                return Unauthorized();
            }

            string version = _serverConfig.Version;

            return Ok(new { status = "Healthy", version });
        }
    }
}
