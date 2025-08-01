using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;
using FluentValidation;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands
{
    public class ReorderSectionsCommand : IRequest<ResultDto<bool>>
    {
        public Guid TemplateId { get; set; }
        public List<SectionOrderDto> Sections { get; set; }
    }

    public class SectionOrderDto
    {
        public Guid SectionId { get; set; }
        public int NewOrder { get; set; }
    }

    public class ReorderSectionsCommandValidator : AbstractValidator<ReorderSectionsCommand>
    {
        public ReorderSectionsCommandValidator()
        {
            RuleFor(x => x.TemplateId)
                .NotEmpty().WithMessage("Template ID is required");

            RuleFor(x => x.Sections)
                .NotEmpty().WithMessage("Sections list is required")
                .Must(sections => sections.Select(s => s.NewOrder).Distinct().Count() == sections.Count)
                .WithMessage("All section orders must be unique");
        }
    }

    public class ReorderSectionsCommandHandler : IRequestHandler<ReorderSectionsCommand, ResultDto<bool>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public ReorderSectionsCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<bool>> Handle(ReorderSectionsCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateWithSectionsAsync(request.TemplateId, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId.ToString());
            
            var sections = template.Sections.ToList();
            
            // Validate all section IDs exist
            var sectionIds = sections.Select(s => s.Id).ToHashSet();
            var requestedIds = request.Sections.Select(s => s.SectionId).ToHashSet();
            
            if (!requestedIds.IsSubsetOf(sectionIds))
                throw new YemenBooking.Application.Exceptions.ValidationException("One or more section IDs are invalid");

            // Update section orders
            foreach (var sectionOrder in request.Sections)
            {
                var section = sections.First(s => s.Id == sectionOrder.SectionId);
                section.UpdateOrder(sectionOrder.NewOrder);
                section.UpdatedBy = _currentUserService.UserId;
            }

            await _repository.UpdateSectionsAsync(sections, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<bool>.Ok(true);
        }
    }
}