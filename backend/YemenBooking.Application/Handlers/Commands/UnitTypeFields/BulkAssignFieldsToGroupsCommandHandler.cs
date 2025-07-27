using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
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
    /// معالج أمر الإسناد الجماعي للحقول إلى المجموعات
    /// </summary>
    public class BulkAssignFieldsToGroupsCommandHandler : IRequestHandler<BulkAssignFieldsToGroupsCommand, ResultDto<bool>>
    {
        private readonly IFieldGroupFieldRepository _linkRepository;
        private readonly IFieldGroupRepository _groupRepository;
        private readonly IUnitTypeFieldRepository _fieldRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<BulkAssignFieldsToGroupsCommandHandler> _logger;

        public BulkAssignFieldsToGroupsCommandHandler(
            IFieldGroupFieldRepository linkRepository,
            IFieldGroupRepository groupRepository,
            IUnitTypeFieldRepository fieldRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<BulkAssignFieldsToGroupsCommandHandler> logger)
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

        public async Task<ResultDto<bool>> Handle(BulkAssignFieldsToGroupsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء الإسناد الجماعي للحقول للمجموعات: Count={Count}", request.Assignments.Count);

            if (request.Assignments == null || !request.Assignments.Any())
                return ResultDto<bool>.Failed("يجب تحديد عمليات الإسناد");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var assignment in request.Assignments)
                {
                    if (!Guid.TryParse(assignment.FieldId, out var fieldId))
                        throw new BusinessRuleException("InvalidFieldId", $"معرف الحقل غير صالح: {assignment.FieldId}");
                    if (!Guid.TryParse(assignment.GroupId, out var groupId))
                        throw new BusinessRuleException("InvalidGroupId", $"معرف المجموعة غير صالح: {assignment.GroupId}");

                    var group = await _groupRepository.GetFieldGroupByIdAsync(groupId, cancellationToken);
                    if (group == null)
                        throw new NotFoundException("FieldGroup", assignment.GroupId);

                    var field = await _fieldRepository.GetUnitTypeFieldByIdAsync(fieldId, cancellationToken);
                    if (field == null)
                        throw new NotFoundException("UnitTypeField", assignment.FieldId);

                    if (field.UnitTypeId != group.UnitTypeId)
                        throw new BusinessRuleException("OwnershipMismatch", "الحقل والمجموعة لا ينتميان لنفس نوع الكيان");

                    var existingLink = (await _linkRepository.FindAsync(l => l.GroupId == groupId && l.FieldId == fieldId, cancellationToken)).FirstOrDefault();
                    if (existingLink != null)
                    {
                        existingLink.SortOrder = assignment.SortOrder;
                        existingLink.UpdatedBy = _currentUserService.UserId;
                        existingLink.UpdatedAt = DateTime.UtcNow;
                        await _linkRepository.UpdateAsync(existingLink, cancellationToken);
                    }
                    else
                    {
                        var link = new FieldGroupField
                        {
                            FieldId = fieldId,
                            GroupId = groupId,
                            SortOrder = assignment.SortOrder,
                            CreatedBy = _currentUserService.UserId,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _linkRepository.AssignFieldToGroupAsync(link, cancellationToken);
                    }
                }

                await _auditService.LogBusinessOperationAsync(
                    "BulkAssignFieldsToGroups",
                    $"تم الإسناد الجماعي للحقول للمجموعات: {request.Assignments.Count}",
                    null,
                    "FieldGroupField",
                    _currentUserService.UserId,
                    null,
                    cancellationToken);

                // await _eventPublisher.PublishEventAsync(new BulkFieldsAssignedToGroupsEvent
                // {
                //     Assignments = request.Assignments.Select(a => new FieldAssignmentResult
                //     {
                //         FieldId = Guid.Parse(a.FieldId),
                //         GroupId = Guid.Parse(a.GroupId),
                //         SortOrder = a.SortOrder
                //     }).ToList(),
                //     PerformedBy = _currentUserService.UserId,
                //     PerformedAt = DateTime.UtcNow
                // }, cancellationToken);

                _logger.LogInformation("اكتمل الإسناد الجماعي للحقول للمجموعات بنجاح");
            });

            return ResultDto<bool>.Succeeded(true, "تم الإسناد الجماعي للحقول للمجموعات بنجاح");
        }
    }

    /// <summary>
    /// حدث الإسناد الجماعي للحقول للمجموعات
    /// Event for bulk fields assigned to groups
    /// </summary>
    public class BulkFieldsAssignedToGroupsEvent
    {
        public List<FieldAssignmentResult> Assignments { get; set; }
        public Guid PerformedBy { get; set; }
        public DateTime PerformedAt { get; set; }
    }

    /// <summary>
    /// نتيجة عملية إسناد حقل لمجموعة
    /// Result model for a field-to-group assignment
    /// </summary>
    public class FieldAssignmentResult
    {
        public Guid FieldId { get; set; }
        public Guid GroupId { get; set; }
        public int SortOrder { get; set; }
    }
} 