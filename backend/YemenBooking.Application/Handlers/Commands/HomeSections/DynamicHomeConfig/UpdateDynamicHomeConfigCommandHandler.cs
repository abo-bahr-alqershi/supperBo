using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.HomeSections.DynamicHomeConfig;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.HomeSections.DynamicHomeConfig
{
    public class UpdateDynamicHomeConfigCommandHandler : IRequestHandler<UpdateDynamicHomeConfigCommand, bool>
    {
        private readonly IRepository<YemenBooking.Core.Entities.DynamicHomeConfig> _repository;

        public UpdateDynamicHomeConfigCommandHandler(IRepository<YemenBooking.Core.Entities.DynamicHomeConfig> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateDynamicHomeConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) return false;

            entity.UpdateConfig(
                request.GlobalSettings,
                request.ThemeSettings,
                request.LayoutSettings,
                request.CacheSettings,
                request.AnalyticsSettings,
                request.EnabledFeatures,
                request.ExperimentalFeatures,
                request.Description);

            await _repository.UpdateAsync(entity, cancellationToken);
            return true;
        }
    }
}