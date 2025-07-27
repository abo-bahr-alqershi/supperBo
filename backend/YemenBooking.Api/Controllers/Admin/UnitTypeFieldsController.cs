using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.UnitTypeFields;
using YemenBooking.Application.Queries.UnitTypeFields;

namespace YemenBooking.Api.Controllers.Admin
{
    /// <summary>
    /// متحكم بحقول نوع الكيان للمدراء
    /// Controller for property type fields management by admins
    /// </summary>
    [Route("api/admin/unit-type-fields")]
    [Authorize(Roles = "Admin")]
    public class UnitTypeFieldsController : BaseAdminController
    {
        public UnitTypeFieldsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// إنشاء حقل نوع للكيان
        /// Create a new property type field
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUnitTypeField([FromBody] CreateUnitTypeFieldCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات حقل نوع الوحدة
        /// Update an existing property type field
        /// </summary>
        [HttpPut("{fieldId}")]
        public async Task<IActionResult> UpdateUnitTypeField(string fieldId, [FromBody] UpdateUnitTypeFieldCommand command)
        {
            command.FieldId = fieldId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف حقل نوع الوحدة
        /// Delete a property type field
        /// </summary>
        [HttpDelete("{fieldId}")]
        public async Task<IActionResult> DeleteUnitTypeField(string fieldId)
        {
            var command = new DeleteUnitTypeFieldCommand { FieldId = fieldId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تبديل حالة تفعيل حقل النوع
        /// Toggle the status of a property type field
        /// </summary>
        [HttpPatch("{fieldId}/toggle-status")]
        public async Task<IActionResult> ToggleUnitTypeFieldStatus(string fieldId, [FromBody] ToggleUnitTypeFieldStatusCommand command)
        {
            command.FieldId = fieldId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إعادة ترتيب حقول نوع الكيان
        /// Reorder property type fields
        /// </summary>
        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderUnitTypeFields([FromBody] ReorderUnitTypeFieldsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

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

        /// <summary>
        /// تعيين عدة حقول إلى مجموعة نوع الوحدة
        /// Assign multiple fields to a unit type group
        /// </summary>
        [HttpPost("{groupId}/assign-fields")]
        public async Task<IActionResult> AssignFieldsToGroup(string groupId, [FromBody] AssignFieldsToGroupCommand command)
        {
            command.GroupId = groupId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الإسناد الجماعي للحقول إلى المجموعات
        /// Bulk assign fields to various unit type groups
        /// </summary>
        [HttpPost("bulk-assign-fields")]
        public async Task<IActionResult> BulkAssignFieldsToGroups([FromBody] BulkAssignFieldsToGroupsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إزالة حقل من مجموعة نوع الوحدة
        /// Remove a field from a unit type group
        /// </summary>
        [HttpPost("{groupId}/remove-field")]
        public async Task<IActionResult> RemoveFieldFromGroup(string groupId, [FromBody] RemoveFieldFromGroupCommand command)
        {
            command.GroupId = groupId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إعادة ترتيب الحقول ضمن مجموعة نوع الوحدة
        /// Reorder fields within a unit type group
        /// </summary>
        [HttpPost("reorder-fields")]
        public async Task<IActionResult> ReorderFieldsInGroup([FromBody] ReorderFieldsInGroupCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب الحقول غير المجمعة ضمن أي مجموعة لنوع الكيان
        /// Get ungrouped unit type fields for a property type
        /// </summary>
        [HttpGet("ungrouped-fields/{propertyTypeId}")]
        public async Task<IActionResult> GetUngroupedFields(string propertyTypeId, [FromQuery] GetUngroupedFieldsQuery query)
        {
            query.PropertyTypeId = Guid.Parse(propertyTypeId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// bulk assign للحقول لمجموعة واحدة
        /// Bulk assign fields to a group
        /// </summary>
        [HttpPost("{groupId}/bulk-assign-field")]
        public async Task<IActionResult> BulkAssignFieldToGroup(string groupId, [FromBody] BulkAssignFieldToGroupCommand command)
        {
            command.GroupId = groupId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
} 