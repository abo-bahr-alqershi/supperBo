using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YemenBooking.Application.Queries.HomeSections
{
    public class GetCityDestinationsQuery : IRequest<List<CityDestinationDto>>
    {
        public string Language { get; set; } = "en"; // "en" or "ar"
        public bool OnlyActive { get; set; } = true;
        public bool OnlyPopular { get; set; } = false;
        public bool OnlyFeatured { get; set; } = false;
        public int? Limit { get; set; }
        public string SortBy { get; set; } = "priority"; // priority, name, popularity
    }

    public class CityDestinationDto
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("nameAr")] public string NameAr { get; set; }
        [JsonPropertyName("country")] public string Country { get; set; }
        [JsonPropertyName("countryAr")] public string CountryAr { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("descriptionAr")] public string DescriptionAr { get; set; }
        [JsonPropertyName("imageUrl")] public string ImageUrl { get; set; }
        [JsonPropertyName("additionalImages")] public List<string> AdditionalImages { get; set; }
        [JsonPropertyName("latitude")] public decimal Latitude { get; set; }
        [JsonPropertyName("longitude")] public decimal Longitude { get; set; }
        [JsonPropertyName("propertyCount")] public int PropertyCount { get; set; }
        [JsonPropertyName("averagePrice")] public decimal AveragePrice { get; set; }
        [JsonPropertyName("currency")] public string Currency { get; set; }
        [JsonPropertyName("averageRating")] public decimal AverageRating { get; set; }
        [JsonPropertyName("reviewCount")] public int ReviewCount { get; set; }
        [JsonPropertyName("isPopular")] public bool IsPopular { get; set; }
        [JsonPropertyName("isFeatured")] public bool IsFeatured { get; set; }
        [JsonPropertyName("priority")] public int Priority { get; set; }
        [JsonPropertyName("highlights")] public List<string> Highlights { get; set; }
        [JsonPropertyName("highlightsAr")] public List<string> HighlightsAr { get; set; }
        [JsonPropertyName("weatherData")] public Dictionary<string, object> WeatherData { get; set; }
        [JsonPropertyName("attractionsData")] public Dictionary<string, object> AttractionsData { get; set; }
        [JsonPropertyName("metadata")] public Dictionary<string, object> Metadata { get; set; }
        [JsonPropertyName("createdAt")] public string CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")] public string UpdatedAt { get; set; }
        [JsonPropertyName("isActive")] public bool IsActive { get; set; }
        [JsonPropertyName("localizedFullName")] public string LocalizedFullName { get; set; }
    }
}