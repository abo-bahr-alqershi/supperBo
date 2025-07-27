using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;
using YemenBooking.Application.Queries.Properties;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Properties
{
    /// <summary>
    /// معالج استعلام جلب تفاصيل الكيان مع الحقول الديناميكية والوحدات الاختيارية
    /// Query handler to get property details with optional dynamic fields and units
    /// </summary>
    public class GetPropertyDetailsQueryHandler : IRequestHandler<GetPropertyDetailsQuery, ResultDto<PropertyDetailsDto>>
    {
        #region Dependencies
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFieldGroupRepository _fieldGroupRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertyDetailsQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetPropertyDetailsQueryHandler(
            IPropertyRepository propertyRepository,
            IFieldGroupRepository fieldGroupRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertyDetailsQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _fieldGroupRepository = fieldGroupRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<ResultDto<PropertyDetailsDto>> Handle(GetPropertyDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("بدء جلب تفاصيل الكيان: {PropertyId}", request.PropertyId);

                // 1. التحقق من صحة المعرف
                if (request.PropertyId == Guid.Empty)
                    return ResultDto<PropertyDetailsDto>.Failure("معرف الكيان غير صالح");

                // 2. بناء الاستعلام
                var query = _propertyRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(p => p.PropertyType)
                    .Include(p => p.Owner)
                    .Include(p => p.Amenities).ThenInclude(pa => pa.PropertyTypeAmenity).ThenInclude(pta => pta.Amenity);


                var property = await query.FirstOrDefaultAsync(p => p.Id == request.PropertyId, cancellationToken);

                if (property == null)
                    return ResultDto<PropertyDetailsDto>.Failure($"الكيان بالمعرف {request.PropertyId} غير موجود");

                // 3. الصلاحيات
                var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
                if (!property.IsApproved)
                {
                    if (currentUser == null)
                        return ResultDto<PropertyDetailsDto>.Failure("يجب تسجيل الدخول لعرض هذا الكيان");
                    if (property.OwnerId != _currentUserService.UserId && !_currentUserService.UserRoles.Contains("Admin"))
                        return ResultDto<PropertyDetailsDto>.Failure("ليس لديك صلاحية لعرض هذا الكيان");
                }

                // 4. بناء DTO
                var dto = new PropertyDetailsDto
                {
                    Id = property.Id,
                    OwnerId = property.OwnerId,
                    TypeId = property.TypeId,
                    Name = property.Name,
                    Address = property.Address,
                    City = property.City,
                    Latitude = property.Latitude,
                    Longitude = property.Longitude,
                    StarRating = property.StarRating,
                    Description = property.Description,
                    IsApproved = property.IsApproved,
                    CreatedAt = property.CreatedAt,
                    OwnerName = property.Owner.Name,
                    TypeName = property.PropertyType.Name,
                    Units = request.IncludeUnits
                        ? property.Units.Select(u => new UnitDto
                        {
                            Id = u.Id,
                            PropertyId = u.PropertyId,
                            UnitTypeId = u.UnitTypeId,
                            Name = u.Name,
                            BasePrice = new MoneyDto { Amount = u.BasePrice.Amount, Currency = u.BasePrice.Currency },
                            CustomFeatures = u.CustomFeatures,
                            IsAvailable = u.IsAvailable,
                            PropertyName = property.Name,
                            UnitTypeName = u.UnitType.Name,
                            PricingMethod = u.PricingMethod,
                            FieldValues = u.FieldValues.OrderBy(fv => fv.CreatedAt).Select(fv => new UnitFieldValueDto
                                {
                                    ValueId = fv.Id,
                                    UnitId = fv.UnitId,
                                    FieldId = fv.UnitTypeFieldId,
                                    FieldName = fv.UnitTypeField.FieldName,
                                    DisplayName = fv.UnitTypeField.DisplayName,
                                    FieldValue = fv.FieldValue,
                                    Field = new UnitTypeFieldDto
                                    {
                                        FieldId = fv.UnitTypeField.Id.ToString(),
                                        UnitTypeId = fv.UnitTypeField.UnitTypeId.ToString(),
                                        FieldTypeId = fv.UnitTypeField.FieldTypeId.ToString(),
                                        FieldName = fv.UnitTypeField.FieldName,
                                        DisplayName = fv.UnitTypeField.DisplayName,
                                        Description = fv.UnitTypeField.Description,
                                        FieldOptions = new Dictionary<string, object>(),
                                        ValidationRules = new Dictionary<string, object>(),
                                        IsRequired = fv.UnitTypeField.IsRequired,
                                        IsSearchable = fv.UnitTypeField.IsSearchable,
                                        IsPublic = fv.UnitTypeField.IsPublic,
                                        SortOrder = fv.UnitTypeField.SortOrder,
                                        Category = fv.UnitTypeField.Category,
                                        GroupId = string.Empty
                                    },
                                    CreatedAt = fv.CreatedAt,
                                    UpdatedAt = fv.UpdatedAt
                                }).ToList()
                        }).ToList()
                        : new List<UnitDto>(),
                    Amenities = property.Amenities.Select(pa => new AmenityDto
                    {
                        Id = pa.PropertyTypeAmenity.AmenityId,
                        Name = pa.PropertyTypeAmenity.Amenity.Name,
                        Description = pa.PropertyTypeAmenity.Amenity.Description
                    }).ToList()
                };


                return ResultDto<PropertyDetailsDto>.Ok(dto, "تم استرجاع تفاصيل الكيان بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب تفاصيل الكيان: {PropertyId}", request.PropertyId);
                return ResultDto<PropertyDetailsDto>.Failure("حدث خطأ داخلي أثناء جلب التفاصيل");
            }
        }
        #endregion
    }
} 