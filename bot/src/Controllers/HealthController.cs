using Microsoft.AspNetCore.Mvc;

namespace LundBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class HealthController : BaseController
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Running");
        }
    }
}
