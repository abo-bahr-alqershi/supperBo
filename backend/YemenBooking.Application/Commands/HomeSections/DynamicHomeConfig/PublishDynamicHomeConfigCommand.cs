using MediatR;
using System;

namespace YemenBooking.Application.Commands.HomeSections.DynamicHomeConfig
{
    public class PublishDynamicHomeConfigCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public PublishDynamicHomeConfigCommand(Guid id)
        {
            Id = id;
        }
    }
}