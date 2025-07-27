using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Properties;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Properties
{
    /// <summary>
    /// معالج استعلام الحصول على الكيانات القريبة من موقع معين
    /// Handler for GetPropertiesNearLocationQuery
    /// </summary>
    public class GetPropertiesNearLocationQueryHandler : IRequestHandler<GetPropertiesNearLocationQuery, PaginatedResult<PropertyDto>>
    {
        #region Dependencies
        private readonly IPropertyRepository _propertyRepository;
        private readonly IGeolocationService _geolocationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertiesNearLocationQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetPropertiesNearLocationQueryHandler(
            IPropertyRepository propertyRepository,
            IGeolocationService geolocationService,
            ICurrentUserService currentUserService,
            ILogger<GetPropertiesNearLocationQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _geolocationService = geolocationService;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<PaginatedResult<PropertyDto>> Handle(GetPropertiesNearLocationQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب الكيانات القريبة من ({Latitude}, {Longitude}) ضمن مسافة {Distance} كم - الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.Latitude, request.Longitude, request.Distance, request.PageNumber, request.PageSize);

            // جلب جميع الكيانات غير المحذوفة
            var allProperties = await _propertyRepository.GetQueryable()
                .AsNoTracking()
                .Include(p => p.PropertyType)
                .Include(p => p.Owner)
                .Where(p => !p.IsDeleted)
                .ToListAsync(cancellationToken);

            // تطبيق فلاتر الأمان
            var userRole = _currentUserService.Role;
            var userId = _currentUserService.UserId;
            var filtered = userRole switch
            {
                "Admin" => allProperties,
                "Owner" => allProperties.Where(p => p.IsApproved || p.OwnerId == userId).ToList(),
                _ => allProperties.Where(p => p.IsApproved).ToList()
            };

            // حساب المسافة وتصفيتها
            var nearby = new System.Collections.Generic.List<Property>();
            foreach (var prop in filtered)
            {
                var distance = await _geolocationService.CalculateDistanceAsync(
                    request.Latitude, request.Longitude,
                    (double)prop.Latitude, (double)prop.Longitude,
                    cancellationToken);
                if (distance <= request.Distance)
                    nearby.Add(prop);
            }

            // تنفيذ الصفحات
            var totalCount = nearby.Count;
            var paged = nearby
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // تحويل إلى DTOs
            var dtos = paged.Select(p => new PropertyDto
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
                OwnerName = p.Owner.Name,
                TypeName = p.PropertyType.Name
            }).ToList();

            _logger.LogInformation("تم جلب {Count} كيان من إجمالي {TotalCount} القريبة", dtos.Count, totalCount);
            return new PaginatedResult<PropertyDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        #endregion
    }
} 