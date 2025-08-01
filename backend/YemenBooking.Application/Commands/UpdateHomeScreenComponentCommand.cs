using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.DTOs;
using FluentValidation;
using AutoMapper;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Commands
{
    public class UpdateHomeScreenComponentCommand : IRequest<ResultDto<HomeScreenComponentDto>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string Alignment { get; set; }
        public string CustomClasses { get; set; }
        public string AnimationType { get; set; }
        public int AnimationDuration { get; set; }
        public string Conditions { get; set; }
    }

    public class UpdateHomeScreenComponentCommandValidator : AbstractValidator<UpdateHomeScreenComponentCommand>
    {
        public UpdateHomeScreenComponentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Component ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Component name is required")
                .MaximumLength(100).WithMessage("Component name must not exceed 100 characters");

            RuleFor(x => x.ColSpan)
                .InclusiveBetween(1, 12).WithMessage("Column span must be between 1 and 12");

            RuleFor(x => x.RowSpan)
                .InclusiveBetween(1, 10).WithMessage("Row span must be between 1 and 10");

            RuleFor(x => x.AnimationDuration)
                .InclusiveBetween(0, 5000).WithMessage("Animation duration must be between 0 and 5000 ms");
        }
    }

    public class UpdateHomeScreenComponentCommandHandler : IRequestHandler<UpdateHomeScreenComponentCommand, ResultDto<HomeScreenComponentDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateHomeScreenComponentCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultDto<HomeScreenComponentDto>> Handle(UpdateHomeScreenComponentCommand request, CancellationToken cancellationToken)
        {
            var component = await _repository.GetComponentByIdAsync(request.Id, cancellationToken);
            
            if (component == null)
                throw new NotFoundException(nameof(HomeScreenComponent), request.Id.ToString());

            component.Update(
                request.Name,
                request.ColSpan,
                request.RowSpan,
                request.Alignment,
                request.CustomClasses,
                request.AnimationType,
                request.AnimationDuration,
                request.Conditions);

            component.UpdatedBy = _currentUserService.UserId;

            await _repository.UpdateComponentAsync(component, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<HomeScreenComponentDto>.Ok(_mapper.Map<HomeScreenComponentDto>(component));
        }
    }
}