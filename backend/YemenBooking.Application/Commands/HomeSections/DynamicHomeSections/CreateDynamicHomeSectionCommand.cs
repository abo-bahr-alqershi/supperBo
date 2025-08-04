using MediatR;
using System;
using System.Collections.Generic;

namespace YemenBooking.Application.Commands.HomeSections.DynamicHomeSections
{
    public class CreateDynamicHomeSectionCommand : IRequest<Guid>
    {
        public string SectionType { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string TitleAr { get; set; }
        public string SubtitleAr { get; set; }
        public string SectionConfig { get; set; }
        public string Metadata { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<string> TargetAudience { get; set; } = new();
        public int Priority { get; set; }
        public List<CreateDynamicSectionContentDto> Content { get; set; } = new();
    }

    public class CreateDynamicSectionContentDto
    {
        public string ContentType { get; set; }
        public string ContentData { get; set; }
        public string Metadata { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}