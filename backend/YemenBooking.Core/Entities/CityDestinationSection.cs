using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemenBooking.Core.Entities
{
    public class CityDestinationSection : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; private set; }
        
        [Required]
        [StringLength(100)]
        public string NameAr { get; private set; }
        
        [Required]
        [StringLength(100)]
        public string Country { get; private set; }
        
        [Required]
        [StringLength(100)]
        public string CountryAr { get; private set; }
        
        [StringLength(1000)]
        public string Description { get; private set; }
        
        [StringLength(1000)]
        public string DescriptionAr { get; private set; }
        
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; private set; }
        
        [Column(TypeName = "json")]
        public string AdditionalImages { get; private set; } // JSON array
        
        [Column(TypeName = "decimal(10,8)")]
        public decimal Latitude { get; private set; }
        
        [Column(TypeName = "decimal(11,8)")]
        public decimal Longitude { get; private set; }
        
        public int PropertyCount { get; private set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal AveragePrice { get; private set; }
        
        [Required]
        [StringLength(10)]
        public string Currency { get; private set; }
        
        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; private set; }
        
        public int ReviewCount { get; private set; }
        
        public bool IsPopular { get; private set; }
        
        public bool IsFeatured { get; private set; }
        
        public int Priority { get; private set; }
        
        [Column(TypeName = "json")]
        public string Highlights { get; private set; } // JSON array
        
        [Column(TypeName = "json")]
        public string HighlightsAr { get; private set; } // JSON array
        
        [Column(TypeName = "json")]
        public string WeatherData { get; private set; } // JSON
        
        [Column(TypeName = "json")]
        public string AttractionsData { get; private set; } // JSON
        
        [Column(TypeName = "json")]
        public string Metadata { get; private set; } // JSON
        
        public bool IsActive { get; private set; }

        protected CityDestinationSection() { }

        public CityDestinationSection(
            string name,
            string nameAr,
            string country,
            string countryAr,
            string imageUrl,
            decimal latitude,
            decimal longitude,
            string currency,
            string description = null,
            string descriptionAr = null,
            string additionalImages = "[]",
            int propertyCount = 0,
            decimal averagePrice = 0,
            decimal averageRating = 0,
            int reviewCount = 0,
            bool isPopular = false,
            bool isFeatured = false,
            int priority = 0,
            string highlights = "[]",
            string highlightsAr = "[]",
            string weatherData = "{}",
            string attractionsData = "{}",
            string metadata = "{}")
        {
            Id = Guid.NewGuid();
            Name = name;
            NameAr = nameAr;
            Country = country;
            CountryAr = countryAr;
            Description = description;
            DescriptionAr = descriptionAr;
            ImageUrl = imageUrl;
            AdditionalImages = additionalImages;
            Latitude = latitude;
            Longitude = longitude;
            PropertyCount = propertyCount;
            AveragePrice = averagePrice;
            Currency = currency;
            AverageRating = averageRating;
            ReviewCount = reviewCount;
            IsPopular = isPopular;
            IsFeatured = isFeatured;
            Priority = priority;
            Highlights = highlights;
            HighlightsAr = highlightsAr;
            WeatherData = weatherData;
            AttractionsData = attractionsData;
            Metadata = metadata;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDestination(
            string name,
            string nameAr,
            string country,
            string countryAr,
            string description,
            string descriptionAr,
            string imageUrl,
            string additionalImages,
            decimal latitude,
            decimal longitude,
            string currency,
            bool isPopular,
            bool isFeatured,
            int priority,
            string highlights,
            string highlightsAr,
            string weatherData,
            string attractionsData,
            string metadata)
        {
            Name = name;
            NameAr = nameAr;
            Country = country;
            CountryAr = countryAr;
            Description = description;
            DescriptionAr = descriptionAr;
            ImageUrl = imageUrl;
            AdditionalImages = additionalImages;
            Latitude = latitude;
            Longitude = longitude;
            Currency = currency;
            IsPopular = isPopular;
            IsFeatured = isFeatured;
            Priority = priority;
            Highlights = highlights;
            HighlightsAr = highlightsAr;
            WeatherData = weatherData;
            AttractionsData = attractionsData;
            Metadata = metadata;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatistics(
            int propertyCount,
            decimal averagePrice,
            decimal averageRating,
            int reviewCount)
        {
            PropertyCount = propertyCount;
            AveragePrice = averagePrice;
            AverageRating = averageRating;
            ReviewCount = reviewCount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void TogglePopularStatus()
        {
            IsPopular = !IsPopular;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleFeaturedStatus()
        {
            IsFeatured = !IsFeatured;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleActiveStatus()
        {
            IsActive = !IsActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetActiveStatus(bool isActive)
        {
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }

        // Helper methods
        public string GetLocalizedName(bool isArabic) => isArabic ? NameAr : Name;
        
        public string GetLocalizedCountry(bool isArabic) => isArabic ? CountryAr : Country;
        
        public string GetLocalizedDescription(bool isArabic) => isArabic ? DescriptionAr : Description;
        
        public string GetFullName(bool isArabic) => 
            isArabic ? $"{NameAr}، {CountryAr}" : $"{Name}, {Country}";
    }
}