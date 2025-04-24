using InternTask.Interfaces;
using InternTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace InternTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoringController : ControllerBase
    {
        private readonly IScoringService _scoringService;
        private readonly ILogger<ScoringController> _logger;

        public ScoringController(IScoringService scoringService, ILogger<ScoringController> logger)
        {
            _scoringService = scoringService;
            _logger = logger;
        }

        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateCustomer([FromBody] Customer customer)
        {
            _logger.LogInformation("Received customer evaluation request.");
            var result = await _scoringService.EvaluateCustomerAsync(customer);
            return Ok(result);
        }
    }
}