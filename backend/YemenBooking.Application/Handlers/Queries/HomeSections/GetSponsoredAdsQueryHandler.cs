using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Queries.HomeSections;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.MobileApp.HomeSections
{
    public class GetSponsoredAdsQueryHandler : IRequestHandler<GetSponsoredAdsQuery, List<SponsoredAdDto>>
    {
        private readonly IRepository<SponsoredAdSection> _adRepository;
        private readonly IRepository<Property> _propertyRepository;

        public GetSponsoredAdsQueryHandler(
            IRepository<SponsoredAdSection> adRepository,
            IRepository<Property> propertyRepository)
        {
            _adRepository = adRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<List<SponsoredAdDto>> Handle(GetSponsoredAdsQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var query = _adRepository.GetQuery();

            if (request.OnlyActive)
            {
                query = query.Where(a => a.IsActive && a.StartDate <= now && a.EndDate >= now);
            }

            var ads = await query.OrderBy(a => a.Priority).ToListAsync(cancellationToken);

            if (request.Limit.HasValue)
            {
                ads = ads.Take(request.Limit.Value).ToList();
            }

            // Prepare property details
            var allPropertyIds = new List<Guid>();
            foreach (var ad in ads)
                allPropertyIds.AddRange(DeserializeJsonArray(ad.PropertyIds)
                    .Select(id => Guid.TryParse(id, out var g) ? g : Guid.Empty)
                    .Where(g => g != Guid.Empty));
            allPropertyIds = allPropertyIds.Distinct().ToList();
            var propertyDict = new Dictionary<Guid, Property>();
            if (request.IncludePropertyDetails && allPropertyIds.Any())
            {
                var properties = await _propertyRepository.GetQuery().Include(p => p.Images)
                    .Where(p => allPropertyIds.Contains(p.Id)).ToListAsync(cancellationToken);
                propertyDict = properties.ToDictionary(p => p.Id);
            }

            var result = new List<SponsoredAdDto>();
            foreach (var ad in ads)
            {
                var ids = DeserializeJsonArray(ad.PropertyIds);
                var dto = new SponsoredAdDto
                {
                    Id = ad.Id.ToString(),
                    Title = ad.Title,
                    Subtitle = ad.Subtitle,
                    Description = ad.Description,
                    PropertyIds = ids,
                    Property = ids.Select(id => Guid.TryParse(id, out var g) && propertyDict.ContainsKey(g) ? propertyDict[g] : null)
                                 .FirstOrDefault()?.ToSummaryDto(),
                    CustomImageUrl = ad.CustomImageUrl,
                    BackgroundColor = ad.BackgroundColor,
                    TextColor = ad.TextColor,
                    Styling = JsonSerializer.Deserialize<Dictionary<string, object>>(ad.Styling),
                    CtaText = ad.CtaText,
                    CtaAction = ad.CtaAction,
                    CtaData = JsonSerializer.Deserialize<Dictionary<string, object>>(ad.CtaData),
                    StartDate = ad.StartDate,
                    EndDate = ad.EndDate,
                    Priority = ad.Priority,
                    TargetingData = JsonSerializer.Deserialize<Dictionary<string, object>>(ad.TargetingData),
                    AnalyticsData = JsonSerializer.Deserialize<Dictionary<string, object>>(ad.AnalyticsData),
                    IsActive = ad.IsActive,
                    CreatedAt = ad.CreatedAt.ToString("O"),
                    UpdatedAt = ad.UpdatedAt.ToString("O"),
                    ImpressionCount = ad.ImpressionCount,
                    ClickCount = ad.ClickCount,
                    ConversionRate = ad.ConversionRate
                };
                result.Add(dto);
            }
            return result;
        }

        private List<string> DeserializeJsonArray(string json)
        {
            try
            {
                return string.IsNullOrEmpty(json)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}