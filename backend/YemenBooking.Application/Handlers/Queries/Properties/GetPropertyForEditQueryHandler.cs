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
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Properties
{
    /// <summary>
    /// معالج استعلام جلب بيانات الكيان للتحرير مع القيم الديناميكية
    /// Query handler to get property data for edit form including dynamic field values
    /// </summary>
    public class GetPropertyForEditQueryHandler : IRequestHandler<GetPropertyForEditQuery, ResultDto<PropertyEditDto>>
    {
        #region Dependencies
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFieldGroupRepository _fieldGroupRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertyForEditQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetPropertyForEditQueryHandler(
            IPropertyRepository propertyRepository,
            IFieldGroupRepository fieldGroupRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertyForEditQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _fieldGroupRepository = fieldGroupRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<ResultDto<PropertyEditDto>> Handle(GetPropertyForEditQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("بدء جلب بيانات الكيان للتحرير: {PropertyId}", request.PropertyId);

                // 1. التحقق من صحة المعرفات
                if (request.PropertyId == Guid.Empty)
                    return ResultDto<PropertyEditDto>.Failure("معرف الكيان غير صالح");
                if (request.OwnerId == Guid.Empty)
                    return ResultDto<PropertyEditDto>.Failure("معرف المالك غير صالح");

                // 2. التحقق من الصلاحيات: يجب أن يكون المالك الحالي أو مسؤول
                var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
                if (currentUser == null || (_currentUserService.UserId != request.OwnerId && !_currentUserService.UserRoles.Contains("Admin")))
                    return ResultDto<PropertyEditDto>.Failure("ليس لديك صلاحية لتحرير هذا الكيان");

                // 3. جلب الكيان مع القيم الديناميكية
                var property = await _propertyRepository.GetQueryable()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == request.PropertyId && p.OwnerId == request.OwnerId, cancellationToken);

                if (property == null)
                    return ResultDto<PropertyEditDto>.Failure("الكيان غير موجود أو لا تملك صلاحية الوصول إليه");

                // 4. بناء DTO
                var dto = new PropertyEditDto
                {
                    PropertyId = property.Id,
                    Name = property.Name,
                    Address = property.Address,
                    City = property.City,
                    Latitude = property.Latitude,
                    Longitude = property.Longitude,
                    StarRating = property.StarRating,
                    Description = property.Description,
                    PropertyTypeId = property.TypeId
                };

                return ResultDto<PropertyEditDto>.Ok(dto, "تم جلب بيانات الكيان للتحرير بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب بيانات الكيان للتحرير: {PropertyId}", request.PropertyId);
                return ResultDto<PropertyEditDto>.Failure("حدث خطأ داخلي أثناء جلب بيانات التحرير");
            }
        }
        #endregion
    }
} 