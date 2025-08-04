using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs
{
    public class DynamicHomeSectionResponseDto
    {
        public string Id { get; set; }
        public string SectionType { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string TitleAr { get; set; }
        public string SubtitleAr { get; set; }
        public dynamic SectionConfig { get; set; } // Parsed JSON
        public dynamic Metadata { get; set; } // Parsed JSON
        public DateTime? ScheduledAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<string> TargetAudience { get; set; } = new();
        public int Priority { get; set; }
        public List<DynamicSectionContentResponseDto> Content { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Computed properties
        public bool IsVisible { get; set; }
        public bool IsExpired { get; set; }
        public bool IsScheduled { get; set; }
        public bool IsTimeSensitive { get; set; }
    }

    public class DynamicSectionContentResponseDto
    {
        public string Id { get; set; }
        public string SectionId { get; set; }
        public string ContentType { get; set; }
        public dynamic ContentData { get; set; } // Parsed JSON
        public dynamic Metadata { get; set; } // Parsed JSON
        public DateTime? ExpiresAt { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Computed properties
        public bool IsValid { get; set; }
        public bool IsExpired { get; set; }
    }

    public class CreateDynamicHomeSectionRequestDto
    {
        public string SectionType { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string TitleAr { get; set; }
        public string SubtitleAr { get; set; }
        public object SectionConfig { get; set; }
        public object Metadata { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<string> TargetAudience { get; set; } = new();
        public int Priority { get; set; }
        public List<CreateDynamicSectionContentRequestDto> Content { get; set; } = new();
    }

    public class CreateDynamicSectionContentRequestDto
    {
        public string ContentType { get; set; }
        public object ContentData { get; set; }
        public object Metadata { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class UpdateDynamicHomeSectionRequestDto
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string TitleAr { get; set; }
        public string SubtitleAr { get; set; }
        public object SectionConfig { get; set; }
        public object Metadata { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<string> TargetAudience { get; set; } = new();
        public int Priority { get; set; }
        public List<UpdateDynamicSectionContentRequestDto> Content { get; set; } = new();
    }

    public class UpdateDynamicSectionContentRequestDto
    {
        public string Id { get; set; } // null for new content
        public string ContentType { get; set; }
        public object ContentData { get; set; }
        public object Metadata { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsDeleted { get; set; } // to mark for deletion
    }
}