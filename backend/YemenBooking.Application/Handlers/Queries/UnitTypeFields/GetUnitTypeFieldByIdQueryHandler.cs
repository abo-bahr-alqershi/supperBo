using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.UnitTypeFields;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace YemenBooking.Application.Handlers.Queries.UnitTypeFields
{
    /// <summary>
    /// معالج استعلام الحصول على حقل نوع الوحدة حسب المعرف
    /// Handles GetUnitTypeFieldByIdQuery and returns the requested field definition
    /// </summary>
    public class GetUnitTypeFieldByIdQueryHandler : IRequestHandler<GetUnitTypeFieldByIdQuery, ResultDto<UnitTypeFieldDto>>
    {
        private readonly IUnitTypeFieldRepository _fieldRepo;
        private readonly ILogger<GetUnitTypeFieldByIdQueryHandler> _logger;

        public GetUnitTypeFieldByIdQueryHandler(
            IUnitTypeFieldRepository fieldRepo,
            ILogger<GetUnitTypeFieldByIdQueryHandler> logger)
        {
            _fieldRepo = fieldRepo;
            _logger = logger;
        }

        public async Task<ResultDto<UnitTypeFieldDto>> Handle(GetUnitTypeFieldByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب حقل نوع الوحدة حسب المعرف: {FieldId}", request.FieldId);

            if (request.FieldId == Guid.Empty)
            {
                _logger.LogWarning("معرف الحقل غير صالح");
                return ResultDto<UnitTypeFieldDto>.Failure("معرف الحقل غير صالح");
            }

            var field = await _fieldRepo.GetUnitTypeFieldByIdAsync(request.FieldId, cancellationToken);
            if (field == null)
            {
                _logger.LogWarning("الحقل غير موجود: {FieldId}", request.FieldId);
                return ResultDto<UnitTypeFieldDto>.Failure("الحقل غير موجود");
            }

            // Map to DTO
            var dto = new UnitTypeFieldDto
            {
                FieldId = field.Id.ToString(),
                UnitTypeId = field.UnitTypeId.ToString(),
                FieldTypeId = field.FieldTypeId.ToString(),
                FieldName = field.FieldName,
                DisplayName = field.DisplayName,
                Description = field.Description,
                FieldOptions = JsonSerializer.Deserialize<Dictionary<string, object>>(field.FieldOptions) ?? new Dictionary<string, object>(),
                ValidationRules = JsonSerializer.Deserialize<Dictionary<string, object>>(field.ValidationRules) ?? new Dictionary<string, object>(),
                IsRequired = field.IsRequired,
                IsSearchable = field.IsSearchable,
                IsPublic = field.IsPublic,
                SortOrder = field.SortOrder,
                Category = field.Category,
                GroupId = field.FieldGroupFields.FirstOrDefault()?.GroupId.ToString() ?? string.Empty,
                IsForUnits = field.IsForUnits,
                ShowInCards = field.ShowInCards,
                IsPrimaryFilter = field.IsPrimaryFilter,
                Priority = field.Priority
            };

            return ResultDto<UnitTypeFieldDto>.Ok(dto);
        }
    }
} 