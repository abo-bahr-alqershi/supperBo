using MediatR;
using System;

namespace YemenBooking.Application.Commands.HomeSections.CityDestinations
{
    public class UpdateCityDestinationStatsCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public int PropertyCount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public UpdateCityDestinationStatsCommand(
            Guid id, 
            int propertyCount, 
            decimal averagePrice, 
            decimal averageRating, 
            int reviewCount)
        {
            Id = id;
            PropertyCount = propertyCount;
            AveragePrice = averagePrice;
            AverageRating = averageRating;
            ReviewCount = reviewCount;
        }
    }
}