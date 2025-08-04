using MediatR;

namespace YemenBooking.Application.Queries.HomeSections
{
    public class GetHomeConfigQuery : IRequest<DynamicHomeConfigDto>
    {
        public string Version { get; set; } // Optional: get specific version
    }

    public class DynamicHomeConfigDto
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }
        public string PublishedAt { get; set; }
        public string GlobalSettings { get; set; }
        public string ThemeSettings { get; set; }
        public string LayoutSettings { get; set; }
        public string CacheSettings { get; set; }
        public string AnalyticsSettings { get; set; }
        public string EnabledFeatures { get; set; }
        public string ExperimentalFeatures { get; set; }
    }
}