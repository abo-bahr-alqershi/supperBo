using MediatR;
using System;

namespace YemenBooking.Application.Commands.HomeSections.DynamicHomeSections
{
    public class DeleteDynamicHomeSectionCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteDynamicHomeSectionCommand(Guid id)
        {
            Id = id;
        }
    }
}