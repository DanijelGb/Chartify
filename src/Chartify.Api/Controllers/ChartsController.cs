using Microsoft.AspNetCore.Mvc;

namespace Chartify.Api.ChartController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartController : ControllerBase
    {

        private readonly ILogger<ChartController> _logger;
        public ChartController(ILogger<ChartController> logger)
        {
            _logger = logger;
        }

        [HttpGet("global/top-100")]
        public async Task<ActionResult<string>> GetCharts()
        {
            _logger.LogInformation("Fetching global top 100 chart");
            var result = await _chartService.GetGlobalTop100Async();

            return Ok("Top 100");
        }

    }
}
