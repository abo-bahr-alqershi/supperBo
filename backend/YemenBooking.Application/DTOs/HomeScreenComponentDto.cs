using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs
{
    public class HomeScreenComponentDto
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string ComponentType { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsVisible { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string Alignment { get; set; }
        public string CustomClasses { get; set; }
        public string AnimationType { get; set; }
        public int AnimationDuration { get; set; }
        public string Conditions { get; set; }
        public List<ComponentPropertyDto> Properties { get; set; }
        public List<ComponentStyleDto> Styles { get; set; }
        public List<ComponentActionDto> Actions { get; set; }
        public ComponentDataSourceDto DataSource { get; set; }
    }
}