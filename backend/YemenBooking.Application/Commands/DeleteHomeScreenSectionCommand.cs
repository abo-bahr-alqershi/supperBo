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
    public class DeleteHomeScreenSectionCommand : IRequest<ResultDto<bool>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteHomeScreenSectionCommandHandler : IRequestHandler<DeleteHomeScreenSectionCommand, ResultDto<bool>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteHomeScreenSectionCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<bool>> Handle(DeleteHomeScreenSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _repository.GetSectionByIdAsync(request.Id, cancellationToken);
            
            if (section == null)
                throw new NotFoundException(nameof(HomeScreenSection), request.Id.ToString());

            // Check if section has components
            if (section.Components.Any())
                throw new ConflictException("","Cannot delete section with existing components. Please remove all components first.");

            section.IsDeleted = true;
            section.DeletedBy = _currentUserService.UserId;
            section.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateSectionAsync(section, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<bool>.Ok(true);
        }
    }
}