using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;
using FluentValidation;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands
{
    public class ReorderComponentsCommand : IRequest<ResultDto<bool>>
    {
        public Guid SectionId { get; set; }
        public List<ComponentOrderDto> Components { get; set; }
    }

    public class ComponentOrderDto
    {
        public Guid ComponentId { get; set; }
        public int NewOrder { get; set; }
    }

    public class ReorderComponentsCommandValidator : AbstractValidator<ReorderComponentsCommand>
    {
        public ReorderComponentsCommandValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required");

            RuleFor(x => x.Components)
                .NotEmpty().WithMessage("Components list is required")
                .Must(components => components.Select(c => c.NewOrder).Distinct().Count() == components.Count)
                .WithMessage("All component orders must be unique");
        }
    }

    public class ReorderComponentsCommandHandler : IRequestHandler<ReorderComponentsCommand, ResultDto<bool>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public ReorderComponentsCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<bool>> Handle(ReorderComponentsCommand request, CancellationToken cancellationToken)
        {
            var section = await _repository.GetSectionWithComponentsAsync(request.SectionId, cancellationToken);
            
            if (section == null)
                throw new NotFoundException(nameof(HomeScreenSection), request.SectionId.ToString());

            var components = section.Components.ToList();
            
            // Validate all component IDs exist
            var componentIds = components.Select(c => c.Id).ToHashSet();
            var requestedIds = request.Components.Select(c => c.ComponentId).ToHashSet();
            
            if (!requestedIds.IsSubsetOf(componentIds))
                throw new YemenBooking.Application.Exceptions.ValidationException("One or more component IDs are invalid");

            // Update component orders
            foreach (var componentOrder in request.Components)
            {
                var component = components.First(c => c.Id == componentOrder.ComponentId);
                component.UpdateOrder(componentOrder.NewOrder);
                component.UpdatedBy = _currentUserService.UserId;
            }

            await _repository.UpdateComponentsAsync(components, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<bool>.Ok(true);
        }
    }
}