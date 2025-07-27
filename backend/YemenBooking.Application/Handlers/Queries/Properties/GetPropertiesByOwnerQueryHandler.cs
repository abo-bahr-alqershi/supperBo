using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Properties;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Properties
{
    /// <summary>
    /// معالج استعلام الحصول على كيانات المالك
    /// Handler for GetPropertiesByOwnerQuery
    /// </summary>
    public class GetPropertiesByOwnerQueryHandler : IRequestHandler<GetPropertiesByOwnerQuery, PaginatedResult<PropertyDto>>
    {
        #region Dependencies
        private readonly IPropertyRepository _propertyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertiesByOwnerQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetPropertiesByOwnerQueryHandler(
            IPropertyRepository propertyRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertiesByOwnerQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<PaginatedResult<PropertyDto>> Handle(GetPropertiesByOwnerQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب كيانات المالك: {OwnerId} - الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.OwnerId, request.PageNumber, request.PageSize);

            // Authorization: only admin or the owner themselves
            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
            {
                _logger.LogWarning("محاولة الوصول بدون تسجيل دخول");
                throw new UnauthorizedAccessException("يجب تسجيل الدخول لعرض كيانات المالك");
            }
            var role = _currentUserService.Role;
            if (role != "Admin" && _currentUserService.UserId != request.OwnerId)
            {
                _logger.LogWarning("ليس لدى المستخدم الصلاحيات اللازمة لعرض كيانات هذا المالك");
                throw new UnauthorizedAccessException("ليس لديك صلاحية لعرض كيانات هذا المالك");
            }

            // Build query
            var query = _propertyRepository.GetQueryable()
                .AsNoTracking()
                .Include(p => p.PropertyType)
                .Include(p => p.Owner)
                .Include(p => p.Images.Where(i => i.IsMainImage && !i.IsDeleted))
                .Where(p => !p.IsDeleted && p.OwnerId == request.OwnerId);

            // Execute pagination
            var totalCount = await query.CountAsync(cancellationToken);
            var properties = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Map to DTO
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

            _logger.LogInformation("تم جلب {Count} كيان من إجمالي {TotalCount} لكيانات المالك", dtos.Count, totalCount);
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