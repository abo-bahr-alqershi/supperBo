using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.UnitTypeFields
{
    /// <summary>
    /// أمر إعادة ترتيب الحقول ضمن مجموعة
    /// Command to reorder fields within a group
    /// </summary>
    public class ReorderFieldsInGroupCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف المجموعة
        /// Group identifier
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// قائمة معرفات الحقول بالترتيب الجديد
        /// List of field identifiers in the new order
        /// </summary>
        public List<string> FieldIds { get; set; } = new List<string>();
    }
} 