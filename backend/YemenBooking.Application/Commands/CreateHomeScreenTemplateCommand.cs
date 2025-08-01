using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.DTOs;
using AutoMapper;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Commands
{
    public class CreateHomeScreenTemplateCommand : IRequest<ResultDto<HomeScreenTemplateDto>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
        public string TargetAudience { get; set; }
        public string MetaData { get; set; }
    }

    public class CreateHomeScreenTemplateCommandHandler : IRequestHandler<CreateHomeScreenTemplateCommand, ResultDto<HomeScreenTemplateDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateHomeScreenTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultDto<HomeScreenTemplateDto>> Handle(CreateHomeScreenTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new HomeScreenTemplate(
                request.Name,
                request.Description,
                request.Version,
                request.Platform,
                request.TargetAudience,
                request.MetaData);

            template.CreatedBy = _currentUserService.UserId;

            await _repository.AddTemplateAsync(template, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<HomeScreenTemplateDto>.Ok(_mapper.Map<HomeScreenTemplateDto>(template));
        }
    }
}