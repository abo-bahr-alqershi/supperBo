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
    public class UpdateHomeScreenTemplateCommand : IRequest<ResultDto<HomeScreenTemplateDto>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaData { get; set; }
    }

    public class UpdateHomeScreenTemplateCommandValidator : AbstractValidator<UpdateHomeScreenTemplateCommand>
    {
        public UpdateHomeScreenTemplateCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Template ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Template name is required")
                .MaximumLength(200).WithMessage("Template name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
        }
    }

    public class UpdateHomeScreenTemplateCommandHandler : IRequestHandler<UpdateHomeScreenTemplateCommand, ResultDto<HomeScreenTemplateDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateHomeScreenTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultDto<HomeScreenTemplateDto>> Handle(UpdateHomeScreenTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateByIdAsync(request.Id, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.Id.ToString());

            template.Update(request.Name, request.Description, request.MetaData);
            template.UpdatedBy = _currentUserService.UserId;

            await _repository.UpdateTemplateAsync(template, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<HomeScreenTemplateDto>.Ok(_mapper.Map<HomeScreenTemplateDto>(template));
        }
    }
}