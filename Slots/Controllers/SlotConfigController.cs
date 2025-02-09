using Microsoft.AspNetCore.Mvc;
using boxs.Models;
using boxs.Services;

namespace boxs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class boxConfigController : ControllerBase
    {
        private readonly ILogger<boxConfigController> _logger;
        private readonly IMachineConfiguration _configuration;

        public boxConfigController(ILogger<boxConfigController> logger, IMachineConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPut(Name = "EditboxConfig")]
        public async Task<IActionResult> EditboxConfig([FromBody] boxMachineDTO NewParameter)
        {
            if (NewParameter.maxX <= 0 || NewParameter.maxY <= 0) {
                _logger.LogError("Parameter validation failed.");
                return BadRequest("Parameter validation failed.");
            }
            if (! await _configuration.SetConfigAsync(NewParameter)) {
                _logger.LogError("Parameter update failed");
                return Problem("Parameter update failed.");
            }
            return Ok("Parameter update successful.");
        }
    }
}
