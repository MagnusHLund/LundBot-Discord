using LundBot.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class HealthController : BaseController
    {
        public HealthController(IOptions<DeveloperEnvironmentConfig> devConfig)
            : base(devConfig) { }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Running");
        }
    }
}
