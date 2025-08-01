using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using System.Linq;
using AutoMapper;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Commands
{
    public class DuplicateTemplateCommand : IRequest<ResultDto<HomeScreenTemplateDto>>
    {
        public Guid TemplateId { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
    }

    public class DuplicateTemplateCommandHandler : IRequestHandler<DuplicateTemplateCommand, ResultDto<HomeScreenTemplateDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public DuplicateTemplateCommandHandler(
            IHomeScreenRepository repository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ResultDto<HomeScreenTemplateDto>> Handle(DuplicateTemplateCommand request, CancellationToken cancellationToken)
        {
            var sourceTemplate = await _repository.GetTemplateWithFullHierarchyAsync(request.TemplateId, cancellationToken);
            
            if (sourceTemplate == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId.ToString());

            // Create new template
            var newTemplate = new HomeScreenTemplate(
                request.NewName ?? $"{sourceTemplate.Name} - Copy",
                request.NewDescription ?? sourceTemplate.Description,
                sourceTemplate.Version,
                sourceTemplate.Platform,
                sourceTemplate.TargetAudience,
                sourceTemplate.MetaData);

            newTemplate.CreatedBy = _currentUserService.UserId;

            // Duplicate sections
            foreach (var sourceSection in sourceTemplate.Sections.OrderBy(s => s.Order))
            {
                var newSection = new HomeScreenSection(
                    newTemplate.Id,
                    sourceSection.Name,
                    sourceSection.Title,
                    sourceSection.Order,
                    sourceSection.BackgroundColor,
                    sourceSection.Padding,
                    sourceSection.Margin);

                newSection.Update(
                    sourceSection.Name,
                    sourceSection.Title,
                    sourceSection.Subtitle,
                    sourceSection.BackgroundColor,
                    sourceSection.BackgroundImage,
                    sourceSection.Padding,
                    sourceSection.Margin,
                    sourceSection.MinHeight,
                    sourceSection.MaxHeight,
                    sourceSection.CustomStyles,
                    sourceSection.Conditions);

                newSection.CreatedBy = _currentUserService.UserId;

                // Duplicate components
                foreach (var sourceComponent in sourceSection.Components.OrderBy(c => c.Order))
                {
                    var newComponent = new HomeScreenComponent(
                        newSection.Id,
                        sourceComponent.ComponentType,
                        sourceComponent.Name,
                        sourceComponent.Order,
                        sourceComponent.ColSpan,
                        sourceComponent.RowSpan);

                    newComponent.Update(
                        sourceComponent.Name,
                        sourceComponent.ColSpan,
                        sourceComponent.RowSpan,
                        sourceComponent.Alignment,
                        sourceComponent.CustomClasses,
                        sourceComponent.AnimationType,
                        sourceComponent.AnimationDuration,
                        sourceComponent.Conditions);

                    newComponent.CreatedBy = _currentUserService.UserId;

                    // Duplicate properties
                    foreach (var sourceProperty in sourceComponent.Properties)
                    {
                        var newProperty = new ComponentProperty(
                            newComponent.Id,
                            sourceProperty.PropertyKey,
                            sourceProperty.PropertyName,
                            sourceProperty.PropertyType,
                            sourceProperty.DefaultValue,
                            sourceProperty.IsRequired,
                            sourceProperty.Order);

                        newProperty.UpdateValue(sourceProperty.Value);
                        newProperty.UpdateMetadata(
                            sourceProperty.PropertyName,
                            sourceProperty.IsRequired,
                            sourceProperty.ValidationRules,
                            sourceProperty.Options,
                            sourceProperty.HelpText);

                        newProperty.CreatedBy = _currentUserService.UserId;
                        newComponent.AddProperty(newProperty);
                    }

                    // Duplicate styles
                    foreach (var sourceStyle in sourceComponent.Styles)
                    {
                        var newStyle = new ComponentStyle(
                            newComponent.Id,
                            sourceStyle.StyleKey,
                            sourceStyle.StyleValue,
                            sourceStyle.Unit,
                            sourceStyle.Platform);

                        newStyle.Update(
                            sourceStyle.StyleValue,
                            sourceStyle.Unit,
                            sourceStyle.IsImportant,
                            sourceStyle.MediaQuery,
                            sourceStyle.State);

                        newStyle.CreatedBy = _currentUserService.UserId;
                        newComponent.AddStyle(newStyle);
                    }

                    // Duplicate actions
                    foreach (var sourceAction in sourceComponent.Actions)
                    {
                        var newAction = new ComponentAction(
                            newComponent.Id,
                            sourceAction.ActionType,
                            sourceAction.ActionTrigger,
                            sourceAction.ActionTarget,
                            sourceAction.Priority);

                        newAction.Update(
                            sourceAction.ActionTarget,
                            sourceAction.ActionParams,
                            sourceAction.Conditions,
                            sourceAction.RequiresAuth,
                            sourceAction.AnimationType);

                        newAction.CreatedBy = _currentUserService.UserId;
                        newComponent.AddAction(newAction);
                    }

                    // Duplicate data source
                    if (sourceComponent.DataSource != null)
                    {
                        var newDataSource = new ComponentDataSource(
                            newComponent.Id,
                            sourceComponent.DataSource.SourceType,
                            sourceComponent.DataSource.DataEndpoint,
                            sourceComponent.DataSource.DataMapping);

                        newDataSource.UpdateEndpoint(
                            sourceComponent.DataSource.DataEndpoint,
                            sourceComponent.DataSource.HttpMethod,
                            sourceComponent.DataSource.Headers,
                            sourceComponent.DataSource.QueryParams,
                            sourceComponent.DataSource.RequestBody);

                        newDataSource.UpdateCaching(
                            sourceComponent.DataSource.CacheKey,
                            sourceComponent.DataSource.CacheDuration,
                            sourceComponent.DataSource.RefreshTrigger,
                            sourceComponent.DataSource.RefreshInterval);

                        newDataSource.SetMockData(
                            sourceComponent.DataSource.MockData,
                            sourceComponent.DataSource.UseMockInDev);

                        newDataSource.CreatedBy = _currentUserService.UserId;
                        newComponent.SetDataSource(newDataSource);
                    }

                    newSection.AddComponent(newComponent);
                }

                newTemplate.AddSection(newSection);
            }

            await _repository.AddTemplateAsync(newTemplate, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return ResultDto<HomeScreenTemplateDto>.Ok(_mapper.Map<HomeScreenTemplateDto>(newTemplate));
        }
    }
}