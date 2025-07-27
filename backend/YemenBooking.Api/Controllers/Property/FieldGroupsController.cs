using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.FieldGroups;

namespace YemenBooking.Api.Controllers.Property
{
    /// <summary>
    /// متحكم بعرض مجموعات حقول الكيان لأصحاب الكيانات
    /// Controller for property owners to view field groups
    /// </summary>
    public class FieldGroupsController : BasePropertyController
    {
        public FieldGroupsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب بيانات مجموعة الحقول حسب المعرف
        /// Get field group by id
        /// </summary>
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetFieldGroupById(string groupId)
        {
            var query = new GetFieldGroupByIdQuery { GroupId = groupId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// جلب مجموعات الحقول حسب نوع وحدة
        /// Get field groups by unit type
        /// </summary>
        [HttpGet("unit-type/{unitTypeId}")]
        public async Task<IActionResult> GetFieldGroupsByUnitType(string unitTypeId)
        {
            var query = new GetFieldGroupsByUnitTypeQuery { UnitTypeId = unitTypeId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 