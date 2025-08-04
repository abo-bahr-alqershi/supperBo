using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemenBooking.Core.Entities
{
    public class DynamicSectionContent : BaseEntity
    {
        [Required]
        public Guid SectionId { get; private set; }
        
        [Required]
        [StringLength(100)]
        public string ContentType { get; private set; } // PROPERTY, OFFER, ADVERTISEMENT, DESTINATION, etc.
        
        [Column(TypeName = "json")]
        public string ContentData { get; private set; } // JSON data
        
        [Column(TypeName = "json")]
        public string Metadata { get; private set; } // Additional metadata
        
        public DateTime? ExpiresAt { get; private set; }
        
        public int DisplayOrder { get; private set; }
        
        public bool IsActive { get; private set; }
        
        // Navigation properties
        public DynamicHomeSection Section { get; private set; }

        protected DynamicSectionContent() { }

        public DynamicSectionContent(
            Guid sectionId,
            string contentType,
            string contentData,
            string metadata = "{}",
            int displayOrder = 0,
            DateTime? expiresAt = null)
        {
            Id = Guid.NewGuid();
            SectionId = sectionId;
            ContentType = contentType;
            ContentData = contentData;
            Metadata = metadata;
            DisplayOrder = displayOrder;
            ExpiresAt = expiresAt;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateContent(
            string contentData,
            string metadata,
            DateTime? expiresAt,
            int displayOrder)
        {
            ContentData = contentData;
            Metadata = metadata;
            ExpiresAt = expiresAt;
            DisplayOrder = displayOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDisplayOrder(int newOrder)
        {
            DisplayOrder = newOrder;
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
        public bool IsValid => IsActive && !IsExpired;
        
        public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    }
}