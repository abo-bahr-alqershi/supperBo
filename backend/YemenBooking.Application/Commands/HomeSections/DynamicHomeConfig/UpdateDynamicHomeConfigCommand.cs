using MediatR;
using System;

namespace YemenBooking.Application.Commands.HomeSections.DynamicHomeConfig
{
    public class UpdateDynamicHomeConfigCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string GlobalSettings { get; set; }
        public string ThemeSettings { get; set; }
        public string LayoutSettings { get; set; }
        public string CacheSettings { get; set; }
        public string AnalyticsSettings { get; set; }
        public string EnabledFeatures { get; set; }
        public string ExperimentalFeatures { get; set; }
        public string Description { get; set; }
    }
}