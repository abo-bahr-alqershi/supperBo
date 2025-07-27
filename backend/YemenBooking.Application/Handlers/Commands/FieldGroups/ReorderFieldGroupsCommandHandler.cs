using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.FieldGroups;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using System.Collections.Generic;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.FieldGroups
{
    /// <summary>
    /// معالج أمر إعادة ترتيب مجموعات الحقول
    /// Reorders field groups and includes:
    /// - Input validation
    /// - Existence and ownership check
    /// - Authorization (Admin only)
    /// - Updating sort orders
    /// - Audit logging
    /// - Event publishing
    /// </summary>
    public class ReorderFieldGroupsCommandHandler : IRequestHandler<ReorderFieldGroupsCommand, ResultDto<bool>>
    {
        private readonly IFieldGroupRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<ReorderFieldGroupsCommandHandler> _logger;

        public ReorderFieldGroupsCommandHandler(
            IFieldGroupRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<ReorderFieldGroupsCommandHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ReorderFieldGroupsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة أمر إعادة ترتيب مجموعات الحقول لنوع الكيان: {PropertyTypeId}", request.PropertyTypeId);

            // التحقق من صحة معرف نوع الكيان
            if (!Guid.TryParse(request.PropertyTypeId, out var propertyTypeId))
                throw new BusinessRuleException("InvalidPropertyTypeId", "معرف نوع الكيان غير صالح");

            // صلاحيات المستخدم
            if (_currentUserService.Role != "Admin")
                throw new ForbiddenException("غير مصرح لك بإعادة ترتيب مجموعات الحقول");

            // التحقق من الطلبات
            if (request.GroupOrders == null || !request.GroupOrders.Any())
                throw new BusinessRuleException("EmptyOrders", "يجب أن يحتوي طلب إعادة الترتيب على بيانات" );

            // تنفيذ إعادة الترتيب ضمن معاملة
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var order in request.GroupOrders)
                {
                    if (order.GroupId == Guid.Empty)
                        throw new BusinessRuleException("InvalidGroupId", "معرف المجموعة غير صالح");

                    var group = await _repository.GetFieldGroupByIdAsync(order.GroupId, cancellationToken);
                    if (group == null)
                        throw new NotFoundException("FieldGroup", order.GroupId.ToString());

                    if (group.UnitTypeId != propertyTypeId)
                        throw new BusinessRuleException("GroupOwnershipMismatch", "المجموعة لا تنتمي لنوع الكيان المحدد");

                    group.SortOrder = order.SortOrder;
                    await _repository.UpdateFieldGroupAsync(group, cancellationToken);
                }

                // تسجيل التدقيق
                var details = string.Join(", ", request.GroupOrders
                    .Select(o => $"{o.GroupId}:{o.SortOrder}"));
                await _auditService.LogActivityAsync(
                    "FieldGroup",
                    propertyTypeId.ToString(),
                    "Reorder",
                    $"تم إعادة ترتيب مجموعات الحقول: {details}",
                    null,
                    null,
                    cancellationToken);

                // نشر الحدث
                // await _eventPublisher.PublishEventAsync(new FieldGroupsReorderedEvent
                // {
                //     PropertyTypeId = propertyTypeId,
                //     GroupOrders = request.GroupOrders
                //         .Select(o => new GroupOrderDto { GroupId = o.GroupId, SortOrder = o.SortOrder })
                //         .ToList(),
                //     ExecutedBy = _currentUserService.UserId,
                //     ExecutedAt = DateTime.UtcNow
                // }, cancellationToken);

                _logger.LogInformation("تم إعادة ترتيب مجموعات الحقول بنجاح لنوع الكيان: {PropertyTypeId}", propertyTypeId);
            });

            return ResultDto<bool>.Ok(true, "تم إعادة ترتيب مجموعات الحقول بنجاح");
        }
    }

    /// <summary>
    /// حدث إعادة ترتيب مجموعات الحقول
    /// Field groups reordered event
    /// </summary>
    public class FieldGroupsReorderedEvent
    {
        public Guid PropertyTypeId { get; set; }
        public List<GroupOrderDto> GroupOrders { get; set; } = new();
        public Guid ExecutedBy { get; set; }
        public DateTime ExecutedAt { get; set; }
    }
} 