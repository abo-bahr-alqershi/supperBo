namespace YemenBooking.Core.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// كيان المرفق
/// Amenity entity
/// </summary>
[Display(Name = "كيان المرفق")]
public class Amenity : BaseEntity
{
    /// <summary>
    /// اسم المرفق
    /// Amenity name (Wi-Fi, Pool, Gym, Breakfast)
    /// </summary>
    [Display(Name = "اسم المرفق")]
    public string Name { get; set; }
    
    /// <summary>
    /// وصف المرفق
    /// Amenity description
    /// </summary>
    [Display(Name = "وصف المرفق")]
    public string Description { get; set; }
    
    /// <summary>
    /// أنواع الكيانات المرتبطة بهذا المرفق
    /// Property types associated with this amenity
    /// </summary>
    [Display(Name = "أنواع الكيانات المرتبطة بهذا المرفق")]
    public virtual ICollection<PropertyTypeAmenity> PropertyTypeAmenities { get; set; } = new List<PropertyTypeAmenity>();
} 