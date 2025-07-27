using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.UnitTypeFields
{
    /// <summary>
    /// أمر إزالة حقل من مجموعة
    /// Command to remove a field from a group
    /// </summary>
    public class RemoveFieldFromGroupCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف الحقل
        /// Field identifier
        /// </summary>
        public string FieldId { get; set; }

        /// <summary>
        /// معرف المجموعة
        /// Group identifier
        /// </summary>
        public string GroupId { get; set; }
    }
} 