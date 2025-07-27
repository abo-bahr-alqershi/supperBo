using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.FieldGroups;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.FieldGroups
{
    /// <summary>
    /// معالج استعلام جلب مجموعة الحقول حسب المعرف
    /// Query handler for GetFieldGroupByIdQuery
    /// </summary>
    public class GetFieldGroupByIdQueryHandler : IRequestHandler<GetFieldGroupByIdQuery, FieldGroupDto>
    {
        private readonly IFieldGroupRepository _groupRepository;
        private readonly ILogger<GetFieldGroupByIdQueryHandler> _logger;

        public GetFieldGroupByIdQueryHandler(
            IFieldGroupRepository groupRepository,
            ILogger<GetFieldGroupByIdQueryHandler> logger)
        {
            _groupRepository = groupRepository;
            _logger = logger;
        }

        public async Task<FieldGroupDto> Handle(GetFieldGroupByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام جلب مجموعة الحقول حسب المعرف: {GroupId}", request.GroupId);

            if (!Guid.TryParse(request.GroupId, out var id))
                throw new ValidationException(nameof(request.GroupId), "معرف المجموعة غير صالح");

            var group = await _groupRepository.GetFieldGroupByIdAsync(id, cancellationToken);
            if (group == null)
                throw new NotFoundException("FieldGroup", request.GroupId, $"مجموعة الحقول بالمعرف {request.GroupId} غير موجودة");

            return new FieldGroupDto
            {
                GroupId = group.Id.ToString(),
                UnitTypeId = group.UnitTypeId.ToString(),
                GroupName = group.GroupName,
                DisplayName = group.DisplayName,
                Description = group.Description,
                SortOrder = group.SortOrder,
                IsCollapsible = group.IsCollapsible,
                IsExpandedByDefault = group.IsExpandedByDefault
            };
        }
    }
} 