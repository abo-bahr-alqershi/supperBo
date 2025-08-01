using System;
using System.Collections.Generic;
using System.Linq;

namespace YemenBooking.Core.Entities
{
    public class HomeScreenComponent : BaseEntity
    {
        public Guid SectionId { get; private set; }
        public string ComponentType { get; private set; } // Banner, Carousel, CategoryGrid, etc.
        public string Name { get; private set; }
        public int Order { get; private set; }
        public bool IsVisible { get; private set; }
        public int ColSpan { get; private set; } // Grid columns (1-12)
        public int RowSpan { get; private set; } // Grid rows
        public string Alignment { get; private set; } // left, center, right
        public string CustomClasses { get; private set; }
        public string AnimationType { get; private set; }
        public int AnimationDuration { get; private set; }
        public string Conditions { get; private set; } // JSON
        
        public HomeScreenSection Section { get; private set; }
        
        private readonly List<ComponentProperty> _properties = new();
        public IReadOnlyCollection<ComponentProperty> Properties => _properties.AsReadOnly();
        
        private readonly List<ComponentStyle> _styles = new();
        public IReadOnlyCollection<ComponentStyle> Styles => _styles.AsReadOnly();
        
        private readonly List<ComponentAction> _actions = new();
        public IReadOnlyCollection<ComponentAction> Actions => _actions.AsReadOnly();
        
        public ComponentDataSource DataSource { get; private set; }

        protected HomeScreenComponent() { }

        public HomeScreenComponent(
            Guid sectionId,
            string componentType,
            string name,
            int order,
            int colSpan = 12,
            int rowSpan = 1)
        {
            Id = Guid.NewGuid();
            SectionId = sectionId;
            ComponentType = componentType;
            Name = name;
            Order = order;
            IsVisible = true;
            ColSpan = colSpan;
            RowSpan = rowSpan;
            Alignment = "left";
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string name,
            int colSpan,
            int rowSpan,
            string alignment,
            string customClasses,
            string animationType,
            int animationDuration,
            string conditions)
        {
            Name = name;
            ColSpan = colSpan;
            RowSpan = rowSpan;
            Alignment = alignment;
            CustomClasses = customClasses;
            AnimationType = animationType;
            AnimationDuration = animationDuration;
            Conditions = conditions;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOrder(int newOrder)
        {
            Order = newOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddProperty(ComponentProperty property)
        {
            _properties.Add(property);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateProperty(Guid propertyId, string value)
        {
            var property = _properties.FirstOrDefault(p => p.Id == propertyId);
            property?.UpdateValue(value);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddStyle(ComponentStyle style)
        {
            _styles.Add(style);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddAction(ComponentAction action)
        {
            _actions.Add(action);
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDataSource(ComponentDataSource dataSource)
        {
            DataSource = dataSource;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}