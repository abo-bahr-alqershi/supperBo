using MediatR;
using System;
using System.Collections.Generic;

namespace YemenBooking.Application.Commands.HomeSections.CityDestinations
{
    public class CreateCityDestinationCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Country { get; set; }
        public string CountryAr { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        public string ImageUrl { get; set; }
        public List<string> AdditionalImages { get; set; } = new();
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Currency { get; set; }
        public bool IsPopular { get; set; }
        public bool IsFeatured { get; set; }
        public int Priority { get; set; }
        public List<string> Highlights { get; set; } = new();
        public List<string> HighlightsAr { get; set; } = new();
        public string WeatherData { get; set; }
        public string AttractionsData { get; set; }
        public string Metadata { get; set; }
    }
}