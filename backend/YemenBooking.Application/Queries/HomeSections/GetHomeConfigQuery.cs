using MediatR;
using System.Text.Json.Serialization;

namespace YemenBooking.Application.Queries.HomeSections
{
    public class GetHomeConfigQuery : IRequest<DynamicHomeConfigDto>
    {
        public string Version { get; set; } // Optional: get specific version
    }

    public class DynamicHomeConfigDto
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("version")] public string Version { get; set; }
        [JsonPropertyName("isActive")] public bool IsActive { get; set; }
        [JsonPropertyName("createdAt")] public string CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")] public string UpdatedAt { get; set; }
        [JsonPropertyName("publishedAt")] public string PublishedAt { get; set; }
        [JsonPropertyName("globalSettings")] public Dictionary<string, object> GlobalSettings { get; set; }
        [JsonPropertyName("themeSettings")] public Dictionary<string, object> ThemeSettings { get; set; }
        [JsonPropertyName("layoutSettings")] public Dictionary<string, object> LayoutSettings { get; set; }
        [JsonPropertyName("cacheSettings")] public Dictionary<string, object> CacheSettings { get; set; }
        [JsonPropertyName("analyticsSettings")] public Dictionary<string, object> AnalyticsSettings { get; set; }
        [JsonPropertyName("enabledFeatures")] public List<string> EnabledFeatures { get; set; }
        [JsonPropertyName("experimentalFeatures")] public Dictionary<string, object> ExperimentalFeatures { get; set; }
    }
}