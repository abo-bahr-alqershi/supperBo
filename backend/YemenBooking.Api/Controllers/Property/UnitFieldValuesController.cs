using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.UnitFieldValues;
using YemenBooking.Application.Queries.UnitFieldValues;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بقيم حقول الوحدات لأصحاب الكيانات
    /// Controller for managing unit field values by property owners
    /// </summary>
    public class UnitFieldValuesController : BasePropertyController
    {
        public UnitFieldValuesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء قيمة حقل لوحدة
        /// Create a new unit field value
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUnitFieldValue([FromBody] CreateUnitFieldValueCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث قيمة حقل لوحدة
        /// Update an existing unit field value
        /// </summary>
        [HttpPut("{valueId}")]
        public async Task<IActionResult> UpdateUnitFieldValue(Guid valueId, [FromBody] UpdateUnitFieldValueCommand command)
        {
            command.ValueId = valueId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف قيمة حقل لوحدة
        /// Delete a unit field value
        /// </summary>
        [HttpDelete("{valueId}")]
        public async Task<IActionResult> DeleteUnitFieldValue(Guid valueId)
        {
            var command = new DeleteUnitFieldValueCommand { ValueId = valueId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث متعدد لقيم حقول الوحدات
        /// Bulk update unit field values
        /// </summary>
        [HttpPost("bulk-update")]
        public async Task<IActionResult> BulkUpdateUnitFieldValues([FromBody] BulkUpdateUnitFieldValuesCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء جماعي لقيم حقول الوحدات
        /// Bulk create unit field values
        /// </summary>
        [HttpPost("bulk-create")]
        public async Task<IActionResult> BulkCreateUnitFieldValues([FromBody] BulkCreateUnitFieldValueCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف جماعي لقيم حقول الوحدات
        /// Bulk delete unit field values
        /// </summary>
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDeleteUnitFieldValues([FromBody] BulkDeleteUnitFieldValueCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث جماعي لقيم حقول الوحدات
        /// Bulk update unit field values
        /// </summary>
        [HttpPost("bulk-update-value")]
        public async Task<IActionResult> BulkUpdateUnitFieldValue([FromBody] BulkUpdateUnitFieldValueCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب قيم حقول الوحدات
        /// Get all unit field values
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUnitFieldValues([FromQuery] GetUnitFieldValuesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب قيم حقول الوحدات مجمعة حسب المجموعات
        /// Get unit field values grouped by field groups
        /// </summary>
        [HttpGet("grouped")]
        public async Task<IActionResult> GetUnitFieldValuesGrouped([FromQuery] GetUnitFieldValuesGroupedQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب قيمة حقل لوحدة حسب المعرف
        /// Get a unit field value by ID
        /// </summary>
        [HttpGet("{valueId}")]
        public async Task<IActionResult> GetUnitFieldValueById(Guid valueId)
        {
            var query = new GetUnitFieldValueByIdQuery { ValueId = valueId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 