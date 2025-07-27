using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.UnitTypeFields
{
    /// <summary>
    /// أمر تعيين عدة حقول لمجموعة وحدة واحدة
    /// Command for bulk assigning fields to a single unit type group
    /// </summary>
    public class BulkAssignFieldToGroupCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف المجموعة
        /// Group identifier
        /// </summary>
        public string GroupId { get; set; } = string.Empty;

        /// <summary>
        /// قائمة معرفات الحقول
        /// List of field identifiers
        /// </summary>
        public List<string> FieldIds { get; set; } = new List<string>();
    }
} 