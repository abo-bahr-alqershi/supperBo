using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.UnitFieldValues;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.UnitFieldValues
{
    /// <summary>
    /// معالج استعلام جلب قيم الحقول للوحدة مجمعة حسب المجموعات
    /// Handles GetUnitFieldValuesGroupedQuery and returns unit field values grouped by field group
    /// </summary>
    public class GetUnitFieldValuesGroupedQueryHandler : IRequestHandler<GetUnitFieldValuesGroupedQuery, List<FieldGroupWithValuesDto>>
    {
        private readonly IUnitFieldValueRepository _valueRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IFieldGroupRepository _groupRepository;
        private readonly ILogger<GetUnitFieldValuesGroupedQueryHandler> _logger;

        public GetUnitFieldValuesGroupedQueryHandler(
            IUnitFieldValueRepository valueRepository,
            IUnitRepository unitRepository,
            IFieldGroupRepository groupRepository,
            ILogger<GetUnitFieldValuesGroupedQueryHandler> logger)
        {
            _valueRepository = valueRepository;
            _unitRepository = unitRepository;
            _groupRepository = groupRepository;
            _logger = logger;
        }

        public async Task<List<FieldGroupWithValuesDto>> Handle(GetUnitFieldValuesGroupedQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب قيم الحقول للوحدة مجمعة حسب المجموعات: {UnitId}", request.UnitId);

            if (request.UnitId == Guid.Empty)
                throw new ValidationException(nameof(request.UnitId), "معرف الوحدة غير صالح");

            var unit = await _unitRepository.GetQueryable()
                .AsNoTracking()
                .Include(u => u.UnitType)
                .FirstOrDefaultAsync(u => u.Id == request.UnitId, cancellationToken);

            if (unit == null)
                throw new NotFoundException("Unit", request.UnitId.ToString(), "الوحدة غير موجودة");

            var propertyTypeId = unit.UnitType.PropertyTypeId;
            var groups = await _groupRepository.GetGroupsByUnitTypeIdAsync(propertyTypeId, cancellationToken);

            var values = await _valueRepository.GetQueryable()
                .AsNoTracking()
                .Include(v => v.UnitTypeField)
                .Where(v => v.UnitId == request.UnitId)
                .Where(v => !request.IsPublic.HasValue || v.UnitTypeField.IsPublic == request.IsPublic.Value)
                .ToListAsync(cancellationToken);

            var result = groups.OrderBy(g => g.SortOrder)
                .Select(g => new FieldGroupWithValuesDto
                {
                    GroupId = g.Id,
                    GroupName = g.GroupName,
                    DisplayName = g.DisplayName,
                    Description = g.Description,
                    FieldValues = values
                        .Where(v => g.FieldGroupFields.Any(fgf => fgf.FieldId == v.UnitTypeFieldId))
                        .Select(v => new FieldWithValueDto
                        {
                            ValueId = v.Id,
                            FieldId = v.UnitTypeFieldId,
                            FieldName = v.UnitTypeField.FieldName,
                            DisplayName = v.UnitTypeField.DisplayName,
                            Value = v.FieldValue
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }
    }
} 