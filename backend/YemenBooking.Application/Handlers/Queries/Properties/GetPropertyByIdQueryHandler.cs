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
using YemenBooking.Application.Queries.CP.Properties;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Properties
{
    /// <summary>
    /// معالج استعلام الحصول على بيانات الكيان بواسطة المعرف
    /// Query to get property details by ID
    /// </summary>
    public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, ResultDto<PropertyDetailsDto>>
    {
        #region Dependencies
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFieldGroupRepository _fieldGroupRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertyByIdQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetPropertyByIdQueryHandler(
            IPropertyRepository propertyRepository,
            IFieldGroupRepository fieldGroupRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertyByIdQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _fieldGroupRepository = fieldGroupRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler
        public async Task<ResultDto<PropertyDetailsDto>> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("بدء معالجة استعلام بيانات الكيان: {PropertyId}", request.PropertyId);

                // 1. التحقق من صحة المعرف
                if (request.PropertyId == Guid.Empty)
                    return ResultDto<PropertyDetailsDto>.Failure("معرف الكيان غير صالح");

                // 2. جلب الكيان مع البيانات المرتبطة
                var property = await _propertyRepository.GetQueryable()
                    .AsNoTracking()
                    .Include(p => p.PropertyType)
                    .Include(p => p.Owner)
                    .Include(p => p.Amenities).ThenInclude(pa => pa.PropertyTypeAmenity).ThenInclude(pta => pta.Amenity)
                    .Include(p => p.Units).ThenInclude(u => u.UnitType)
                    .Include(p => p.Units).ThenInclude(u => u.FieldValues).ThenInclude(fv => fv.UnitTypeField)
                    .FirstOrDefaultAsync(p => p.Id == request.PropertyId, cancellationToken);

                if (property == null)
                    return ResultDto<PropertyDetailsDto>.Failure($"الكيان بالمعرف {request.PropertyId} غير موجود");

                // 3. التحقق من الصلاحيات
                var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
                if (!property.IsApproved)
                {
                    if (currentUser == null)
                        return ResultDto<PropertyDetailsDto>.Failure("يجب تسجيل الدخول لعرض هذا الكيان");

                    var roles = _currentUserService.UserRoles;
                    if (property.OwnerId != _currentUserService.UserId && !roles.Contains("Admin"))
                        return ResultDto<PropertyDetailsDto>.Failure("ليس لديك صلاحية لعرض هذا الكيان");
                }

                // 4. بناء DTO للاستجابة
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
                    // صور الكيان
                    Images = property.Images.Select(i => new PropertyImageDto
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
                    }).ToList(),
                    Units = property.Units.Select(u => new UnitDto
                    {
                        Id = u.Id,
                        PropertyId = u.PropertyId,
                        UnitTypeId = u.UnitTypeId,
                        Name = u.Name,
                        BasePrice = new MoneyDto
                        {
                            Amount = u.BasePrice.Amount,
                            Currency = u.BasePrice.Currency
                        },
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
                    }).ToList(),
                    Amenities = property.Amenities.Select(pa => new AmenityDto
                    {
                        Id = pa.PropertyTypeAmenity.AmenityId,
                        Name = pa.PropertyTypeAmenity.Amenity.Name,
                        Description = pa.PropertyTypeAmenity.Amenity.Description
                    }).ToList()
                };


                return ResultDto<PropertyDetailsDto>.Ok(dto, "تم استرجاع بيانات الكيان بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في معالجة استعلام بيانات الكيان: {PropertyId}", request.PropertyId);
                return ResultDto<PropertyDetailsDto>.Failure("حدث خطأ داخلي أثناء معالجة الطلب");
            }
        }
        #endregion
    }
} 