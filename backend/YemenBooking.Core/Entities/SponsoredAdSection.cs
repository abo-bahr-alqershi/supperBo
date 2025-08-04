using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace YemenBooking.Core.Entities
{
    public class SponsoredAdSection : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Title { get; private set; }
        
        [StringLength(500)]
        public string Subtitle { get; private set; }
        
        [StringLength(1000)]
        public string Description { get; private set; }
        
        [Column(TypeName = "json")]
        public string PropertyIds { get; private set; } // JSON array of property IDs
        
        [StringLength(500)]
        public string CustomImageUrl { get; private set; }
        
        [StringLength(50)]
        public string BackgroundColor { get; private set; }
        
        [StringLength(50)]
        public string TextColor { get; private set; }
        
        [Column(TypeName = "json")]
        public string Styling { get; private set; } // JSON for custom styling
        
        [Required]
        [StringLength(100)]
        public string CtaText { get; private set; }
        
        [Required]
        [StringLength(100)]
        public string CtaAction { get; private set; } // navigate, external_link, etc.
        
        [Column(TypeName = "json")]
        public string CtaData { get; private set; } // JSON for CTA data
        
        public DateTime StartDate { get; private set; }
        
        public DateTime EndDate { get; private set; }
        
        public int Priority { get; private set; }
        
        [Column(TypeName = "json")]
        public string TargetingData { get; private set; } // JSON for targeting
        
        [Column(TypeName = "json")]
        public string AnalyticsData { get; private set; } // JSON for analytics tracking
        
        public bool IsActive { get; private set; }
        
        // Statistics
        public int ImpressionCount { get; private set; }
        
        public int ClickCount { get; private set; }
        
        public decimal ConversionRate { get; private set; }

        protected SponsoredAdSection() { }

        public SponsoredAdSection(
            string title,
            string propertyIds,
            string ctaText,
            string ctaAction,
            DateTime startDate,
            DateTime endDate,
            int priority = 0,
            string subtitle = null,
            string description = null,
            string customImageUrl = null,
            string backgroundColor = null,
            string textColor = null,
            string styling = "{}",
            string ctaData = "{}",
            string targetingData = "{}",
            string analyticsData = "{}")
        {
            Id = Guid.NewGuid();
            Title = title;
            Subtitle = subtitle;
            Description = description;
            PropertyIds = propertyIds;
            CustomImageUrl = customImageUrl;
            BackgroundColor = backgroundColor;
            TextColor = textColor;
            Styling = styling;
            CtaText = ctaText;
            CtaAction = ctaAction;
            CtaData = ctaData;
            StartDate = startDate;
            EndDate = endDate;
            Priority = priority;
            TargetingData = targetingData;
            AnalyticsData = analyticsData;
            IsActive = true;
            ImpressionCount = 0;
            ClickCount = 0;
            ConversionRate = 0;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAd(
            string title,
            string subtitle,
            string description,
            string propertyIds,
            string customImageUrl,
            string backgroundColor,
            string textColor,
            string styling,
            string ctaText,
            string ctaAction,
            string ctaData,
            DateTime startDate,
            DateTime endDate,
            int priority,
            string targetingData,
            string analyticsData)
        {
            Title = title;
            Subtitle = subtitle;
            Description = description;
            PropertyIds = propertyIds;
            CustomImageUrl = customImageUrl;
            BackgroundColor = backgroundColor;
            TextColor = textColor;
            Styling = styling;
            CtaText = ctaText;
            CtaAction = ctaAction;
            CtaData = ctaData;
            StartDate = startDate;
            EndDate = endDate;
            Priority = priority;
            TargetingData = targetingData;
            AnalyticsData = analyticsData;
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

        public void RecordImpression()
        {
            ImpressionCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordClick()
        {
            ClickCount++;
            CalculateConversionRate();
            UpdatedAt = DateTime.UtcNow;
        }

        private void CalculateConversionRate()
        {
            if (ImpressionCount > 0)
            {
                ConversionRate = (decimal)ClickCount / ImpressionCount * 100;
            }
        }

        // Helper methods
        public bool IsCurrentlyActive => IsActive && DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
        
        public bool IsExpired => DateTime.UtcNow > EndDate;
        
        public bool IsScheduled => DateTime.UtcNow < StartDate;
        
        public TimeSpan RemainingTime => EndDate > DateTime.UtcNow ? EndDate - DateTime.UtcNow : TimeSpan.Zero;
    }
}