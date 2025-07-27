using System;
using System.Threading;
using System.Threading.Tasks;
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
    /// معالج أمر إزالة حقل من مجموعة
    /// </summary>
    public class RemoveFieldFromGroupCommandHandler : IRequestHandler<RemoveFieldFromGroupCommand, ResultDto<bool>>
    {
        private readonly IFieldGroupFieldRepository _linkRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<RemoveFieldFromGroupCommandHandler> _logger;

        public RemoveFieldFromGroupCommandHandler(
            IFieldGroupFieldRepository linkRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<RemoveFieldFromGroupCommandHandler> logger)
        {
            _linkRepository = linkRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(RemoveFieldFromGroupCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إزالة الحقل {FieldId} من المجموعة {GroupId}", request.FieldId, request.GroupId);

            if (!Guid.TryParse(request.FieldId, out var fieldId))
                throw new BusinessRuleException("InvalidFieldId", "معرف الحقل غير صالح");
            if (!Guid.TryParse(request.GroupId, out var groupId))
                throw new BusinessRuleException("InvalidGroupId", "معرف المجموعة غير صالح");

            bool exists = await _linkRepository.GroupHasFieldAsync(groupId, fieldId, cancellationToken);
            if (!exists)
                return ResultDto<bool>.Failed("لا يوجد ارتباط للحقل بالمجموعة");

            var success = await _linkRepository.RemoveFieldFromGroupAsync(fieldId, groupId, cancellationToken);
            if (!success)
                return ResultDto<bool>.Failed("فشل إزالة الحقل من المجموعة");

            await _auditService.LogActivityAsync(
                "FieldGroupField",
                fieldId.ToString(),
                "Remove",
                $"تم إزالة الحقل {request.FieldId} من المجموعة {request.GroupId}",
                null,
                null,
                cancellationToken);

            // await _eventPublisher.PublishEventAsync(new FieldRemovedFromGroupEvent
            // {
            //     FieldId = fieldId,
            //     GroupId = groupId,
            //     RemovedBy = _currentUserService.UserId,
            //     RemovedAt = DateTime.UtcNow
            // }, cancellationToken);

            _logger.LogInformation("اكتمل إزالة الحقل من المجموعة بنجاح: FieldId={FieldId}, GroupId={GroupId}", fieldId, groupId);
            return ResultDto<bool>.Succeeded(true, "تمت إزالة الحقل من المجموعة بنجاح");
        }
    }

    /// <summary>
    /// حدث إزالة حقل من مجموعة
    /// Event for field removed from a group
    /// </summary>
    public class FieldRemovedFromGroupEvent
    {
        public Guid FieldId { get; set; }
        public Guid GroupId { get; set; }
        public Guid RemovedBy { get; set; }
        public DateTime RemovedAt { get; set; }
    }
} 