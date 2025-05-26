// Services/ConditionConfigService.cs
using InternTask.DB;
using InternTask.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using InternTask.Entities;
using InternTask.Models.Configuration;

namespace InternTask.Services
{
    public class ConditionConfigService
    {
        private readonly ScoringDbContext _context;

        public ConditionConfigService(ScoringDbContext context)
        {
            _context = context;
        }

        public async Task<ConditionConfigEntity?> SetConditionEnabledAsync(string type, bool enabled)
        {
            var config = await _context.ConditionConfigs
                .FirstOrDefaultAsync(c => c.Type == type);

            if (config == null) return null;

            config.Enabled = enabled;
            _context.ConditionConfigs.Update(config);
            await _context.SaveChangesAsync();

            return config;
        }
    }
}