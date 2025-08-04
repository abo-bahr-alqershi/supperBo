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

            var result = new List<SponsoredAdDto>();
            var allPropertyIds = new List<Guid>();

            if (request.IncludePropertyDetails)
            {
                foreach (var ad in ads)
                {
                    var pids = DeserializeJsonArray(ad.PropertyIds)
                        .Select(id => Guid.TryParse(id, out var g) ? (Guid?)g : null)
                        .Where(g => g.HasValue)
                        .Select(g => g.Value);
                    allPropertyIds.AddRange(pids);
                }
                allPropertyIds = allPropertyIds.Distinct().ToList();
            }

            var propertyDict = new Dictionary<Guid, Property>();
            if (request.IncludePropertyDetails && allPropertyIds.Any())
            {
                var properties = await _propertyRepository.GetQuery()
                    .Include(p => p.Images)
                    .Where(p => allPropertyIds.Contains(p.Id))
                    .ToListAsync(cancellationToken);
                propertyDict = properties.ToDictionary(p => p.Id);
            }

            foreach (var ad in ads)
            {
                var dto = new SponsoredAdDto
                {
                    Id = ad.Id.ToString(),
                    Title = ad.Title,
                    Subtitle = ad.Subtitle,
                    Description = ad.Description,
                    PropertyIds = DeserializeJsonArray(ad.PropertyIds),
                    CustomImageUrl = ad.CustomImageUrl,
                    BackgroundColor = ad.BackgroundColor,
                    TextColor = ad.TextColor,
                    Styling = ad.Styling,
                    CtaText = ad.CtaText,
                    CtaAction = ad.CtaAction,
                    CtaData = ad.CtaData,
                    StartDate = ad.StartDate,
                    EndDate = ad.EndDate,
                    Priority = ad.Priority,
                    TargetingData = ad.TargetingData,
                    AnalyticsData = ad.AnalyticsData,
                    IsCurrentlyActive = ad.IsCurrentlyActive,
                    ImpressionCount = ad.ImpressionCount,
                    ClickCount = ad.ClickCount,
                    ConversionRate = ad.ConversionRate
                };

                if (request.IncludePropertyDetails)
                {
                    dto.Properties = DeserializeJsonArray(ad.PropertyIds)
                        .Select(id => Guid.TryParse(id, out var g) ? (Guid?)g : null)
                        .Where(g => g.HasValue && propertyDict.ContainsKey(g.Value))
                        .Select(g => propertyDict[g.Value])
                        .Select(p =>
                        {
                            var mainImage = p.Images.FirstOrDefault(i => i.IsMain) ?? p.Images.OrderBy(i => i.SortOrder).FirstOrDefault();
                            return new PropertySummaryDto
                            {
                                Id = p.Id.ToString(),
                                Name = p.Name,
                                MainImageUrl = mainImage?.Url,
                                BasePrice = p.BasePricePerNight,
                                Currency = p.Currency,
                                AverageRating = (double)p.AverageRating
                            };
                        })
                        .ToList();
                }

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