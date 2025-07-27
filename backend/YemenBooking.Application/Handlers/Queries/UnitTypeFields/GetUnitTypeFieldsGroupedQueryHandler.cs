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
    /// معالج استعلام جلب الحقول الديناميكية مجمعة حسب المجموعات
    /// Query handler for GetUnitTypeFieldsGroupedQuery
    /// </summary>
    public class GetUnitTypeFieldsGroupedQueryHandler : IRequestHandler<GetUnitTypeFieldsGroupedQuery, List<FieldGroupWithFieldsDto>>
    {
        private readonly IUnitTypeFieldRepository _fieldRepo;
        private readonly IFieldGroupRepository _groupRepo;
        private readonly ILogger<GetUnitTypeFieldsGroupedQueryHandler> _logger;

        public GetUnitTypeFieldsGroupedQueryHandler(
            IUnitTypeFieldRepository fieldRepo,
            IFieldGroupRepository groupRepo,
            ILogger<GetUnitTypeFieldsGroupedQueryHandler> logger)
        {
            _fieldRepo = fieldRepo;
            _groupRepo = groupRepo;
            _logger = logger;
        }

        public async Task<List<FieldGroupWithFieldsDto>> Handle(GetUnitTypeFieldsGroupedQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام الحقول مجمعة حسب المجموعات لنوع الكيان: {PropertyTypeId}", request.PropertyTypeId);

            if (!Guid.TryParse(request.PropertyTypeId, out var typeId))
                throw new ValidationException(nameof(request.PropertyTypeId), "معرف نوع الكيان غير صالح");

            var groups = await _groupRepo.GetGroupsByUnitTypeIdAsync(typeId, cancellationToken);
            var fields = await _fieldRepo.GetQueryable()
                .AsNoTracking()
                .Where(f => f.UnitTypeId == typeId)
                .Where(f => !request.IsPublic.HasValue || f.IsPublic == request.IsPublic.Value)
                .ToListAsync(cancellationToken);

            var result = groups.OrderBy(g => g.SortOrder)
                .Select(g => new FieldGroupWithFieldsDto
                {
                    GroupId = g.Id.ToString(),
                    GroupName = g.GroupName,
                    DisplayName = g.DisplayName,
                    Description = g.Description,
                    SortOrder = g.SortOrder,
                    IsCollapsible = g.IsCollapsible,
                    IsExpandedByDefault = g.IsExpandedByDefault,
                    Fields = fields
                        .Where(f => g.FieldGroupFields.Any(link => link.FieldId == f.Id))
                        .Select(f => new UnitTypeFieldDto
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
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }
    }
} 