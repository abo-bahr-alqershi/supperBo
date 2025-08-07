using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YemenBooking.Application.Queries.HomeSections
{
    public class GetDynamicHomeSectionsQuery : IRequest<List<DynamicHomeSectionDto>>
    {
        public string Language { get; set; } = "en"; // "en" or "ar"
        public List<string> TargetAudience { get; set; } = new();
        public bool IncludeContent { get; set; } = true;
        public bool OnlyActive { get; set; } = true;
    }

    public class DynamicHomeSectionDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string SectionType { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }

        [JsonPropertyName("createdAt")]
        // Serialized as ISO8601 string
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("config")]
        public Dictionary<string, object> SectionConfig { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonPropertyName("scheduledAt")]
        public string ScheduledAt { get; set; }

        [JsonPropertyName("expiresAt")]
        public string ExpiresAt { get; set; }

        [JsonPropertyName("targetAudience")]
        public List<string> TargetAudience { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("content")]
        public List<DynamicSectionContentDto> Content { get; set; } = new();
    }

    public class DynamicSectionContentDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("sectionId")]
        public string SectionId { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, object> Data { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonPropertyName("expiresAt")]
        public string ExpiresAt { get; set; }

        [JsonPropertyName("displayOrder")]
        public int DisplayOrder { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string UpdatedAt { get; set; }
    }
}