using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using AutoMapper;
using Newtonsoft.Json;

namespace YemenBooking.Application.Queries
{
    public class PreviewHomeScreenQuery : IRequest<ResultDto<HomeScreenPreviewDto>>
    {
        public Guid TemplateId { get; set; }
        public string Platform { get; set; }
        public string DeviceType { get; set; }
        public bool UseMockData { get; set; }
    }

    public class PreviewHomeScreenQueryHandler : IRequestHandler<PreviewHomeScreenQuery, ResultDto<HomeScreenPreviewDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly IMapper _mapper;

        public PreviewHomeScreenQueryHandler(
            IHomeScreenRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultDto<HomeScreenPreviewDto>> Handle(PreviewHomeScreenQuery request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetTemplateWithFullHierarchyAsync(request.TemplateId, cancellationToken);
            
            if (template == null)
                throw new NotFoundException(nameof(HomeScreenTemplate), request.TemplateId.ToString());

            var preview = new HomeScreenPreviewDto
            {
                TemplateId = template.Id,
                TemplateName = template.Name,
                Platform = request.Platform ?? template.Platform,
                DeviceType = request.DeviceType ?? "mobile",
                Sections = new List<HomeScreenSectionPreviewDto>()
            };

            foreach (var section in template.Sections.OrderBy(s => s.Order))
            {
                var sectionPreview = new HomeScreenSectionPreviewDto
                {
                    Id = section.Id,
                    Name = section.Name,
                    Title = section.Title,
                    Subtitle = section.Subtitle,
                    Order = section.Order,
                    IsVisible = section.IsVisible,
                    Styles = GenerateSectionStyles(section),
                    Components = new List<HomeScreenComponentPreviewDto>()
                };

                foreach (var component in section.Components.Where(c => c.IsVisible).OrderBy(c => c.Order))
                {
                    var componentPreview = new HomeScreenComponentPreviewDto
                    {
                        Id = component.Id,
                        Type = component.ComponentType,
                        Name = component.Name,
                        Order = component.Order,
                        ColSpan = component.ColSpan,
                        RowSpan = component.RowSpan,
                        Alignment = component.Alignment,
                        Properties = GetComponentProperties(component),
                        Styles = GetComponentStyles(component, request.Platform),
                        Data = await GetComponentData(component, request.UseMockData, cancellationToken)
                    };

                    // Apply animations
                    if (!string.IsNullOrEmpty(component.AnimationType))
                    {
                        componentPreview.Animation = new AnimationConfig
                        {
                            Type = component.AnimationType,
                            Duration = component.AnimationDuration,
                            Delay = 0
                        };
                    }

                    sectionPreview.Components.Add(componentPreview);
                }

                preview.Sections.Add(sectionPreview);
            }

            // Add preview metadata
            preview.Metadata = new PreviewMetadata
            {
                GeneratedAt = DateTime.UtcNow,
                TotalSections = preview.Sections.Count,
                TotalComponents = preview.Sections.Sum(s => s.Components.Count),
                EstimatedLoadTime = CalculateEstimatedLoadTime(preview),
                UsedMockData = request.UseMockData
            };

            return ResultDto<HomeScreenPreviewDto>.Ok(preview);
        }

        private Dictionary<string, string> GenerateSectionStyles(HomeScreenSection section)
        {
            var styles = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(section.BackgroundColor))
                styles["backgroundColor"] = section.BackgroundColor;
            
            if (!string.IsNullOrEmpty(section.BackgroundImage))
                styles["backgroundImage"] = $"url({section.BackgroundImage})";
            
            if (!string.IsNullOrEmpty(section.Padding))
                styles["padding"] = $"{section.Padding}px";
            
            if (!string.IsNullOrEmpty(section.Margin))
                styles["margin"] = $"{section.Margin}px";
            
            if (section.MinHeight > 0)
                styles["minHeight"] = $"{section.MinHeight}px";
            
            if (section.MaxHeight > 0)
                styles["maxHeight"] = $"{section.MaxHeight}px";

            // Parse custom styles
            if (!string.IsNullOrEmpty(section.CustomStyles))
            {
                try
                {
                    var customStyles = JsonConvert.DeserializeObject<Dictionary<string, string>>(section.CustomStyles);
                    foreach (var style in customStyles)
                    {
                        styles[style.Key] = style.Value;
                    }
                }
                catch { }
            }

            return styles;
        }

        private Dictionary<string, object> GetComponentProperties(HomeScreenComponent component)
        {
            var properties = new Dictionary<string, object>();
            
            foreach (var prop in component.Properties)
            {
                properties[prop.PropertyKey] = prop.Value ?? prop.DefaultValue;
            }

            return properties;
        }

        private Dictionary<string, string> GetComponentStyles(HomeScreenComponent component, string platform)
        {
            var styles = new Dictionary<string, string>();
            
            var relevantStyles = component.Styles
                .Where(s => s.Platform == "All" || s.Platform == platform)
                .OrderBy(s => s.Platform == "All" ? 1 : 0); // Platform-specific styles override

            foreach (var style in relevantStyles)
            {
                var value = style.StyleValue;
                if (!string.IsNullOrEmpty(style.Unit))
                    value += style.Unit;
                
                styles[style.StyleKey] = value;
            }

            return styles;
        }

        private async Task<object> GetComponentData(HomeScreenComponent component, bool useMockData, CancellationToken cancellationToken)
        {
            if (component.DataSource == null)
                return null;

            if (useMockData && !string.IsNullOrEmpty(component.DataSource.MockData))
            {
                try
                {
                    return JsonConvert.DeserializeObject(component.DataSource.MockData);
                }
                catch
                {
                    return null;
                }
            }

            // In preview mode, we'll return sample data based on component type
            return GetSampleDataForComponentType(component.ComponentType);
        }

        private object GetSampleDataForComponentType(string componentType)
        {
            switch (componentType)
            {
                case "PropertyList":
                    return new
                    {
                        items = new[]
                        {
                            new { id = 1, name = "Sample Property 1", price = "$100/night", rating = 4.5 },
                            new { id = 2, name = "Sample Property 2", price = "$150/night", rating = 4.8 },
                            new { id = 3, name = "Sample Property 3", price = "$200/night", rating = 4.2 }
                        }
                    };

                case "CategoryGrid":
                    return new
                    {
                        categories = new[]
                        {
                            new { id = 1, name = "Apartments", icon = "apartment", count = 25 },
                            new { id = 2, name = "Houses", icon = "house", count = 18 },
                            new { id = 3, name = "Villas", icon = "villa", count = 12 }
                        }
                    };

                case "OfferCard":
                    return new
                    {
                        offers = new[]
                        {
                            new { id = 1, title = "Summer Sale", discount = "20% OFF", validUntil = "2024-08-31" }
                        }
                    };

                case "MapView":
                    return new
                    {
                        properties = new[]
                        {
                            new { id = 1, lat = 15.3694, lng = 44.191, name = "Sample Property 1" },
                            new { id = 2, lat = 15.3784, lng = 44.201, name = "Sample Property 2" }
                        }
                    };

                default:
                    return null;
            }
        }

        private int CalculateEstimatedLoadTime(HomeScreenPreviewDto preview)
        {
            // Simple estimation based on component count and data sources
            var baseTime = 100; // Base load time in ms
            var componentTime = preview.Sections.Sum(s => s.Components.Count) * 50;
            var dataSourceTime = preview.Sections
                .SelectMany(s => s.Components)
                .Count(c => c.Data != null) * 200;
            
            return baseTime + componentTime + dataSourceTime;
        }
    }
}