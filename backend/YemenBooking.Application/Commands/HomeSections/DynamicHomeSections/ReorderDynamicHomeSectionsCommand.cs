using MediatR;
using System;
using System.Collections.Generic;

namespace YemenBooking.Application.Commands.HomeSections.DynamicHomeSections
{
    public class ReorderDynamicHomeSectionsCommand : IRequest<bool>
    {
        public List<ReorderSectionDto> Sections { get; set; } = new();
    }

    public class ReorderSectionDto
    {
        public Guid Id { get; set; }
        public int NewOrder { get; set; }
    }
}