using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.UnitTypeFields;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بحقول نوع الكيان لأصحاب الكيانات
    /// Controller for property type fields viewing by property owners
    /// </summary>
    [Route("api/property/unit-type-fields")]
    [Authorize(Roles = "Owner")]
    public class UnitTypeFieldsController : BasePropertyController
    {
        public UnitTypeFieldsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب الحقول حسب نوع الوحدة مع دعم الفلاتر
        /// Get unit type fields for a given unit type with filters
        /// </summary>
        [HttpGet("unit-type/{unitTypeId}")]
        public async Task<IActionResult> GetUnitTypeFields(
            string unitTypeId,
            [FromQuery] bool? isActive,
            [FromQuery] bool? isSearchable,
            [FromQuery] bool? isPublic,
            [FromQuery] bool? isForUnits,
            [FromQuery] string? category,
            [FromQuery] string? searchTerm)
        {
            var query = new GetUnitTypeFieldsQuery
            {
                unitTypeId = unitTypeId,
                IsActive = isActive,
                IsSearchable = isSearchable,
                IsPublic = isPublic,
                IsForUnits = isForUnits,
                Category = category,
                SearchTerm = searchTerm
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب بيانات حقل نوع الوحدة بواسطة المعرف
        /// Get property type field by id
        /// </summary>
        [HttpGet("{fieldId}")]
        public async Task<IActionResult> GetUnitTypeFieldById(Guid fieldId, [FromQuery] GetUnitTypeFieldByIdQuery query)
        {
            query.FieldId = fieldId;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب الحقول المجمعة لحقل نوع الوحدة
        /// Get grouped property type fields
        /// </summary>
        [HttpGet("grouped")]
        public async Task<IActionResult> GetUnitTypeFieldsGrouped([FromQuery] GetUnitTypeFieldsGroupedQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 