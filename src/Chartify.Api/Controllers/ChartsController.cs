using Microsoft.AspNetCore.Mvc;
using Chartify.Application.Interfaces;
using Chartify.Domain.Entities;

namespace Chartify.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartsController : ControllerBase
    {
        private readonly IChartService _chartService;
        private readonly ILogger<ChartsController> _logger;
        public ChartsController(ILogger<ChartsController> logger, IChartService chartService)
        {
            _chartService = chartService;
            _logger = logger;
        }

        [HttpGet("global/top-100")]
        public async Task<ActionResult<Chart>> GetCharts()
        {
            _logger.LogInformation("Fetching global top 100 chart");

            try
            {
                var chart = await _chartService.GetGlobalTop100Async();
                return Ok(chart);

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occured while fetching the global top 100 chart");
                return StatusCode(500, "Internal server error");
            }

        }

    }
}
