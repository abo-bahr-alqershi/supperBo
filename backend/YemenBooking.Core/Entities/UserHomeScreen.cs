using System;

namespace YemenBooking.Core.Entities
{
    public class UserHomeScreen : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid TemplateId { get; private set; }
        public string CustomizationData { get; private set; } // JSON للتخصيصات الشخصية
        public string UserPreferences { get; private set; } // JSON
        public DateTime LastViewedAt { get; private set; }
        public string DeviceInfo { get; private set; } // JSON
        
        public User User { get; private set; }
        public HomeScreenTemplate Template { get; private set; }

        protected UserHomeScreen() { }

        public UserHomeScreen(
            Guid userId,
            Guid templateId,
            string deviceInfo = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            TemplateId = templateId;
            DeviceInfo = deviceInfo;
            LastViewedAt = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateCustomization(string customizationData)
        {
            CustomizationData = customizationData;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePreferences(string userPreferences)
        {
            UserPreferences = userPreferences;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordView()
        {
            LastViewedAt = DateTime.UtcNow;
        }
    }
}