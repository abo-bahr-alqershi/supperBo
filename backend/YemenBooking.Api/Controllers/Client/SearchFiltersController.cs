using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.UnitTypeFields;
using YemenBooking.Application.Queries.SearchFilters;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعرض فلاتر البحث للعملاء
    /// Controller for clients to view search filters
    /// </summary>
    public class SearchFiltersController : BaseClientController
    {
        public SearchFiltersController(IMediator mediator) : base(mediator) { }

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