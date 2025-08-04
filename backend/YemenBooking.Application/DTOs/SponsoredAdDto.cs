using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs
{
    public class SponsoredAdResponseDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public List<string> PropertyIds { get; set; } = new();
        public string CustomImageUrl { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public dynamic Styling { get; set; } // Parsed JSON
        public string CtaText { get; set; }
        public string CtaAction { get; set; }
        public dynamic CtaData { get; set; } // Parsed JSON
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public dynamic TargetingData { get; set; } // Parsed JSON
        public dynamic AnalyticsData { get; set; } // Parsed JSON
        public bool IsActive { get; set; }
        public int ImpressionCount { get; set; }
        public int ClickCount { get; set; }
        public decimal ConversionRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Computed properties
        public bool IsCurrentlyActive { get; set; }
        public bool IsExpired { get; set; }
        public bool IsScheduled { get; set; }
        public TimeSpan RemainingTime { get; set; }
        
        // Related data
        public List<PropertySummaryResponseDto> Properties { get; set; } = new();
    }

    public class PropertySummaryResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string MainImageUrl { get; set; }
        public decimal? BasePrice { get; set; }
        public string Currency { get; set; }
        public double AverageRating { get; set; }
        public int StarRating { get; set; }
        public List<string> TopAmenities { get; set; } = new();
    }

    public class CreateSponsoredAdRequestDto
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public List<string> PropertyIds { get; set; } = new();
        public string CustomImageUrl { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public object Styling { get; set; }
        public string CtaText { get; set; }
        public string CtaAction { get; set; }
        public object CtaData { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public object TargetingData { get; set; }
        public object AnalyticsData { get; set; }
    }

    public class UpdateSponsoredAdRequestDto
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public List<string> PropertyIds { get; set; } = new();
        public string CustomImageUrl { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public object Styling { get; set; }
        public string CtaText { get; set; }
        public string CtaAction { get; set; }
        public object CtaData { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public object TargetingData { get; set; }
        public object AnalyticsData { get; set; }
    }

    public class AdInteractionResponseDto
    {
        public string AdId { get; set; }
        public string InteractionType { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public dynamic AdditionalData { get; set; } // Parsed JSON
    }
}