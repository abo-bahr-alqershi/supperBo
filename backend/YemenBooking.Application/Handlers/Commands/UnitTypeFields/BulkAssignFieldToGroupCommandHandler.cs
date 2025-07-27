using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.UnitTypeFields;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Handlers.Commands.UnitTypeFields
{
    /// <summary>
    /// معالج أمر bulk assign للحقول لمجموعة وحدة واحدة
    /// Handler for BulkAssignFieldToGroupCommand
    /// </summary>
    public class BulkAssignFieldToGroupCommandHandler : IRequestHandler<BulkAssignFieldToGroupCommand, ResultDto<bool>>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BulkAssignFieldToGroupCommandHandler> _logger;

        public BulkAssignFieldToGroupCommandHandler(IMediator mediator, ILogger<BulkAssignFieldToGroupCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(BulkAssignFieldToGroupCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bulk assigning {Count} fields to group {GroupId}", request.FieldIds.Count, request.GroupId);

            // Delegate to existing AssignFieldsToGroupCommand
            var assignCommand = new AssignFieldsToGroupCommand
            {
                GroupId = request.GroupId,
                FieldIds = request.FieldIds
            };

            var result = await _mediator.Send(assignCommand, cancellationToken);
            return result;
        }
    }
} 