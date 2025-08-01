using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using AutoMapper;

namespace YemenBooking.Application.Queries
{
    public class GetHomeScreenTemplateByIdQuery : IRequest<ResultDto<HomeScreenTemplateDto>>
    {
        public Guid TemplateId { get; set; }
        public bool IncludeHierarchy { get; set; }
    }

    public class GetHomeScreenTemplateByIdQueryHandler : IRequestHandler<GetHomeScreenTemplateByIdQuery, ResultDto<HomeScreenTemplateDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly IMapper _mapper;

        public GetHomeScreenTemplateByIdQueryHandler(
            IHomeScreenRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultDto<HomeScreenTemplateDto>> Handle(GetHomeScreenTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var template = request.IncludeHierarchy
                ? await _repository.GetTemplateWithFullHierarchyAsync(request.TemplateId, cancellationToken)
                : await _repository.GetTemplateByIdAsync(request.TemplateId, cancellationToken);

            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId.ToString());

            return ResultDto<HomeScreenTemplateDto>.Ok(_mapper.Map<HomeScreenTemplateDto>(template));
        }
    }
}