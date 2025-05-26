using InternTask.Interfaces;
using InternTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using InternTask.DB;
using Microsoft.EntityFrameworkCore;

namespace InternTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoringController : ControllerBase
    {
        private readonly IScoringService _scoringService;
        private readonly ILogger<ScoringController> _logger;
   
        /// <summary>
        /// Evaluates a customer for loan eligibility.
        /// </summary>
        /// <param name="customer">Customer details including salary, age, account balance, etc.</param>
        /// <returns>Loan approval decision with detailed evaluation results.</returns>
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
        
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
        
       
    }
}