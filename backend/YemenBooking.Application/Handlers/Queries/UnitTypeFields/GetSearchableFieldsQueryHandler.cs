using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.UnitTypeFields;
using YemenBooking.Core.Interfaces.Repositories;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Queries.UnitTypeFields
{
    /// <summary>
    /// معالج استعلام جلب الحقول القابلة للبحث
    /// Query handler for GetSearchableFieldsQuery
    /// </summary>
    public class GetSearchableFieldsQueryHandler : IRequestHandler<GetSearchableFieldsQuery, List<UnitTypeFieldDto>>
    {
        private readonly IUnitTypeFieldRepository _fieldRepo;
        private readonly ILogger<GetSearchableFieldsQueryHandler> _logger;

        public GetSearchableFieldsQueryHandler(
            IUnitTypeFieldRepository fieldRepo,
            ILogger<GetSearchableFieldsQueryHandler> logger)
        {
            _fieldRepo = fieldRepo;
            _logger = logger;
        }

        public async Task<List<UnitTypeFieldDto>> Handle(GetSearchableFieldsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام الحقول القابلة للبحث لنوع الكيان: {PropertyTypeId}", request.PropertyTypeId);

            if (!Guid.TryParse(request.PropertyTypeId, out var typeId))
                throw new ValidationException(nameof(request.PropertyTypeId), "معرف نوع الكيان غير صالح");

            var fields = await _fieldRepo.GetQueryable()
                .AsNoTracking()
                .Where(f => f.UnitTypeId == typeId && f.IsSearchable)
                .OrderBy(f => f.SortOrder)
                .ToListAsync(cancellationToken);

            return fields.Select(f => new UnitTypeFieldDto
            {
                FieldId = f.Id.ToString(),
                UnitTypeId = f.UnitTypeId.ToString(),
                FieldTypeId = f.FieldTypeId.ToString(),
                FieldName = f.FieldName,
                DisplayName = f.DisplayName,
                Description = f.Description,
                FieldOptions = JsonSerializer.Deserialize<Dictionary<string, object>>(f.FieldOptions) ?? new Dictionary<string, object>(),
                ValidationRules = JsonSerializer.Deserialize<Dictionary<string, object>>(f.ValidationRules) ?? new Dictionary<string, object>(),
                IsRequired = f.IsRequired,
                IsSearchable = f.IsSearchable,
                IsPublic = f.IsPublic,
                SortOrder = f.SortOrder,
                Category = f.Category,
                GroupId = f.FieldGroupFields.FirstOrDefault()?.GroupId.ToString() ?? string.Empty
            }).ToList();
        }
    }
} 