using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.SearchFilters;
using YemenBooking.Application.Queries.SearchFilters;
using YemenBooking.Application.Queries.UnitTypeFields;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بفلاتر البحث للمدراء
    /// Controller for managing search filters by admins
    /// </summary>
    public class SearchFiltersController : BaseAdminController
    {
        public SearchFiltersController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء فلتر بحث
        /// Create a new search filter
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSearchFilter([FromBody] CreateSearchFilterCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث فلتر بحث
        /// Update an existing search filter
        /// </summary>
        [HttpPut("{filterId}")]
        public async Task<IActionResult> UpdateSearchFilter(Guid filterId, [FromBody] UpdateSearchFilterCommand command)
        {
            command.FilterId = filterId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف فلتر بحث
        /// Delete a search filter
        /// </summary>
        [HttpDelete("{filterId}")]
        public async Task<IActionResult> DeleteSearchFilter(Guid filterId)
        {
            var command = new DeleteSearchFilterCommand { FilterId = filterId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تبديل حالة التفعيل لفلتر بحث
        /// Toggle search filter status (enabled/disabled)
        /// </summary>
        [HttpPatch("{filterId}/toggle-status")]
        public async Task<IActionResult> ToggleSearchFilterStatus(Guid filterId, [FromBody] ToggleSearchFilterStatusCommand command)
        {
            command.FilterId = filterId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب جميع فلاتر البحث لنوع كيان معين
        /// Get all search filters for a specific property type
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSearchFilters([FromQuery] GetSearchFiltersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب فلتر بحث حسب المعرف
        /// Get a search filter by its ID
        /// </summary>
        [HttpGet("{filterId}")]
        public async Task<IActionResult> GetSearchFilterById(Guid filterId)
        {
            var query = new GetSearchFilterByIdQuery { FilterId = filterId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب الحقول القابلة للبحث لنوع كيان معين
        /// Get searchable fields for a specific property type
        /// </summary>
        [HttpGet("searchable-fields")]
        public async Task<IActionResult> GetSearchableFields([FromQuery] GetSearchableFieldsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 