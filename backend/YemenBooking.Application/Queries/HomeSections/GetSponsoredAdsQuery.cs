using MediatR;
using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;

namespace YemenBooking.Application.Queries.HomeSections
{
    public class GetSponsoredAdsQuery : IRequest<List<SponsoredAdDto>>
    {
        public bool OnlyActive { get; set; } = true;
        public List<string> TargetAudience { get; set; } = new();
        public int? Limit { get; set; }
        public bool IncludePropertyDetails { get; set; } = false;
    }

    public class SponsoredAdDto
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("subtitle")] public string Subtitle { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("property")] public PropertySummaryDto Property { get; set; }
        [JsonPropertyName("propertyIds")] public List<string> PropertyIds { get; set; }
        [JsonPropertyName("customImageUrl")] public string CustomImageUrl { get; set; }
        [JsonPropertyName("backgroundColor")] public string BackgroundColor { get; set; }
        [JsonPropertyName("textColor")] public string TextColor { get; set; }
        [JsonPropertyName("styling")] public Dictionary<string, object> Styling { get; set; }
        [JsonPropertyName("ctaText")] public string CtaText { get; set; }
        [JsonPropertyName("ctaAction")] public string CtaAction { get; set; }
        [JsonPropertyName("ctaData")] public Dictionary<string, object> CtaData { get; set; }
        [JsonPropertyName("startDate")] public DateTime StartDate { get; set; }
        [JsonPropertyName("endDate")] public DateTime EndDate { get; set; }
        [JsonPropertyName("priority")] public int Priority { get; set; }
        [JsonPropertyName("targetingData")] public Dictionary<string, object> TargetingData { get; set; }
        [JsonPropertyName("analyticsData")] public Dictionary<string, object> AnalyticsData { get; set; }
        [JsonPropertyName("isActive")] public bool IsActive { get; set; }
        [JsonPropertyName("createdAt")] public string CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")] public string UpdatedAt { get; set; }
        [JsonPropertyName("impressionCount")] public int ImpressionCount { get; set; }
        [JsonPropertyName("clickCount")] public int ClickCount { get; set; }
        [JsonPropertyName("conversionRate")] public decimal ConversionRate { get; set; }
    }

    public class PropertySummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainImageUrl { get; set; }
        public decimal? BasePrice { get; set; }
        public string Currency { get; set; }
        public double AverageRating { get; set; }
    }
}