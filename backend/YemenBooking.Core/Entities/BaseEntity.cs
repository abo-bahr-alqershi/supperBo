using YemenBooking.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace YemenBooking.Core.Entities;

/// <summary>
/// الكيان الأساسي الذي ترث منه جميع الكيانات في النظام
/// Base entity class that all entities inherit from
/// </summary>
[Display(Name = "الكيان الأساسي لجميع الكيانات")]
public abstract class BaseEntity : IBaseEntity
{
    /// <summary>
    /// المعرف الفريد للكيان
    /// Unique identifier for the entity
    /// </summary>
    [Display(Name = "المعرف الفريد للكيان")]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// المستخدم اللذي إنشاء الكيان
    /// User who created the entity
    /// </summary>
    [Display(Name = "المستخدم الذي أنشأ الكيان")]
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// تاريخ إنشاء الكيان
    /// Creation date of the entity
    /// </summary>
    [Display(Name = "تاريخ إنشاء الكيان")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// المستخدم اللذي حدث الكيان
    /// User who updated the entity
    /// </summary>
    [Display(Name = "المستخدم الذي حدث الكيان")]
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// تاريخ آخر تحديث (اختياري)
    /// Date of last update (optional)
    /// </summary>
    [Display(Name = "تاريخ آخر تحديث")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// حالة نشاط الكيان
    /// Activity status of the entity
    /// </summary>
    [Display(Name = "حالة نشاط الكيان")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates if the entity is soft-deleted.
    /// </summary>
    [Display(Name = "حذف كاذب للكيان")]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// المستخدم اللذي حذف الكيان
    /// User who soft-deleted the entity
    /// </summary>
    [Display(Name = "المستخدم الذي حذف الكيان")]
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Date when the entity is soft-deleted.
    /// </summary>
    [Display(Name = "تاريخ حذف الكيان")]
    public DateTime? DeletedAt { get; set; }
}