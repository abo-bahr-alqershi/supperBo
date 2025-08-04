using MediatR;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.HomeSections.DynamicHomeSections;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.DynamicHomeSections
{
    public class CreateDynamicHomeSectionCommandHandler : IRequestHandler<CreateDynamicHomeSectionCommand, Guid>
    {
        private readonly IRepository<DynamicHomeSection> _sectionRepository;
        private readonly IRepository<DynamicSectionContent> _contentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDynamicHomeSectionCommandHandler(
            IRepository<DynamicHomeSection> sectionRepository,
            IRepository<DynamicSectionContent> contentRepository,
            IUnitOfWork unitOfWork)
        {
            _sectionRepository = sectionRepository;
            _contentRepository = contentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateDynamicHomeSectionCommand request, CancellationToken cancellationToken)
        {
            // Serialize collections to JSON
            var targetAudienceJson = JsonSerializer.Serialize(request.TargetAudience);

            // Create the section
            var section = new DynamicHomeSection(
                sectionType: request.SectionType,
                order: request.Order,
                title: request.Title,
                titleAr: request.TitleAr,
                sectionConfig: request.SectionConfig ?? "{}",
                metadata: request.Metadata ?? "{}",
                targetAudience: targetAudienceJson,
                priority: request.Priority);

            // Update optional fields
            section.UpdateSection(
                title: request.Title,
                subtitle: request.Subtitle,
                titleAr: request.TitleAr,
                subtitleAr: request.SubtitleAr,
                sectionConfig: request.SectionConfig ?? "{}",
                metadata: request.Metadata ?? "{}",
                scheduledAt: request.ScheduledAt,
                expiresAt: request.ExpiresAt,
                targetAudience: targetAudienceJson,
                priority: request.Priority);

            await _sectionRepository.AddAsync(section);

            // Add content items
            foreach (var contentDto in request.Content)
            {
                var content = new DynamicSectionContent(
                    sectionId: section.Id,
                    contentType: contentDto.ContentType,
                    contentData: contentDto.ContentData,
                    metadata: contentDto.Metadata ?? "{}",
                    displayOrder: contentDto.DisplayOrder,
                    expiresAt: contentDto.ExpiresAt);

                await _contentRepository.AddAsync(content);
            }

            await _unitOfWork.SaveChangesAsync();

            return section.Id;
        }
    }
}