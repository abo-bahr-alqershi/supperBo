using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.SearchFilters;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.SearchFilters
{
    /// <summary>
    /// معالج استعلام جلب فلتر البحث حسب المعرف
    /// Query handler for GetSearchFilterByIdQuery
    /// </summary>
    public class GetSearchFilterByIdQueryHandler : IRequestHandler<GetSearchFilterByIdQuery, SearchFilterDto>
    {
        private readonly ISearchFilterRepository _filterRepository;
        private readonly ILogger<GetSearchFilterByIdQueryHandler> _logger;

        public GetSearchFilterByIdQueryHandler(
            ISearchFilterRepository filterRepository,
            ILogger<GetSearchFilterByIdQueryHandler> logger)
        {
            _filterRepository = filterRepository;
            _logger = logger;
        }

        public async Task<SearchFilterDto> Handle(GetSearchFilterByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام جلب فلتر البحث حسب المعرف: {FilterId}", request.FilterId);

            if (request.FilterId == Guid.Empty)
                throw new ValidationException(nameof(request.FilterId), "معرف الفلتر غير صالح");

            var filter = await _filterRepository.GetQueryable()
                .AsNoTracking()
                .Include(sf => sf.UnitTypeField)
                .FirstOrDefaultAsync(sf => sf.Id == request.FilterId, cancellationToken);

            if (filter == null)
            {
                throw new NotFoundException("SearchFilter", request.FilterId.ToString(), $"فلتر البحث بالمعرف {request.FilterId} غير موجود");
            }

            return new SearchFilterDto
            {
                FilterId = filter.Id,
                FieldId = filter.FieldId,
                FilterType = filter.FilterType,
                DisplayName = filter.DisplayName,
                FilterOptions = JsonSerializer.Deserialize<Dictionary<string, object>>(filter.FilterOptions) ?? new Dictionary<string, object>(),
                IsActive = filter.IsActive,
                SortOrder = filter.SortOrder,
                Field = new UnitTypeFieldDto
                {
                    FieldId = filter.UnitTypeField.Id.ToString(),
                    UnitTypeId = filter.UnitTypeField.UnitTypeId.ToString(),
                    FieldTypeId = filter.UnitTypeField.FieldTypeId.ToString(),
                    FieldName = filter.UnitTypeField.FieldName,
                    DisplayName = filter.UnitTypeField.DisplayName,
                    Description = filter.UnitTypeField.Description,
                    FieldOptions = JsonSerializer.Deserialize<Dictionary<string, object>>(filter.UnitTypeField.FieldOptions) ?? new Dictionary<string, object>(),
                    ValidationRules = JsonSerializer.Deserialize<Dictionary<string, object>>(filter.UnitTypeField.ValidationRules) ?? new Dictionary<string, object>(),
                    IsRequired = filter.UnitTypeField.IsRequired,
                    IsSearchable = filter.UnitTypeField.IsSearchable,
                    IsPublic = filter.UnitTypeField.IsPublic,
                    SortOrder = filter.UnitTypeField.SortOrder,
                    Category = filter.UnitTypeField.Category,
                    GroupId = filter.UnitTypeField.FieldGroupFields.FirstOrDefault()?.GroupId.ToString() ?? string.Empty
                }
            };
        }
    }
} 