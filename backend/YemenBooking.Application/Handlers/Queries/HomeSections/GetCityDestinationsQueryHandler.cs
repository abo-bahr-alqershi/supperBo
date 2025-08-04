using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Queries.HomeSections;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.MobileApp.HomeSections
{
    public class GetCityDestinationsQueryHandler : IRequestHandler<GetCityDestinationsQuery, List<CityDestinationDto>>
    {
        private readonly IRepository<CityDestinationSection> _destinationRepository;

        public GetCityDestinationsQueryHandler(IRepository<CityDestinationSection> destinationRepository)
        {
            _destinationRepository = destinationRepository;
        }

        public async Task<List<CityDestinationDto>> Handle(GetCityDestinationsQuery request, CancellationToken cancellationToken)
        {
            var query = _destinationRepository.GetQuery();

            if (request.OnlyActive)
            {
                query = query.Where(d => d.IsActive);
            }
            if (request.OnlyPopular)
            {
                query = query.Where(d => d.IsPopular);
            }
            if (request.OnlyFeatured)
            {
                query = query.Where(d => d.IsFeatured);
            }

            switch (request.SortBy?.ToLowerInvariant())
            {
                case "name":
                    query = query.OrderBy(d => request.Language == "ar" ? d.NameAr : d.Name);
                    break;
                case "popularity":
                    query = query.OrderByDescending(d => d.ReviewCount);
                    break;
                default:
                    query = query.OrderBy(d => d.Priority);
                    break;
            }

            if (request.Limit.HasValue)
            {
                query = query.Take(request.Limit.Value);
            }

            var destinations = await query.ToListAsync(cancellationToken);

            var result = destinations.Select(d => new CityDestinationDto
            {
                Id = d.Id.ToString(),
                Name = d.Name,
                NameAr = d.NameAr,
                Country = d.Country,
                CountryAr = d.CountryAr,
                Description = d.Description,
                DescriptionAr = d.DescriptionAr,
                ImageUrl = d.ImageUrl,
                AdditionalImages = DeserializeJsonArray(d.AdditionalImages),
                Latitude = d.Latitude,
                Longitude = d.Longitude,
                PropertyCount = d.PropertyCount,
                AveragePrice = d.AveragePrice,
                Currency = d.Currency,
                AverageRating = d.AverageRating,
                ReviewCount = d.ReviewCount,
                IsPopular = d.IsPopular,
                IsFeatured = d.IsFeatured,
                Priority = d.Priority,
                Highlights = DeserializeJsonArray(d.Highlights),
                HighlightsAr = DeserializeJsonArray(d.HighlightsAr),
                WeatherData = JsonSerializer.Deserialize<Dictionary<string, object>>(d.WeatherData),
                AttractionsData = JsonSerializer.Deserialize<Dictionary<string, object>>(d.AttractionsData),
                Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(d.Metadata),
                CreatedAt = d.CreatedAt.ToString("O"),
                UpdatedAt = d.UpdatedAt.ToString("O"),
                IsActive = d.IsActive,
                LocalizedFullName = d.GetFullName(request.Language == "ar")
            }).ToList();

            return result;
        }

        private List<string> DeserializeJsonArray(string json)
        {
            try
            {
                return string.IsNullOrEmpty(json)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}