using MediatR;
using Microsoft.EntityFrameworkCore;
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
    public class GetDynamicHomeSectionsQueryHandler : IRequestHandler<GetDynamicHomeSectionsQuery, List<DynamicHomeSectionDto>>
    {
        private readonly IRepository<DynamicHomeSection> _sectionRepository;

        public GetDynamicHomeSectionsQueryHandler(IRepository<DynamicHomeSection> sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }

        public async Task<List<DynamicHomeSectionDto>> Handle(GetDynamicHomeSectionsQuery request, CancellationToken cancellationToken)
        {
            var query = _sectionRepository.GetQueryable();

            // Filter active sections if requested
            if (request.OnlyActive)
            {
                query = query.Where(s => s.IsActive && 
                                   (!s.ScheduledAt.HasValue || s.ScheduledAt <= System.DateTime.UtcNow) &&
                                   (!s.ExpiresAt.HasValue || s.ExpiresAt >= System.DateTime.UtcNow));
            }

            // Include content if requested
            if (request.IncludeContent)
            {
                query = query.Include(s => s.Content.Where(c => c.IsActive && 
                                                          (!c.ExpiresAt.HasValue || c.ExpiresAt >= System.DateTime.UtcNow)));
            }

            // Order sections
            query = query.OrderBy(s => s.Order).ThenByDescending(s => s.Priority);

            var sections = await query.ToListAsync(cancellationToken);

            var result = sections.Select(section => new DynamicHomeSectionDto
            {
                Id = section.Id.ToString(),
                SectionType = section.SectionType,
                Order = section.Order,
                IsActive = section.IsActive,
                Title = GetLocalizedTitle(section, request.Language),
                Subtitle = GetLocalizedSubtitle(section, request.Language),
                CreatedAt = section.CreatedAt,
                UpdatedAt = section.UpdatedAt,
                SectionConfig = JsonSerializer.Deserialize<Dictionary<string, object>>(section.SectionConfig),
                Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(section.Metadata),
                ScheduledAt = section.ScheduledAt?.ToString("O"),
                ExpiresAt = section.ExpiresAt?.ToString("O"),
                TargetAudience = DeserializeJsonArray(section.TargetAudience),
                Priority = section.Priority,
                Content = section.Content.Select(content => new DynamicSectionContentDto
                {
                    Id = content.Id.ToString(),
                    SectionId = section.Id.ToString(),
                    ContentType = content.ContentType,
                    Data = JsonSerializer.Deserialize<Dictionary<string, object>>(content.ContentData),
                    Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(content.Metadata),
                    ExpiresAt = content.ExpiresAt?.ToString("O"),
                    CreatedAt = content.CreatedAt.ToString("O"),
                    UpdatedAt = content.UpdatedAt.ToString("O")
                }).ToList()
            }).ToList();

            return result;
        }

        private string GetLocalizedTitle(DynamicHomeSection section, string language)
        {
            return language == "ar" && !string.IsNullOrEmpty(section.TitleAr) 
                ? section.TitleAr 
                : section.Title;
        }

        private string GetLocalizedSubtitle(DynamicHomeSection section, string language)
        {
            return language == "ar" && !string.IsNullOrEmpty(section.SubtitleAr) 
                ? section.SubtitleAr 
                : section.Subtitle;
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