using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.UnitTypeFields;
using YemenBooking.Core.Interfaces.Repositories;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace YemenBooking.Application.Handlers.Queries.UnitTypeFields
{
    /// <summary>
    /// معالج استعلام لجلب الحقول غير المجمعة ضمن أي مجموعة لنوع الكيان
    /// Handler for GetUngroupedFieldsQuery
    /// </summary>
    public class GetUngroupedFieldsQueryHandler : IRequestHandler<GetUngroupedFieldsQuery, PaginatedResult<UnitTypeFieldDto>>
    {
        private readonly IUnitTypeFieldRepository _fieldRepository;
        private readonly IFieldGroupFieldRepository _linkRepository;
        private readonly ILogger<GetUngroupedFieldsQueryHandler> _logger;

        public GetUngroupedFieldsQueryHandler(
            IUnitTypeFieldRepository fieldRepository,
            IFieldGroupFieldRepository linkRepository,
            ILogger<GetUngroupedFieldsQueryHandler> logger)
        {
            _fieldRepository = fieldRepository;
            _linkRepository = linkRepository;
            _logger = logger;
        }

        public async Task<PaginatedResult<UnitTypeFieldDto>> Handle(GetUngroupedFieldsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching ungrouped fields for PropertyTypeId={PropertyTypeId}", request.PropertyTypeId);

            // get all field IDs already in any group
            var groupedFieldIds = await _linkRepository.GetQueryable()
                .Where(l => l.UnitTypeField.UnitTypeId == request.PropertyTypeId)
                .Select(l => l.FieldId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var query = _fieldRepository.GetQueryable()
                .AsNoTracking()
                .Where(f => f.UnitTypeId == request.PropertyTypeId)
                .Where(f => !groupedFieldIds.Contains(f.Id));

            if (request.IsSearchable.HasValue)
                query = query.Where(f => f.IsSearchable == request.IsSearchable.Value);
            if (request.IsPublic.HasValue)
                query = query.Where(f => f.IsPublic == request.IsPublic.Value);
            if (request.IsForUnits.HasValue)
                query = query.Where(f => f.IsForUnits == request.IsForUnits.Value);
            if (!string.IsNullOrWhiteSpace(request.Category))
                query = query.Where(f => f.Category == request.Category);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(f => f.SortOrder)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Map to DTO
            var dtos = items.Select(f => new UnitTypeFieldDto
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
                GroupId = string.Empty,
                IsForUnits = f.IsForUnits
            }).ToList();

            return new PaginatedResult<UnitTypeFieldDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
} 