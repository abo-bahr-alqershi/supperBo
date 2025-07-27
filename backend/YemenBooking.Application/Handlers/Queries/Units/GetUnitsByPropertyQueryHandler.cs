using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Units;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Units
{
    /// <summary>
    /// معالج استعلام الحصول على وحدات كيان محدد
    /// Query handler for GetUnitsByPropertyQuery
    /// </summary>
    public class GetUnitsByPropertyQueryHandler : IRequestHandler<GetUnitsByPropertyQuery, PaginatedResult<UnitDto>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetUnitsByPropertyQueryHandler> _logger;
        private readonly IFieldGroupRepository _groupRepository; // Added this line

        public GetUnitsByPropertyQueryHandler(
            IUnitRepository unitRepository,
            ICurrentUserService currentUserService,
            ILogger<GetUnitsByPropertyQueryHandler> logger,
            IFieldGroupRepository groupRepository) // Added this line
        {
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _logger = logger;
            _groupRepository = groupRepository; // Added this line
        }

        public async Task<PaginatedResult<UnitDto>> Handle(GetUnitsByPropertyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام وحدات الكيان: {PropertyId}", request.PropertyId);

            var query = _unitRepository.GetQueryable()
                .AsNoTracking()
                .Include(u => u.Property)
                .Include(u => u.UnitType)
                .Include(u => u.FieldValues)
                .Include(p => p.Images.Where(i => !i.IsDeleted))
                .Where(u => u.PropertyId == request.PropertyId);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            var role = _currentUserService.Role;
            bool isOwner = currentUser != null && _currentUserService.PropertyId == request.PropertyId;
            if (role != "Admin" && !isOwner)
                query = query.Where(u => u.IsAvailable && u.Property.IsApproved);

            if (request.IsAvailable.HasValue)
                query = query.Where(u => u.IsAvailable == request.IsAvailable.Value);

            if (request.MinBasePrice.HasValue)
                query = query.Where(u => u.BasePrice.Amount >= request.MinBasePrice.Value);

            if (request.MaxBasePrice.HasValue)
                query = query.Where(u => u.BasePrice.Amount <= request.MaxBasePrice.Value);

            if (request.MinCapacity.HasValue)
                query = query.Where(u => u.MaxCapacity >= request.MinCapacity.Value);

            if (!string.IsNullOrWhiteSpace(request.NameContains))
                query = query.Where(u => u.Name.Contains(request.NameContains));

            var totalCount = await query.CountAsync(cancellationToken);

            var unitsList = await query
                .OrderBy(u => u.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // بناء قائمة DTOs مع تجميع القيم الديناميكية ضمن المجمعات
            var dtos = new List<UnitDto>();
            foreach (var unit in unitsList)
            {
                var dto = new UnitDto
                {
                    Id = unit.Id,
                    PropertyId = unit.PropertyId,
                    UnitTypeId = unit.UnitTypeId,
                    Name = unit.Name,
                    BasePrice = new MoneyDto { Amount = unit.BasePrice.Amount, Currency = unit.BasePrice.Currency },
                    CustomFeatures = unit.CustomFeatures,
                    IsAvailable = unit.IsAvailable,
                    PropertyName = unit.Property.Name,
                    UnitTypeName = unit.UnitType.Name,
                    PricingMethod = unit.PricingMethod,
                    Images = unit.Images.Select(i => new PropertyImageDto
                    {
                        Id = i.Id,
                        Url = i.Url,
                        IsMain = i.IsMainImage,
                        DisplayOrder = i.DisplayOrder,
                        Category = i.Category,
                        Type = i.Type,
                        SizeBytes = i.SizeBytes,
                        Name = i.Name,
                        Caption = i.Caption,
                        AltText = i.AltText,
                        Sizes = i.Sizes
                    }).ToList(),
                    FieldValues = new List<UnitFieldValueDto>() // لم يعد نستخدم القائمة المسطحة
                };

                // جلب مجموعات الحقول لهذا النوع
                var groups = await _groupRepository.GetGroupsByUnitTypeIdAsync(unit.UnitTypeId, cancellationToken);
                foreach (var group in groups.OrderBy(g => g.SortOrder))
                {
                    var groupDto = new FieldGroupWithValuesDto
                    {
                        GroupId = group.Id,
                        GroupName = group.GroupName,
                        DisplayName = group.DisplayName,
                        Description = group.Description,
                        FieldValues = new List<FieldWithValueDto>()
                    };

                    // أدخل القيم المطابقة لكل رابط بين المجموعة والحقل
                    foreach (var link in group.FieldGroupFields.OrderBy(l => l.SortOrder))
                    {
                        var valueEntity = unit.FieldValues.FirstOrDefault(v => v.UnitTypeFieldId == link.FieldId);
                        if (valueEntity != null)
                        {
                            groupDto.FieldValues.Add(new FieldWithValueDto
                            {
                                ValueId = valueEntity.Id,
                                FieldId = link.FieldId,
                                FieldName = link.UnitTypeField.FieldName,
                                DisplayName = link.UnitTypeField.DisplayName,
                                Value = valueEntity.FieldValue
                            });
                        }
                    }

                    dto.DynamicFields.Add(groupDto);
                }

                dtos.Add(dto);
            }

            return new PaginatedResult<UnitDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
} 