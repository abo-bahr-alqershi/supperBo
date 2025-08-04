using MediatR;
using System.Collections.Generic;

namespace YemenBooking.Application.Queries.HomeSections
{
    public class GetDynamicHomeSectionsQuery : IRequest<List<DynamicHomeSectionDto>>
    {
        public string Language { get; set; } = "en"; // "en" or "ar"
        public List<string> TargetAudience { get; set; } = new();
        public bool IncludeContent { get; set; } = true;
        public bool OnlyActive { get; set; } = true;
    }

    public class DynamicHomeSectionDto
    {
        public string Id { get; set; }
        public string SectionType { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string SectionConfig { get; set; }
        public string Metadata { get; set; }
        public string ScheduledAt { get; set; }
        public string ExpiresAt { get; set; }
        public List<string> TargetAudience { get; set; }
        public int Priority { get; set; }
        public List<DynamicSectionContentDto> Content { get; set; } = new();
    }

    public class DynamicSectionContentDto
    {
        public string Id { get; set; }
        public string ContentType { get; set; }
        public string ContentData { get; set; }
        public string Metadata { get; set; }
        public string ExpiresAt { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}