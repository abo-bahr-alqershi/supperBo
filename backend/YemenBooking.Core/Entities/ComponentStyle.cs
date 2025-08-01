using System;

namespace YemenBooking.Core.Entities
{
    public class ComponentStyle : BaseEntity
    {
        public Guid ComponentId { get; private set; }
        public string StyleKey { get; private set; }
        public string StyleValue { get; private set; }
        public string Unit { get; private set; } // px, %, em, rem
        public bool IsImportant { get; private set; }
        public string MediaQuery { get; private set; } // Responsive styles
        public string State { get; private set; } // hover, active, focus
        public string Platform { get; private set; } // iOS, Android, All
        
        public HomeScreenComponent Component { get; private set; }

        protected ComponentStyle() { }

        public ComponentStyle(
            Guid componentId,
            string styleKey,
            string styleValue,
            string unit = null,
            string platform = "All")
        {
            Id = Guid.NewGuid();
            ComponentId = componentId;
            StyleKey = styleKey;
            StyleValue = styleValue;
            Unit = unit;
            Platform = platform;
            IsImportant = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string styleValue,
            string unit,
            bool isImportant,
            string mediaQuery,
            string state)
        {
            StyleValue = styleValue;
            Unit = unit;
            IsImportant = isImportant;
            MediaQuery = mediaQuery;
            State = state;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}