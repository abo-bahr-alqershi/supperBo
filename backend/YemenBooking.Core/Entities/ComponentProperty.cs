using System;

namespace YemenBooking.Core.Entities
{
    public class ComponentProperty : BaseEntity
    {
        public Guid ComponentId { get; private set; }
        public string PropertyKey { get; private set; }
        public string PropertyName { get; private set; }
        public string PropertyType { get; private set; } // text, number, boolean, select, color, image
        public string Value { get; private set; }
        public string DefaultValue { get; private set; }
        public bool IsRequired { get; private set; }
        public string ValidationRules { get; private set; } // JSON
        public string Options { get; private set; } // JSON for select options
        public string HelpText { get; private set; }
        public int Order { get; private set; }
        
        public HomeScreenComponent Component { get; private set; }

        protected ComponentProperty() { }

        public ComponentProperty(
            Guid componentId,
            string propertyKey,
            string propertyName,
            string propertyType,
            string defaultValue,
            bool isRequired,
            int order)
        {
            Id = Guid.NewGuid();
            ComponentId = componentId;
            PropertyKey = propertyKey;
            PropertyName = propertyName;
            PropertyType = propertyType;
            Value = defaultValue;
            DefaultValue = defaultValue;
            IsRequired = isRequired;
            Order = order;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateValue(string value)
        {
            Value = value;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMetadata(
            string propertyName,
            bool isRequired,
            string validationRules,
            string options,
            string helpText)
        {
            PropertyName = propertyName;
            IsRequired = isRequired;
            ValidationRules = validationRules;
            Options = options;
            HelpText = helpText;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}