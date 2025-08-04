using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemenBooking.Core.Entities
{
    public class DynamicHomeSection : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string SectionType { get; private set; } // SINGLE_PROPERTY_AD, FEATURED_PROPERTY_AD, etc.
        
        public int Order { get; private set; }
        
        public bool IsActive { get; private set; }
        
        [StringLength(200)]
        public string Title { get; private set; }
        
        [StringLength(500)]
        public string Subtitle { get; private set; }
        
        [StringLength(200)]
        public string TitleAr { get; private set; }
        
        [StringLength(500)]
        public string SubtitleAr { get; private set; }
        
        [Column(TypeName = "json")]
        public string SectionConfig { get; private set; } // JSON configuration
        
        [Column(TypeName = "json")]
        public string Metadata { get; private set; } // Additional metadata
        
        public DateTime? ScheduledAt { get; private set; }
        
        public DateTime? ExpiresAt { get; private set; }
        
        [Column(TypeName = "json")]
        public string TargetAudience { get; private set; } // JSON array of audience types
        
        public int Priority { get; private set; }
        
        // Navigation properties
        private readonly List<DynamicSectionContent> _content = new();
        public IReadOnlyCollection<DynamicSectionContent> Content => _content.AsReadOnly();

        protected DynamicHomeSection() { }

        public DynamicHomeSection(
            string sectionType,
            int order,
            string title = null,
            string titleAr = null,
            string sectionConfig = "{}",
            string metadata = "{}",
            string targetAudience = "[]",
            int priority = 0)
        {
            Id = Guid.NewGuid();
            SectionType = sectionType;
            Order = order;
            IsActive = true;
            Title = title;
            TitleAr = titleAr;
            SectionConfig = sectionConfig;
            Metadata = metadata;
            TargetAudience = targetAudience;
            Priority = priority;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSection(
            string title,
            string subtitle,
            string titleAr,
            string subtitleAr,
            string sectionConfig,
            string metadata,
            DateTime? scheduledAt,
            DateTime? expiresAt,
            string targetAudience,
            int priority)
        {
            Title = title;
            Subtitle = subtitle;
            TitleAr = titleAr;
            SubtitleAr = subtitleAr;
            SectionConfig = sectionConfig;
            Metadata = metadata;
            ScheduledAt = scheduledAt;
            ExpiresAt = expiresAt;
            TargetAudience = targetAudience;
            Priority = priority;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOrder(int newOrder)
        {
            Order = newOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleActiveStatus()
        {
            IsActive = !IsActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetActiveStatus(bool isActive)
        {
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddContent(DynamicSectionContent content)
        {
            _content.Add(content);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveContent(Guid contentId)
        {
            _content.RemoveAll(c => c.Id == contentId);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearContent()
        {
            _content.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        // Helper methods
        public bool IsVisible => IsActive && !IsExpired && !IsScheduled;
        
        public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
        
        public bool IsScheduled => ScheduledAt.HasValue && DateTime.UtcNow < ScheduledAt.Value;
        
        public bool IsTimeSensitive => 
            SectionType.Contains("LIMITED_TIME") || 
            SectionType.Contains("FLASH_DEALS") || 
            SectionType.Contains("SEASONAL");
    }
}