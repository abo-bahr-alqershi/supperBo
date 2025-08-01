using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries
{
    public class GetComponentTypesQuery : IRequest<ResultDto<List<ComponentTypeDto>>>
    {
        public string Platform { get; set; }
    }

    public class GetComponentTypesQueryHandler : IRequestHandler<GetComponentTypesQuery, ResultDto<List<ComponentTypeDto>>>
    {
        public GetComponentTypesQueryHandler()
        {
        }

        public Task<ResultDto<List<ComponentTypeDto>>> Handle(GetComponentTypesQuery request, CancellationToken cancellationToken)
        {
            var componentTypes = new List<ComponentTypeDto>
            {
                new ComponentTypeDto
                {
                    Type = "Banner",
                    Name = "Banner",
                    Description = "Full-width promotional banner",
                    Icon = "image",
                    Category = "Display",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 2,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "imageUrl",
                            Name = "Image URL",
                            Type = "image",
                            IsRequired = true,
                            DefaultValue = ""
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "title",
                            Name = "Title",
                            Type = "text",
                            IsRequired = false,
                            DefaultValue = ""
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "actionUrl",
                            Name = "Action URL",
                            Type = "text",
                            IsRequired = false,
                            DefaultValue = ""
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "Carousel",
                    Name = "Carousel",
                    Description = "Sliding carousel for multiple items",
                    Icon = "view_carousel",
                    Category = "Display",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 3,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "autoPlay",
                            Name = "Auto Play",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "interval",
                            Name = "Interval (seconds)",
                            Type = "number",
                            IsRequired = false,
                            DefaultValue = "5"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showIndicators",
                            Name = "Show Indicators",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "CategoryGrid",
                    Name = "Category Grid",
                    Description = "Grid layout for categories",
                    Icon = "grid_view",
                    Category = "Navigation",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 2,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "columns",
                            Name = "Columns",
                            Type = "select",
                            IsRequired = true,
                            DefaultValue = "4",
                            Options = new[] { "2", "3", "4", "6" }
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showLabels",
                            Name = "Show Labels",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "PropertyList",
                    Name = "Property List",
                    Description = "List of properties with filters",
                    Icon = "list",
                    Category = "Data",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 4,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "layout",
                            Name = "Layout",
                            Type = "select",
                            IsRequired = true,
                            DefaultValue = "grid",
                            Options = new[] { "grid", "list", "card" }
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "itemsPerPage",
                            Name = "Items Per Page",
                            Type = "number",
                            IsRequired = false,
                            DefaultValue = "10"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showFilters",
                            Name = "Show Filters",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "SearchBar",
                    Name = "Search Bar",
                    Description = "Search input with suggestions",
                    Icon = "search",
                    Category = "Input",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 1,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "placeholder",
                            Name = "Placeholder",
                            Type = "text",
                            IsRequired = false,
                            DefaultValue = "Search..."
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showSuggestions",
                            Name = "Show Suggestions",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "searchInFields",
                            Name = "Search Fields",
                            Type = "multiselect",
                            IsRequired = true,
                            DefaultValue = "name,description",
                            Options = new[] { "name", "description", "location", "tags" }
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "OfferCard",
                    Name = "Offer Card",
                    Description = "Special offers and deals",
                    Icon = "local_offer",
                    Category = "Display",
                    DefaultColSpan = 6,
                    DefaultRowSpan = 2,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "offerType",
                            Name = "Offer Type",
                            Type = "select",
                            IsRequired = true,
                            DefaultValue = "percentage",
                            Options = new[] { "percentage", "fixed", "bogo" }
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "offerValue",
                            Name = "Offer Value",
                            Type = "text",
                            IsRequired = true,
                            DefaultValue = ""
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "expiryDate",
                            Name = "Expiry Date",
                            Type = "date",
                            IsRequired = false,
                            DefaultValue = ""
                        }
                    }
                },
                new ComponentTypeDto
                {
                    Type = "MapView",
                    Name = "Map View",
                    Description = "Interactive map with markers",
                    Icon = "map",
                    Category = "Display",
                    DefaultColSpan = 12,
                    DefaultRowSpan = 4,
                    Properties = new List<ComponentPropertyMetadata>
                    {
                        new ComponentPropertyMetadata
                        {
                            Key = "centerLat",
                            Name = "Center Latitude",
                            Type = "number",
                            IsRequired = true,
                            DefaultValue = "0"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "centerLng",
                            Name = "Center Longitude",
                            Type = "number",
                            IsRequired = true,
                            DefaultValue = "0"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "zoom",
                            Name = "Zoom Level",
                            Type = "number",
                            IsRequired = false,
                            DefaultValue = "12"
                        },
                        new ComponentPropertyMetadata
                        {
                            Key = "showUserLocation",
                            Name = "Show User Location",
                            Type = "boolean",
                            IsRequired = false,
                            DefaultValue = "true"
                        }
                    }
                }
            };

            // Filter by platform if specified
            if (!string.IsNullOrEmpty(request.Platform) && request.Platform != "All")
            {
                // In a real implementation, you might have platform-specific components
                // For now, return all components for all platforms
            }

            return Task.FromResult(ResultDto<List<ComponentTypeDto>>.Ok(componentTypes));
        }
    }
}