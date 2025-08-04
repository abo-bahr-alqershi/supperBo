using MediatR;
using System.Collections.Generic;
using System;

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
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public List<string> PropertyIds { get; set; }
        public string CustomImageUrl { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public string Styling { get; set; }
        public string CtaText { get; set; }
        public string CtaAction { get; set; }
        public string CtaData { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public string TargetingData { get; set; }
        public string AnalyticsData { get; set; }
        public bool IsCurrentlyActive { get; set; }
        public int ImpressionCount { get; set; }
        public int ClickCount { get; set; }
        public decimal ConversionRate { get; set; }
        public List<PropertySummaryDto> Properties { get; set; } = new();
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