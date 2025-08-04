using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.HomeSections.DynamicHomeConfig;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.HomeSections.DynamicHomeConfig
{
    public class PublishDynamicHomeConfigCommandHandler : IRequestHandler<PublishDynamicHomeConfigCommand, bool>
    {
        private readonly IRepository<DynamicHomeConfig> _repository;

        public PublishDynamicHomeConfigCommandHandler(IRepository<DynamicHomeConfig> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(PublishDynamicHomeConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) return false;

            entity.Publish();
            await _repository.UpdateAsync(entity, cancellationToken);
            return true;
        }
    }
}