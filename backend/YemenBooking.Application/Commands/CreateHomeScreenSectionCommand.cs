using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using AutoMapper;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Commands
{
    public class CreateHomeScreenSectionCommand : IRequest<ResultDto<HomeScreenSectionDto>>
    {
        public Guid TemplateId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int Order { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string Padding { get; set; }
        public string Margin { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public string CustomStyles { get; set; }
        public string Conditions { get; set; }
    }

    public class CreateHomeScreenSectionCommandHandler : IRequestHandler<CreateHomeScreenSectionCommand, ResultDto<HomeScreenSectionDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateHomeScreenSectionCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultDto<HomeScreenSectionDto>> Handle(CreateHomeScreenSectionCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateByIdAsync(request.TemplateId, cancellationToken);
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId.ToString());

            var section = new HomeScreenSection(
                request.TemplateId,
                request.Name,
                request.Title,
                request.Order,
                request.BackgroundColor,
                request.Padding ?? "16",
                request.Margin ?? "0");

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

            section.CreatedBy = _currentUserService.UserId;

            await _repository.AddSectionAsync(section, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<HomeScreenSectionDto>.Ok(_mapper.Map<HomeScreenSectionDto>(section));
        }
    }
}