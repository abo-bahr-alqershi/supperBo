using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemenBooking.Core.Entities
{
    public class DynamicHomeConfig : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Version { get; private set; }
        
        public bool IsActive { get; private set; }
        
        public DateTime? PublishedAt { get; private set; }
        
        [Column(TypeName = "json")]
        public string GlobalSettings { get; private set; } // JSON for global settings
        
        [Column(TypeName = "json")]
        public string ThemeSettings { get; private set; } // JSON for theme settings
        
        [Column(TypeName = "json")]
        public string LayoutSettings { get; private set; } // JSON for layout settings
        
        [Column(TypeName = "json")]
        public string CacheSettings { get; private set; } // JSON for cache settings
        
        [Column(TypeName = "json")]
        public string AnalyticsSettings { get; private set; } // JSON for analytics settings
        
        [Column(TypeName = "json")]
        public string EnabledFeatures { get; private set; } // JSON array of enabled features
        
        [Column(TypeName = "json")]
        public string ExperimentalFeatures { get; private set; } // JSON for experimental features
        
        [StringLength(1000)]
        public string Description { get; private set; }

        protected DynamicHomeConfig() { }

        public DynamicHomeConfig(
            string version,
            string globalSettings = "{}",
            string themeSettings = "{}",
            string layoutSettings = "{}",
            string cacheSettings = "{}",
            string analyticsSettings = "{}",
            string enabledFeatures = "[]",
            string experimentalFeatures = "{}",
            string description = null)
        {
            Id = Guid.NewGuid();
            Version = version;
            IsActive = false; // New configs start as inactive
            GlobalSettings = globalSettings;
            ThemeSettings = themeSettings;
            LayoutSettings = layoutSettings;
            CacheSettings = cacheSettings;
            AnalyticsSettings = analyticsSettings;
            EnabledFeatures = enabledFeatures;
            ExperimentalFeatures = experimentalFeatures;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateConfig(
            string globalSettings,
            string themeSettings,
            string layoutSettings,
            string cacheSettings,
            string analyticsSettings,
            string enabledFeatures,
            string experimentalFeatures,
            string description)
        {
            GlobalSettings = globalSettings;
            ThemeSettings = themeSettings;
            LayoutSettings = layoutSettings;
            CacheSettings = cacheSettings;
            AnalyticsSettings = analyticsSettings;
            EnabledFeatures = enabledFeatures;
            ExperimentalFeatures = experimentalFeatures;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            IsActive = true;
            PublishedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unpublish()
        {
            IsActive = false;
            PublishedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void CreateNewVersion(string newVersion)
        {
            Version = newVersion;
            IsActive = false;
            PublishedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        // Helper methods
        public bool IsPublished => IsActive && PublishedAt.HasValue;
        
        public string GetDisplayName() => $"Version {Version} ({(IsPublished ? "Published" : "Draft")})";
    }
}