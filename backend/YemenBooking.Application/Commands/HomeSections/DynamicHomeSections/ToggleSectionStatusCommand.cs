using MediatR;
using System;

namespace YemenBooking.Application.Commands.HomeSections.DynamicHomeSections
{
    public class ToggleSectionStatusCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public bool? SetActive { get; set; } // null = toggle, true/false = set specific state

        public ToggleSectionStatusCommand(Guid id, bool? setActive = null)
        {
            Id = id;
            SetActive = setActive;
        }
    }
}