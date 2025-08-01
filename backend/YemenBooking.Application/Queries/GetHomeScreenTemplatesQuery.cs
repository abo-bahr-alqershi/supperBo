using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.DTOs;
using AutoMapper;

namespace YemenBooking.Application.Queries
{
    public class GetHomeScreenTemplatesQuery : IRequest<ResultDto<List<HomeScreenTemplateDto>>>
    {
        public string Platform { get; set; }
        public string TargetAudience { get; set; }
        public bool? IsActive { get; set; }
        public bool IncludeDeleted { get; set; }
    }

    public class GetHomeScreenTemplatesQueryHandler : IRequestHandler<GetHomeScreenTemplatesQuery, ResultDto<List<HomeScreenTemplateDto>>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly IMapper _mapper;

        public GetHomeScreenTemplatesQueryHandler(
            IHomeScreenRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultDto<List<HomeScreenTemplateDto>>> Handle(GetHomeScreenTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetTemplatesAsync(
                request.Platform,
                request.TargetAudience,
                request.IsActive,
                request.IncludeDeleted,
                cancellationToken);

            return ResultDto<List<HomeScreenTemplateDto>>.Ok(_mapper.Map<List<HomeScreenTemplateDto>>(templates));
        }
    }
}