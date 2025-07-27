using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.UnitTypeFields
{
    /// <summary>
    /// أمر الإسناد الجماعي للحقول إلى المجموعات
    /// Command for bulk assigning fields to various groups
    /// </summary>
    public class BulkAssignFieldsToGroupsCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// قائمة عمليات الإسناد (معرف الحقل، معرف المجموعة، الترتيب)
        /// List of assignments (fieldId, groupId, sortOrder)
        /// </summary>
        public List<FieldGroupAssignmentDto> Assignments { get; set; } = new List<FieldGroupAssignmentDto>();
    }

    /// <summary>
    /// نموذج لعملية إسناد حقل لمجموعة
    /// DTO for field-to-group assignment
    /// </summary>
    public class FieldGroupAssignmentDto
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

        /// <summary>
        /// ترتيب الحقل داخل المجموعة
        /// Sort order within the group
        /// </summary>
        public int SortOrder { get; set; }
    }
} 