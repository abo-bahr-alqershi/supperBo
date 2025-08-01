using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands
{
    public class DeleteHomeScreenTemplateCommand : IRequest<ResultDto<bool>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteHomeScreenTemplateCommandHandler : IRequestHandler<DeleteHomeScreenTemplateCommand, ResultDto<bool>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteHomeScreenTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<bool>> Handle(DeleteHomeScreenTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateByIdAsync(request.Id, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.Id.ToString());

            if (template.IsActive)
                throw new ConflictException("", "Cannot delete an active template. Please deactivate it first.");

            template.IsDeleted = true;
            template.DeletedBy = _currentUserService.UserId;
            template.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateTemplateAsync(template, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<bool>.Ok(true);
        }
    }
}