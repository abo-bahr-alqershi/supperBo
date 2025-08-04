using MediatR;
using System;
using System.Collections.Generic;

namespace YemenBooking.Application.Commands.HomeSections.DynamicHomeSections
{
    public class UpdateDynamicHomeSectionCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
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
        public List<UpdateDynamicSectionContentDto> Content { get; set; } = new();
    }

    public class UpdateDynamicSectionContentDto
    {
        public Guid? Id { get; set; } // null for new content
        public string ContentType { get; set; }
        public string ContentData { get; set; }
        public string Metadata { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsDeleted { get; set; } // to mark for deletion
    }
}