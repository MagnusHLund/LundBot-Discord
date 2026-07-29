using LundBot.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class HealthController : BaseController
    {
        private readonly ServerConfig _serverConfig;

        public HealthController(
            IOptions<DeveloperEnvironmentConfig> devConfig,
            IOptions<ServerConfig> serverConfig
        )
            : base(devConfig)
        {
            _serverConfig = serverConfig.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string version = _serverConfig.Version;

            return Ok(new { status = "Healthy", version });
        }
    }
}
