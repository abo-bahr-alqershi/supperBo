using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.PropertyTypes;
using YemenBooking.Application.DTOs.Properties;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.PropertyTypes
{
    /// <summary>
    /// معالج استعلام الحصول على جميع أنواع الكيانات
    /// Query handler for GetAllPropertyTypesQuery
    /// </summary>
    public class GetAllPropertyTypesQueryHandler : IRequestHandler<GetAllPropertyTypesQuery, PaginatedResult<PropertyTypeDto>>
    {
        private readonly IPropertyTypeRepository _repo;
        private readonly ILogger<GetAllPropertyTypesQueryHandler> _logger;

        public GetAllPropertyTypesQueryHandler(
            IPropertyTypeRepository repo,
            ILogger<GetAllPropertyTypesQueryHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<PaginatedResult<PropertyTypeDto>> Handle(GetAllPropertyTypesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام جميع أنواع الكيانات - الصفحة: {PageNumber}, الحجم: {PageSize}", request.PageNumber, request.PageSize);

            var all = await _repo.GetAllPropertyTypesAsync(cancellationToken);
            var dtos = all.Select(pt => new PropertyTypeDto
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description,
                DefaultAmenities = string.IsNullOrEmpty(pt.DefaultAmenities) 
                    ? new List<string>() 
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(pt.DefaultAmenities) ?? new List<string>()
            }).ToList();

            var totalCount = dtos.Count;
            var items = dtos.Skip((request.PageNumber - 1) * request.PageSize)
                            .Take(request.PageSize)
                            .ToList();

            return new PaginatedResult<PropertyTypeDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
} 