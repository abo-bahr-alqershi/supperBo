using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.UnitTypeFields;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.UnitTypeFields
{
    /// <summary>
    /// معالج أمر إسناد حقل إلى مجموعة
    /// Assigns a dynamic field to a field group and includes:
    /// - Input validation
    /// - Existence and ownership checks
    /// - Authorization (Admin only)
    /// - Add or update assignment
    /// - Audit logging
    /// - Event publishing
    /// </summary>
    public class AssignFieldToGroupCommandHandler : IRequestHandler<AssignFieldToGroupCommand, ResultDto<bool>>
    {
        private readonly IFieldGroupFieldRepository _linkRepository;
        private readonly IFieldGroupRepository _groupRepository;
        private readonly IUnitTypeFieldRepository _fieldRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<AssignFieldToGroupCommandHandler> _logger;

        public AssignFieldToGroupCommandHandler(
            IFieldGroupFieldRepository linkRepository,
            IFieldGroupRepository groupRepository,
            IUnitTypeFieldRepository fieldRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<AssignFieldToGroupCommandHandler> logger)
        {
            _linkRepository = linkRepository;
            _groupRepository = groupRepository;
            _fieldRepository = fieldRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(AssignFieldToGroupCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة أمر إسناد الحقل {FieldId} إلى المجموعة {GroupId}", request.FieldId, request.GroupId);

            // التحقق من المعرفات
            if (!Guid.TryParse(request.FieldId, out var fieldId))
                throw new BusinessRuleException("InvalidFieldId", "معرف الحقل غير صالح");
            if (!Guid.TryParse(request.GroupId, out var groupId))
                throw new BusinessRuleException("InvalidGroupId", "معرف المجموعة غير صالح");

            // التحقق من وجود المجموعة
            var group = await _groupRepository.GetFieldGroupByIdAsync(groupId, cancellationToken);
            if (group == null)
                throw new NotFoundException("FieldGroup", request.GroupId);

            // التحقق من وجود الحقل
            var field = await _fieldRepository.GetUnitTypeFieldByIdAsync(fieldId, cancellationToken);
            if (field == null)
                throw new NotFoundException("UnitTypeField", request.FieldId);

            // التحقق من التوافق بين الحقل والمجموعة
            if (field.UnitTypeId != group.UnitTypeId)
                throw new BusinessRuleException("OwnershipMismatch", "الحقل أو المجموعة لا تنتمي لنفس نوع الكيان");

            // التحقق من صلاحيات المستخدم
            if (_currentUserService.Role != "Admin")
                throw new ForbiddenException("غير مصرح لك بإسناد الحقل للمجموعة");

            // تنفيذ الإسناد أو التحديث ضمن معاملة
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // التحقق من وجود ارتباط مسبق
                var existingLink = (await _linkRepository.FindAsync(l => l.GroupId == groupId && l.FieldId == fieldId, cancellationToken)).FirstOrDefault();

                if (existingLink != null)
                {
                    // تحديث ترتيب الحقل
                    existingLink.SortOrder = request.SortOrder;
                    existingLink.UpdatedBy = _currentUserService.UserId;
                    existingLink.UpdatedAt = DateTime.UtcNow;
                    await _linkRepository.UpdateAsync(existingLink, cancellationToken);
                }
                else
                {
                    // إنشاء ارتباط جديد
                    var link = new FieldGroupField
                    {
                        FieldId = fieldId,
                        GroupId = groupId,
                        SortOrder = request.SortOrder,
                        CreatedBy = _currentUserService.UserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _linkRepository.AssignFieldToGroupAsync(link, cancellationToken);
                }

                // تسجيل التدقيق
                await _auditService.LogActivityAsync(
                    "FieldGroupField",
                    fieldId.ToString(),
                    "Assign",
                    $"تم إسناد الحقل {field.FieldName} للمجموعة {group.GroupName} بترتيب {request.SortOrder}",
                    null,
                    null,
                    cancellationToken);

                // نشر الحدث
                // await _eventPublisher.PublishEventAsync(new FieldAssignedToGroupEvent
                // {
                //     FieldId = fieldId,
                //     GroupId = groupId,
                //     SortOrder = request.SortOrder,
                //     AssignedBy = _currentUserService.UserId,
                //     AssignedAt = DateTime.UtcNow
                // }, cancellationToken);

                _logger.LogInformation("تم إسناد الحقل بنجاح: {FieldId} -> {GroupId}", fieldId, groupId);
            });

            return ResultDto<bool>.Ok(true);
        }
    }

    /// <summary>
    /// حدث إسناد حقل إلى مجموعة
    /// Field assigned to group event
    /// </summary>
    public class FieldAssignedToGroupEvent
    {
        public Guid FieldId { get; set; }
        public Guid GroupId { get; set; }
        public int SortOrder { get; set; }
        public Guid AssignedBy { get; set; }
        public DateTime AssignedAt { get; set; }
    }
} 