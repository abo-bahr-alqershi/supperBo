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
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.UnitTypeFields
{
    /// <summary>
    /// معالج أمر إعادة ترتيب الحقول ضمن مجموعة
    /// </summary>
    public class ReorderFieldsInGroupCommandHandler : IRequestHandler<ReorderFieldsInGroupCommand, ResultDto<bool>>
    {
        private readonly IFieldGroupFieldRepository _linkRepository;
        private readonly IFieldGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<ReorderFieldsInGroupCommandHandler> _logger;

        public ReorderFieldsInGroupCommandHandler(
            IFieldGroupFieldRepository linkRepository,
            IFieldGroupRepository groupRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<ReorderFieldsInGroupCommandHandler> logger)
        {
            _linkRepository = linkRepository;
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ReorderFieldsInGroupCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إعادة ترتيب الحقول ضمن المجموعة: GroupId={GroupId}", request.GroupId);

            if (!Guid.TryParse(request.GroupId, out var groupId))
                throw new BusinessRuleException("InvalidGroupId", "معرف المجموعة غير صالح");

            var group = await _groupRepository.GetFieldGroupByIdAsync(groupId, cancellationToken);
            if (group == null)
                throw new NotFoundException("FieldGroup", request.GroupId);

            var fieldIds = request.FieldIds.Select(fid =>
            {
                if (!Guid.TryParse(fid, out var parsed))
                    throw new BusinessRuleException("InvalidFieldId", $"معرف الحقل غير صالح: {fid}");
                return parsed;
            }).ToList();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingLinks = (await _linkRepository.GetFieldsByGroupIdAsync(groupId, cancellationToken)).ToList();
                for (int i = 0; i < fieldIds.Count; i++)
                {
                    var fid = fieldIds[i];
                    var link = existingLinks.FirstOrDefault(l => l.FieldId == fid);
                    if (link == null) continue;
                    link.SortOrder = i;
                    link.UpdatedBy = _currentUserService.UserId;
                    link.UpdatedAt = DateTime.UtcNow;
                    await _linkRepository.UpdateAsync(link, cancellationToken);
                }

                await _auditService.LogBusinessOperationAsync(
                    "ReorderFieldsInGroup",
                    $"تم إعادة ترتيب الحقول في المجموعة {group.GroupName}",
                    groupId,
                    "FieldGroupField",
                    _currentUserService.UserId,
                    null,
                    cancellationToken);

                // await _eventPublisher.PublishEventAsync(new FieldsReorderedInGroupEvent
                // {
                //     GroupId = groupId,
                //     OrderedFieldIds = fieldIds,
                //     ReorderedBy = _currentUserService.UserId,
                //     ReorderedAt = DateTime.UtcNow
                // }, cancellationToken);

                _logger.LogInformation("اكتمل إعادة ترتيب الحقول بنجاح: Count={Count}", fieldIds.Count);
            });

            return ResultDto<bool>.Succeeded(true, "تم إعادة ترتيب الحقول بنجاح");
        }
    }

    /// <summary>
    /// حدث إعادة ترتيب الحقول ضمن مجموعة
    /// Event for fields reordered in a group
    /// </summary>
    public class FieldsReorderedInGroupEvent
    {
        public Guid GroupId { get; set; }
        public List<Guid> OrderedFieldIds { get; set; }
        public Guid ReorderedBy { get; set; }
        public DateTime ReorderedAt { get; set; }
    }
} 