using MediatR;
using System;
using System.Collections.Generic;

namespace YemenBooking.Application.Commands.HomeSections.SponsoredAds
{
    public class UpdateSponsoredAdCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public List<Guid> PropertyIds { get; set; } = new();
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
    }
}