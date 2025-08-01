using System;
using System.Collections.Generic;
using System.Linq;

namespace YemenBooking.Core.Entities
{
    public class HomeScreenTemplate : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Version { get; private set; }
        public bool IsDefault { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public Guid? PublishedBy { get; private set; }
        public string Platform { get; private set; } // iOS, Android, All
        public string TargetAudience { get; private set; } // Guest, User, Premium
        public string MetaData { get; private set; } // JSON for additional settings
        
        private readonly List<HomeScreenSection> _sections = new();
        public IReadOnlyCollection<HomeScreenSection> Sections => _sections.AsReadOnly();

        protected HomeScreenTemplate() { }

        public HomeScreenTemplate(
            string name,
            string description,
            string version,
            string platform,
            string targetAudience,
            string metaData = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Version = version;
            Platform = platform;
            TargetAudience = targetAudience;
            MetaData = metaData;
            IsDefault = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string description, string metaData)
        {
            Name = name;
            Description = description;
            MetaData = metaData;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish(Guid userId)
        {
            IsActive = true;
            PublishedAt = DateTime.UtcNow;
            PublishedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unpublish()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAsDefault()
        {
            IsDefault = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddSection(HomeScreenSection section)
        {
            _sections.Add(section);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveSection(Guid sectionId)
        {
            _sections.RemoveAll(s => s.Id == sectionId);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}