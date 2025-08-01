using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries
{
    public class GetDataSourcesQuery : IRequest<ResultDto<List<DataSourceDto>>>
    {
        public string ComponentType { get; set; }
    }

    public class GetDataSourcesQueryHandler : IRequestHandler<GetDataSourcesQuery, ResultDto<List<DataSourceDto>>>
    {
        public GetDataSourcesQueryHandler()
        {
        }

        public Task<ResultDto<List<DataSourceDto>>> Handle(GetDataSourcesQuery request, CancellationToken cancellationToken)
        {
            var dataSources = new List<DataSourceDto>();

            // Static data sources
            dataSources.Add(new DataSourceDto
            {
                Id = "static",
                Name = "Static Data",
                Type = "Static",
                Description = "Manually configured data",
                IsAvailable = true,
                RequiresAuth = false,
                SupportedComponents = new[] { "All" }
            });

            // API data sources based on component type
            switch (request.ComponentType)
            {
                case "PropertyList":
                    dataSources.Add(new DataSourceDto
                    {
                        Id = "properties-api",
                        Name = "Properties API",
                        Type = "API",
                        Description = "Fetch properties from API",
                        Endpoint = "/api/properties",
                        IsAvailable = true,
                        RequiresAuth = false,
                        SupportedComponents = new[] { "PropertyList", "Carousel" },
                        Parameters = new List<DataSourceParameter>
                        {
                            new DataSourceParameter
                            {
                                Key = "limit",
                                Name = "Limit",
                                Type = "number",
                                DefaultValue = "10",
                                IsRequired = false
                            },
                            new DataSourceParameter
                            {
                                Key = "sort",
                                Name = "Sort By",
                                Type = "select",
                                Options = new[] { "price", "rating", "date" },
                                DefaultValue = "rating",
                                IsRequired = false
                            }
                        }
                    });
                    break;

                case "CategoryGrid":
                    dataSources.Add(new DataSourceDto
                    {
                        Id = "categories-api",
                        Name = "Categories API",
                        Type = "API",
                        Description = "Fetch property categories",
                        Endpoint = "/api/property-types",
                        IsAvailable = true,
                        RequiresAuth = false,
                        SupportedComponents = new[] { "CategoryGrid" }
                    });
                    break;

                case "OfferCard":
                    dataSources.Add(new DataSourceDto
                    {
                        Id = "offers-api",
                        Name = "Offers API",
                        Type = "API",
                        Description = "Fetch active offers",
                        Endpoint = "/api/offers",
                        IsAvailable = true,
                        RequiresAuth = false,
                        SupportedComponents = new[] { "OfferCard", "Carousel" },
                        Parameters = new List<DataSourceParameter>
                        {
                            new DataSourceParameter
                            {
                                Key = "active",
                                Name = "Active Only",
                                Type = "boolean",
                                DefaultValue = "true",
                                IsRequired = false
                            }
                        }
                    });
                    break;

                case "MapView":
                    dataSources.Add(new DataSourceDto
                    {
                        Id = "map-properties-api",
                        Name = "Map Properties API",
                        Type = "API",
                        Description = "Fetch properties with location data",
                        Endpoint = "/api/properties/map",
                        IsAvailable = true,
                        RequiresAuth = false,
                        SupportedComponents = new[] { "MapView" },
                        Parameters = new List<DataSourceParameter>
                        {
                            new DataSourceParameter
                            {
                                Key = "bounds",
                                Name = "Map Bounds",
                                Type = "object",
                                IsRequired = true,
                                Description = "NE and SW coordinates"
                            }
                        }
                    });
                    break;
            }

            return Task.FromResult(ResultDto<List<DataSourceDto>>.Ok(dataSources));
        }
    }
}