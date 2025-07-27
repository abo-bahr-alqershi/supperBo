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
    /// معالج استعلام الحصول على الكيانات حسب النوع
    /// </summary>
    public class GetPropertiesByTypeQueryHandler : IRequestHandler<GetPropertiesByTypeQuery, PaginatedResult<PropertyDto>>
    {
        #region Dependencies
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertiesByTypeQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetPropertiesByTypeQueryHandler(
            IPropertyRepository propertyRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertiesByTypeQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<PaginatedResult<PropertyDto>> Handle(GetPropertiesByTypeQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب كيانات النوع: {PropertyTypeId} - الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.PropertyTypeId, request.PageNumber, request.PageSize);

            // Authorization: تطبيق فلاتر الأمان حسب دور المستخدم
            var userRole = _currentUserService.Role;
            var currentUserId = _currentUserService.UserId;

            var query = _propertyRepository.GetQueryable()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.PropertyType)
                .Include(p => p.Owner)
                .Include(p => p.Images.Where(i => i.IsMainImage && !i.IsDeleted))
                .Where(p => !p.IsDeleted && p.TypeId == request.PropertyTypeId);

            query = userRole switch
            {
                "Admin" => query,
                "Owner" => query.Where(p => p.IsApproved || p.OwnerId == currentUserId),
                _ => query.Where(p => p.IsApproved)
            };

            // Pagination
            var totalCount = await query.CountAsync(cancellationToken);
            var properties = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

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

            _logger.LogInformation("تم جلب {Count} كيان من إجمالي {TotalCount} لكيانات النوع", dtos.Count, totalCount);
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