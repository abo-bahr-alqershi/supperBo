using MediatR;
using System.Collections.Generic;

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
        public string Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Country { get; set; }
        public string CountryAr { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        public string ImageUrl { get; set; }
        public List<string> AdditionalImages { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int PropertyCount { get; set; }
        public decimal AveragePrice { get; set; }
        public string Currency { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsPopular { get; set; }
        public bool IsFeatured { get; set; }
        public int Priority { get; set; }
        public List<string> Highlights { get; set; }
        public List<string> HighlightsAr { get; set; }
        public string WeatherData { get; set; }
        public string AttractionsData { get; set; }
        public string Metadata { get; set; }

        // Localized properties for easier access
        public string LocalizedName { get; set; }
        public string LocalizedCountry { get; set; }
        public string LocalizedDescription { get; set; }
        public List<string> LocalizedHighlights { get; set; }
        public string LocalizedFullName { get; set; }
    }
}