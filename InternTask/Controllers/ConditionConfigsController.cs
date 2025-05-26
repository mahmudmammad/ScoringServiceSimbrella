using InternTask.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConditionConfigsController : ControllerBase
{
    private readonly ScoringDbContext _context;

    public ConditionConfigsController(ScoringDbContext context)
    {
        _context = context;
    }

    [HttpPut("{type}/enable")]
    public async Task<IActionResult> SetConditionEnabled(string type, [FromQuery] bool enabled)
    {
        var config = await _context.ConditionConfigs
            .FirstOrDefaultAsync(c => c.Type == type);

        if (config == null)
        {
            return NotFound($"Condition with type '{type}' not found.");
        }

        config.Enabled = enabled;
        _context.ConditionConfigs.Update(config);
        await _context.SaveChangesAsync();

        return Ok(new { config.Type, config.Enabled });
    }
}
