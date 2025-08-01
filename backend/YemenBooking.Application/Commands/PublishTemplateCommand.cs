using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using System.Linq;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands
{
    public class PublishTemplateCommand : IRequest<ResultDto<bool>>
    {
        public Guid TemplateId { get; set; }
        public bool DeactivateOthers { get; set; }
    }

    public class PublishTemplateCommandHandler : IRequestHandler<PublishTemplateCommand, ResultDto<bool>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public PublishTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<bool>> Handle(PublishTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateByIdAsync(request.TemplateId, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId.ToString());

            // Validate template has at least one section
            var sections = await _repository.GetTemplateSectionsAsync(request.TemplateId, cancellationToken);
            if (!sections.Any())
                throw new ValidationException("Cannot publish template without sections");

            // Deactivate other templates if requested
            if (request.DeactivateOthers)
            {
                var activeTemplates = await _repository.GetActiveTemplatesAsync(
                    template.Platform, 
                    template.TargetAudience, 
                    cancellationToken);

                foreach (var activeTemplate in activeTemplates.Where(t => t.Id != request.TemplateId))
                {
                    activeTemplate.Unpublish();
                    activeTemplate.UpdatedBy = _currentUserService.UserId;
                    await _repository.UpdateTemplateAsync(activeTemplate, cancellationToken);
                }
            }

            template.Publish(_currentUserService.UserId);
            await _repository.UpdateTemplateAsync(template, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<bool>.Ok(true);
        }
    }
}