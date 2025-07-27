using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.UnitFieldValues;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace YemenBooking.Application.Handlers.Queries.UnitFieldValues
{
    /// <summary>
    /// معالج استعلام جلب قيمة حقل للوحدة حسب المعرف
    /// Handles GetUnitFieldValueByIdQuery and returns the requested unit field value
    /// </summary>
    public class GetUnitFieldValueByIdQueryHandler : IRequestHandler<GetUnitFieldValueByIdQuery, UnitFieldValueDto>
    {
        private readonly IUnitFieldValueRepository _valueRepository;
        private readonly ILogger<GetUnitFieldValueByIdQueryHandler> _logger;

        public GetUnitFieldValueByIdQueryHandler(
            IUnitFieldValueRepository valueRepository,
            ILogger<GetUnitFieldValueByIdQueryHandler> logger)
        {
            _valueRepository = valueRepository;
            _logger = logger;
        }

        public async Task<UnitFieldValueDto> Handle(GetUnitFieldValueByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب قيمة حقل للوحدة بالمعرف: {ValueId}", request.ValueId);

            if (request.ValueId == Guid.Empty)
                throw new ValidationException(nameof(request.ValueId), "معرف القيمة غير صالح");

            var entity = await _valueRepository.GetQueryable()
                .AsNoTracking()
                .Include(v => v.UnitTypeField)
                .FirstOrDefaultAsync(v => v.Id == request.ValueId, cancellationToken);

            if (entity == null)
                throw new NotFoundException("UnitFieldValue", request.ValueId.ToString(), $"قيمة حقل الوحدة بالمعرف {request.ValueId} غير موجودة");

            return new UnitFieldValueDto
            {
                ValueId = entity.Id,
                UnitId = entity.UnitId,
                FieldId = entity.UnitTypeFieldId,
                FieldName = entity.UnitTypeField.FieldName,
                DisplayName = entity.UnitTypeField.DisplayName,
                FieldValue = entity.FieldValue,
                Field = new UnitTypeFieldDto
                {
                    FieldId = entity.UnitTypeField.Id.ToString(),
                    UnitTypeId = entity.UnitTypeField.UnitTypeId.ToString(),
                    FieldTypeId = entity.UnitTypeField.FieldTypeId.ToString(),
                    FieldName = entity.UnitTypeField.FieldName,
                    DisplayName = entity.UnitTypeField.DisplayName,
                    Description = entity.UnitTypeField.Description,
                    FieldOptions = JsonSerializer.Deserialize<Dictionary<string, object>>(entity.UnitTypeField.FieldOptions) ?? new Dictionary<string, object>(),
                    ValidationRules = JsonSerializer.Deserialize<Dictionary<string, object>>(entity.UnitTypeField.ValidationRules) ?? new Dictionary<string, object>(),
                    IsRequired = entity.UnitTypeField.IsRequired,
                    IsSearchable = entity.UnitTypeField.IsSearchable,
                    IsPublic = entity.UnitTypeField.IsPublic,
                    SortOrder = entity.UnitTypeField.SortOrder,
                    Category = entity.UnitTypeField.Category,
                    GroupId = entity.UnitTypeField.FieldGroupFields.FirstOrDefault()?.GroupId.ToString() ?? string.Empty
                },
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
} 