using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Properties;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Properties
{
    /// <summary>
    /// معالج استعلام الحصول على الكيانات حسب المدينة
    /// Handler for GetPropertiesByCityQuery
    /// </summary>
    public class GetPropertiesByCityQueryHandler : IRequestHandler<GetPropertiesByCityQuery, PaginatedResult<PropertyDto>>
    {
        #region Dependencies
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertiesByCityQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetPropertiesByCityQueryHandler(
            IPropertyRepository propertyRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertiesByCityQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<PaginatedResult<PropertyDto>> Handle(GetPropertiesByCityQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب الكيانات في المدينة: {CityName} - الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.CityName, request.PageNumber, request.PageSize);

            // بناء الاستعلام الأساسي مع التضمينات
            var userRole = _currentUserService.Role;
            var userId = _currentUserService.UserId;

            var query = _propertyRepository.GetQueryable()
                .AsNoTracking()
                // Removed AsSplitQuery for compatibility
                .Include(p => p.PropertyType)
                .Include(p => p.Owner)
                .Include(p => p.Images.Where(i => i.IsMainImage && !i.IsDeleted))
                .Where(p => !p.IsDeleted && p.City == request.CityName);

            // تطبيق فلاتر الأمان حسب دور المستخدم
            query = userRole switch
            {
                "Admin" => query,
                "Owner" => query.Where(p => p.IsApproved || p.OwnerId == userId),
                _ => query.Where(p => p.IsApproved)
            };

            // تنفيذ الصفحات
            var totalCount = await query.CountAsync(cancellationToken);
            var properties = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // تحويل إلى DTOs
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
                OwnerName = p.Owner.Name,
                TypeName = p.PropertyType.Name,
                Images = p.Images.Select(i => new PropertyImageDto
                {
                    Id = i.Id,
                    PropertyId = i.PropertyId,
                    UnitId = i.UnitId,
                    Name = i.Name,
                    Url = i.Url,
                    SizeBytes = i.SizeBytes,
                    Type = i.Type,
                    Category = i.Category,
                    Caption = i.Caption,
                    AltText = i.AltText,
                    Tags = i.Tags,
                    Sizes = i.Sizes,
                    IsMain = i.IsMain,
                    DisplayOrder = i.DisplayOrder,
                    UploadedAt = i.UploadedAt,
                    Status = i.Status,
                    AssociationType = i.PropertyId != null ? "Property" : "Unit"
                }).ToList()
            }).ToList();

            _logger.LogInformation("تم جلب {Count} كيان من إجمالي {TotalCount} في المدينة", dtos.Count, totalCount);
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