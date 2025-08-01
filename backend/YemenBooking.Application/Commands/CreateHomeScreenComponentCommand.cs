using YemenBooking.Core.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Linq;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands
{
    public class CreateHomeScreenComponentCommand : IRequest<ResultDto<Guid>>
    {
        public Guid SectionId { get; set; }
        public string ComponentType { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int ColSpan { get; set; } = 12;
        public int RowSpan { get; set; } = 1;
        public string Alignment { get; set; } = "left";
        public string CustomClasses { get; set; }
        public string AnimationType { get; set; }
        public int AnimationDuration { get; set; }
        public string Conditions { get; set; }
    }

    public class CreateHomeScreenComponentCommandValidator : AbstractValidator<CreateHomeScreenComponentCommand>
    {
        public CreateHomeScreenComponentCommandValidator()
        {
            RuleFor(x => x.ComponentType)
                .NotEmpty().WithMessage("Component type is required")
                .MaximumLength(50).WithMessage("Component type must not exceed 50 characters");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Component name is required")
                .MaximumLength(100).WithMessage("Component name must not exceed 100 characters");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Order must be non-negative");

            RuleFor(x => x.ColSpan)
                .InclusiveBetween(1, 12).WithMessage("Column span must be between 1 and 12");

            RuleFor(x => x.RowSpan)
                .InclusiveBetween(1, 10).WithMessage("Row span must be between 1 and 10");
        }
    }

    public class CreateHomeScreenComponentCommandHandler : IRequestHandler<CreateHomeScreenComponentCommand, ResultDto<Guid>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public CreateHomeScreenComponentCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<Guid>> Handle(CreateHomeScreenComponentCommand request, CancellationToken cancellationToken)
        {
            var section = await _repository.GetSectionByIdAsync(request.SectionId, cancellationToken);
            if (section == null)
                throw new NotFoundException(nameof(HomeScreenSection), request.SectionId.ToString());

            var component = new HomeScreenComponent(
                request.SectionId,
                request.ComponentType,
                request.Name,
                request.Order,
                request.ColSpan,
                request.RowSpan);

            component.Update(
                request.Name,
                request.ColSpan,
                request.RowSpan,
                request.Alignment,
                request.CustomClasses,
                request.AnimationType,
                request.AnimationDuration,
                request.Conditions);

            component.CreatedBy = _currentUserService.UserId;

            await _repository.AddComponentAsync(component, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<Guid>.Ok(component.Id);
        }
    }
}