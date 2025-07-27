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
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Queries.UnitFieldValues
{
    /// <summary>
    /// معالج استعلام جلب قيم الحقول لوحدة معينة
    /// Handles GetUnitFieldValuesQuery and returns a list of unit field values
    /// </summary>
    public class GetUnitFieldValuesQueryHandler : IRequestHandler<GetUnitFieldValuesQuery, List<UnitFieldValueDto>>
    {
        private readonly IUnitFieldValueRepository _valueRepository;
        private readonly ILogger<GetUnitFieldValuesQueryHandler> _logger;

        public GetUnitFieldValuesQueryHandler(
            IUnitFieldValueRepository valueRepository,
            ILogger<GetUnitFieldValuesQueryHandler> logger)
        {
            _valueRepository = valueRepository;
            _logger = logger;
        }

        public async Task<List<UnitFieldValueDto>> Handle(GetUnitFieldValuesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب قيم الحقول للوحدة: {UnitId}", request.UnitId);

            if (request.UnitId == Guid.Empty)
                throw new ValidationException(nameof(request.UnitId), "معرف الوحدة غير صالح");

            var query = _valueRepository.GetQueryable()
                .AsNoTracking()
                .Include(v => v.UnitTypeField)
                .Where(v => v.UnitId == request.UnitId);

            var entities = await query.ToListAsync(cancellationToken);

            var items = entities
                .Where(v => !request.IsPublic.HasValue || v.UnitTypeField.IsPublic == request.IsPublic.Value)
                .Select(v => new UnitFieldValueDto
                {
                    ValueId = v.Id,
                    UnitId = v.UnitId,
                    FieldId = v.UnitTypeFieldId,
                    FieldName = v.UnitTypeField.FieldName,
                    DisplayName = v.UnitTypeField.DisplayName,
                    FieldValue = v.FieldValue,
                    Field = new UnitTypeFieldDto
                    {
                        FieldId = v.UnitTypeField.Id.ToString(),
                        UnitTypeId = v.UnitTypeField.UnitTypeId.ToString(),
                        FieldTypeId = v.UnitTypeField.FieldTypeId.ToString(),
                        FieldName = v.UnitTypeField.FieldName,
                        DisplayName = v.UnitTypeField.DisplayName,
                        Description = v.UnitTypeField.Description,
                        FieldOptions = JsonSerializer.Deserialize<Dictionary<string, object>>(v.UnitTypeField.FieldOptions) ?? new Dictionary<string, object>(),
                        ValidationRules = JsonSerializer.Deserialize<Dictionary<string, object>>(v.UnitTypeField.ValidationRules) ?? new Dictionary<string, object>(),
                        IsRequired = v.UnitTypeField.IsRequired,
                        IsSearchable = v.UnitTypeField.IsSearchable,
                        IsPublic = v.UnitTypeField.IsPublic,
                        SortOrder = v.UnitTypeField.SortOrder,
                        Category = v.UnitTypeField.Category,
                        GroupId = v.UnitTypeField.FieldGroupFields.FirstOrDefault()?.GroupId.ToString() ?? string.Empty
                    },
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt
                })
                .ToList();

            return items;
        }
    }
} 