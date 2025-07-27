using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.UnitTypeFields
{
    /// <summary>
    /// أمر إسناد عدة حقول إلى مجموعة
    /// Command to assign multiple fields to a group
    /// </summary>
    public class AssignFieldsToGroupCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف المجموعة
        /// Group identifier
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// قائمة معرفات الحقول
        /// List of field identifiers
        /// </summary>
        public List<string> FieldIds { get; set; } = new List<string>();
    }
} 