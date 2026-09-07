using LundBot.Presentation.Api.Common;
using LundBot.Presentation.Config;
using Microsoft.AspNetCore.Authorization;
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
            : base(devConfig)
        {
            _serverConfig = serverConfig.Value;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            string version = _serverConfig.Version;

            return Ok(new { status = "Healthy", version });
        }
    }
}
