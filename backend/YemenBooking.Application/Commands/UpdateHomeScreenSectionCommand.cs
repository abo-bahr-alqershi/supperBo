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
    public class UpdateHomeScreenSectionCommand : IRequest<ResultDto<HomeScreenSectionDto>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string Padding { get; set; }
        public string Margin { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public string CustomStyles { get; set; }
        public string Conditions { get; set; }
    }

    public class UpdateHomeScreenSectionCommandValidator : AbstractValidator<UpdateHomeScreenSectionCommand>
    {
        public UpdateHomeScreenSectionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Section ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Section name is required")
                .MaximumLength(100).WithMessage("Section name must not exceed 100 characters");

            RuleFor(x => x.MinHeight)
                .GreaterThanOrEqualTo(0).WithMessage("Min height must be non-negative");

            RuleFor(x => x.MaxHeight)
                .GreaterThanOrEqualTo(x => x.MinHeight)
                .When(x => x.MaxHeight > 0)
                .WithMessage("Max height must be greater than or equal to min height");
        }
    }

    public class UpdateHomeScreenSectionCommandHandler : IRequestHandler<UpdateHomeScreenSectionCommand, ResultDto<HomeScreenSectionDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateHomeScreenSectionCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultDto<HomeScreenSectionDto>> Handle(UpdateHomeScreenSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _repository.GetSectionByIdAsync(request.Id, cancellationToken);
            
            if (section == null)
                throw new NotFoundException(nameof(HomeScreenSection), request.Id.ToString());

            section.Update(
                request.Name,
                request.Title,
                request.Subtitle,
                request.BackgroundColor,
                request.BackgroundImage,
                request.Padding,
                request.Margin,
                request.MinHeight,
                request.MaxHeight,
                request.CustomStyles,
                request.Conditions);

            section.UpdatedBy = _currentUserService.UserId;

            await _repository.UpdateSectionAsync(section, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<HomeScreenSectionDto>.Ok(_mapper.Map<HomeScreenSectionDto>(section));
        }
    }
}