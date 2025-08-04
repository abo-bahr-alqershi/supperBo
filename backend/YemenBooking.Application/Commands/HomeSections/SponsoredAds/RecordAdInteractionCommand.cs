using MediatR;
using System;

namespace YemenBooking.Application.Commands.HomeSections.SponsoredAds
{
    public class RecordAdInteractionCommand : IRequest<bool>
    {
        public Guid AdId { get; set; }
        public string InteractionType { get; set; } // "impression" or "click"
        public Guid? UserId { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
        public string AdditionalData { get; set; } // JSON

        public RecordAdInteractionCommand(Guid adId, string interactionType)
        {
            AdId = adId;
            InteractionType = interactionType;
        }
    }
}