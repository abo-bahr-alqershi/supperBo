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
    /// معالج أمر إسناد عدة حقول إلى مجموعة
    /// </summary>
    public class AssignFieldsToGroupCommandHandler : IRequestHandler<AssignFieldsToGroupCommand, ResultDto<bool>>
    {
        private readonly IFieldGroupFieldRepository _linkRepository;
        private readonly IFieldGroupRepository _groupRepository;
        private readonly IUnitTypeFieldRepository _fieldRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<AssignFieldsToGroupCommandHandler> _logger;

        public AssignFieldsToGroupCommandHandler(
            IFieldGroupFieldRepository linkRepository,
            IFieldGroupRepository groupRepository,
            IUnitTypeFieldRepository fieldRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<AssignFieldsToGroupCommandHandler> logger)
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

        public async Task<ResultDto<bool>> Handle(AssignFieldsToGroupCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إسناد عدة حقول إلى المجموعة: GroupId={GroupId}, FieldCount={Count}", request.GroupId, request.FieldIds.Count);

            if (!Guid.TryParse(request.GroupId, out var groupId))
                throw new BusinessRuleException("InvalidGroupId", "معرف المجموعة غير صالح");

            var group = await _groupRepository.GetFieldGroupByIdAsync(groupId, cancellationToken);
            if (group == null)
                throw new NotFoundException("FieldGroup", request.GroupId);

            if (_currentUserService.Role != "Admin")
                throw new ForbiddenException("غير مصرح لك بإسناد الحقول للمجموعة");

            var fieldIds = request.FieldIds.Select(fid =>
            {
                if (!Guid.TryParse(fid, out var parsed))
                    throw new BusinessRuleException("InvalidFieldId", $"معرف الحقل غير صالح: {fid}");
                return parsed;
            }).ToList();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                int sortOrder = 0;
                foreach (var fieldId in fieldIds)
                {
                    var field = await _fieldRepository.GetUnitTypeFieldByIdAsync(fieldId, cancellationToken);
                    if (field == null)
                        throw new NotFoundException("UnitTypeField", fieldId.ToString());
                    if (field.UnitTypeId != group.UnitTypeId)
                        throw new BusinessRuleException("OwnershipMismatch", "الحقل والمجموعة لا ينتميان لنفس نوع الكيان");

                    var existingLink = (await _linkRepository.FindAsync(l => l.GroupId == groupId && l.FieldId == fieldId, cancellationToken)).FirstOrDefault();
                    if (existingLink != null)
                    {
                        existingLink.SortOrder = sortOrder;
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
                            SortOrder = sortOrder,
                            CreatedBy = _currentUserService.UserId,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _linkRepository.AssignFieldToGroupAsync(link, cancellationToken);
                    }
                    sortOrder++;
                }

                await _auditService.LogBusinessOperationAsync(
                    "AssignMultipleFieldsToGroup",
                    $"تم إسناد {request.FieldIds.Count} حقول إلى المجموعة {group.GroupName}",
                    groupId,
                    "FieldGroupField",
                    _currentUserService.UserId,
                    null,
                    cancellationToken);

                // await _eventPublisher.PublishEventAsync(new MultipleFieldsAssignedToGroupEvent
                // {
                //     GroupId = groupId,
                //     FieldIds = fieldIds,
                //     AssignedBy = _currentUserService.UserId,
                //     AssignedAt = DateTime.UtcNow
                // }, cancellationToken);

                _logger.LogInformation("اكتمل إسناد الحقول إلى المجموعة بنجاح: Count={Count}", fieldIds.Count);
            });

            return ResultDto<bool>.Succeeded(true, "تم إسناد الحقول إلى المجموعة بنجاح");
        }
    }

    /// <summary>
    /// حدث إسناد عدة حقول إلى مجموعة
    /// Event for multiple fields assigned to a group
    /// </summary>
    public class MultipleFieldsAssignedToGroupEvent
    {
        public Guid GroupId { get; set; }
        public List<Guid> FieldIds { get; set; }
        public Guid AssignedBy { get; set; }
        public DateTime AssignedAt { get; set; }
    }
} 