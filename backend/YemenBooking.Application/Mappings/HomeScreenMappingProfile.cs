using AutoMapper;
using YemenBooking.Core.Entities;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Mappings
{
    /// <summary>
    /// Mapping profile for HomeScreen templates, sections, components and their related value objects.
    /// </summary>
    public class HomeScreenMappingProfile : Profile
    {
        public HomeScreenMappingProfile()
        {
            // Template mapping
            CreateMap<HomeScreenTemplate, HomeScreenTemplateDto>()
                .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections))
                // The following members don’t exist on the entity – ignore or map later in dedicated query handlers
                .ForMember(dest => dest.PublishedByName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomizationData, opt => opt.Ignore())
                .ForMember(dest => dest.UserPreferences, opt => opt.Ignore());

            // Section mapping
            CreateMap<HomeScreenSection, HomeScreenSectionDto>()
                .ForMember(dest => dest.Components, opt => opt.MapFrom(src => src.Components));

            // Component mapping
            CreateMap<HomeScreenComponent, HomeScreenComponentDto>()
                .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties))
                .ForMember(dest => dest.Styles, opt => opt.MapFrom(src => src.Styles))
                .ForMember(dest => dest.Actions, opt => opt.MapFrom(src => src.Actions))
                .ForMember(dest => dest.DataSource, opt => opt.MapFrom(src => src.DataSource));

            // Sub-objects
            CreateMap<ComponentProperty, ComponentPropertyDto>();
            CreateMap<ComponentStyle, ComponentStyleDto>();
            CreateMap<ComponentAction, ComponentActionDto>();
            CreateMap<ComponentDataSource, ComponentDataSourceDto>();
        }
    }
}