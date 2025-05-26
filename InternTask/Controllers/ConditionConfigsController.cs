// Controllers/ConditionConfigsController.cs
using InternTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConditionConfigsController : ControllerBase
{
    private readonly ConditionConfigService _configService;

    public ConditionConfigsController(ConditionConfigService configService)
    {
        _configService = configService;
    }

    [HttpPut("{type}/enable")]
    public async Task<IActionResult> SetConditionEnabled(string type, [FromQuery] bool enabled)
    {
        var updatedConfig = await _configService.SetConditionEnabledAsync(type, enabled);

        if (updatedConfig == null)
            return NotFound($"Condition with type '{type}' not found.");

        return Ok(new { updatedConfig.Type, updatedConfig.Enabled });
    }
}