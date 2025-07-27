using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.SearchFilters;
using YemenBooking.Core.Interfaces.Repositories;
using System.Collections.Generic;

namespace YemenBooking.Application.Handlers.Queries.SearchFilters
{
    /// <summary>
    /// معالج استعلام الحصول على الكيانات المميزة
    /// Query handler for GetFeaturedPropertiesQuery
    /// </summary>
    public class GetFeaturedPropertiesQueryHandler : IRequestHandler<GetFeaturedPropertiesQuery, PaginatedResult<PropertyDto>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ILogger<GetFeaturedPropertiesQueryHandler> _logger;

        public GetFeaturedPropertiesQueryHandler(IPropertyRepository propertyRepository, ILogger<GetFeaturedPropertiesQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _logger = logger;
        }

        public async Task<PaginatedResult<PropertyDto>> Handle(GetFeaturedPropertiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام الكيانات المميزة بحد أقصى: {Count}", request.Count);

            var properties = await _propertyRepository.GetFeaturedPropertiesAsync(request.Count, cancellationToken);
            var dtos = properties.Select(p => new PropertyDto
            {
                Id = p.Id,
                OwnerId = p.OwnerId,
                TypeId = p.TypeId,
                Name = p.Name,
                Address = p.Address,
                City = p.City,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                StarRating = p.StarRating,
                Description = p.Description,
                IsApproved = p.IsApproved,
                CreatedAt = p.CreatedAt,
                OwnerName = p.Owner?.Name ?? string.Empty,
                TypeName = p.PropertyType?.Name ?? string.Empty
            }).ToList();

            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                dtos = dtos.Where(x => x.City.Equals(request.Location, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return new PaginatedResult<PropertyDto>
            {
                Items = dtos,
                PageNumber = 1,
                PageSize = request.Count,
                TotalCount = dtos.Count()
            };
        }
    }
} 