using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Queries.HomeSections;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.Generic;

namespace YemenBooking.Application.Handlers.MobileApp.HomeSections
{
    public class GetHomeConfigQueryHandler : IRequestHandler<GetHomeConfigQuery, DynamicHomeConfigDto>
    {
        private readonly IRepository<DynamicHomeConfig> _configRepository;

        public GetHomeConfigQueryHandler(IRepository<DynamicHomeConfig> configRepository)
        {
            _configRepository = configRepository;
        }

        public async Task<DynamicHomeConfigDto> Handle(GetHomeConfigQuery request, CancellationToken cancellationToken)
        {
            var query = _configRepository.GetQuery();

            DynamicHomeConfig config;

            if (!string.IsNullOrEmpty(request.Version))
            {
                config = await query.FirstOrDefaultAsync(c => c.Version == request.Version, cancellationToken);
            }
            else
            {
                // Get the active published config, or the latest if none is published
                config = await query
                    .Where(c => c.IsActive && c.PublishedAt.HasValue)
                    .OrderByDescending(c => c.PublishedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                if (config == null)
                {
                    config = await query
                        .OrderByDescending(c => c.CreatedAt)
                        .FirstOrDefaultAsync(cancellationToken);
                }
            }

            if (config == null)
            {
                return null;
            }

            return new DynamicHomeConfigDto
            {
                Id = config.Id.ToString(),
                Version = config.Version,
                IsActive = config.IsActive,
                CreatedAt = config.CreatedAt.ToString("O"),
                UpdatedAt = config.UpdatedAt.ToString("O"),
                PublishedAt = config.PublishedAt?.ToString("O"),
                GlobalSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(config.GlobalSettings),
                ThemeSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(config.ThemeSettings),
                LayoutSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(config.LayoutSettings),
                CacheSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(config.CacheSettings),
                AnalyticsSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(config.AnalyticsSettings),
                EnabledFeatures = JsonSerializer.Deserialize<List<string>>(config.EnabledFeatures),
                ExperimentalFeatures = JsonSerializer.Deserialize<Dictionary<string, object>>(config.ExperimentalFeatures)
            };
        }
    }
}