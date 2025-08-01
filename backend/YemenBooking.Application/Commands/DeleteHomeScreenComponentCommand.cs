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
    public class DeleteHomeScreenComponentCommand : IRequest<ResultDto<bool>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteHomeScreenComponentCommandHandler : IRequestHandler<DeleteHomeScreenComponentCommand, ResultDto<bool>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteHomeScreenComponentCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<bool>> Handle(DeleteHomeScreenComponentCommand request, CancellationToken cancellationToken)
        {
            var component = await _repository.GetComponentByIdAsync(request.Id, cancellationToken);
            
            if (component == null)
                throw new NotFoundException(nameof(HomeScreenComponent), request.Id.ToString());

            component.IsDeleted = true;
            component.DeletedBy = _currentUserService.UserId;
            component.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateComponentAsync(component, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<bool>.Ok(true);
        }
    }
}