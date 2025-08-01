using System;
using System.Collections.Generic;
using System.Linq;

namespace YemenBooking.Core.Entities
{
    public class HomeScreenSection : BaseEntity
    {
        public Guid TemplateId { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public int Order { get; private set; }
        public bool IsVisible { get; private set; }
        public string BackgroundColor { get; private set; }
        public string BackgroundImage { get; private set; }
        public string Padding { get; private set; }
        public string Margin { get; private set; }
        public int MinHeight { get; private set; }
        public int MaxHeight { get; private set; }
        public string CustomStyles { get; private set; } // JSON
        public string Conditions { get; private set; } // JSON for visibility conditions
        
        public HomeScreenTemplate Template { get; private set; }
        
        private readonly List<HomeScreenComponent> _components = new();
        public IReadOnlyCollection<HomeScreenComponent> Components => _components.AsReadOnly();

        protected HomeScreenSection() { }

        public HomeScreenSection(
            Guid templateId,
            string name,
            string title,
            int order,
            string backgroundColor = null,
            string padding = "16",
            string margin = "0")
        {
            Id = Guid.NewGuid();
            TemplateId = templateId;
            Name = name;
            Title = title;
            Order = order;
            IsVisible = true;
            BackgroundColor = backgroundColor;
            Padding = padding;
            Margin = margin;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string name,
            string title,
            string subtitle,
            string backgroundColor,
            string backgroundImage,
            string padding,
            string margin,
            int minHeight,
            int maxHeight,
            string customStyles,
            string conditions)
        {
            Name = name;
            Title = title;
            Subtitle = subtitle;
            BackgroundColor = backgroundColor;
            BackgroundImage = backgroundImage;
            Padding = padding;
            Margin = margin;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
            CustomStyles = customStyles;
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

        public void AddComponent(HomeScreenComponent component)
        {
            _components.Add(component);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveComponent(Guid componentId)
        {
            _components.RemoveAll(c => c.Id == componentId);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}