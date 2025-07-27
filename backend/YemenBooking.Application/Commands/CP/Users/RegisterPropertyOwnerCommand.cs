using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;
using YemenBooking.Application.DTOs.Properties;

namespace YemenBooking.Application.Commands.Users
{
    /// <summary>
    /// أمر لتسجيل مالك كيان جديد مع بيانات الكيان الكاملة والحقول الديناميكية
    /// Command to register a new property owner with full property data and dynamic fields
    /// </summary>
    public class RegisterPropertyOwnerCommand : IRequest<ResultDto<OwnerRegistrationResultDto>>
    {
        // بيانات المستخدم
        /// <summary>
        /// اسم المالك
        /// Owner name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// بريد المالك الإلكتروني
        /// Owner email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// كلمة مرور المالك
        /// Owner password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// رقم هاتف المالك
        /// Owner phone number
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// رابط صورة الملف الشخصي (اختياري)
        /// Profile image URL (optional)
        /// </summary>
        public string? ProfileImage { get; set; }

        // بيانات الكيان
        /// <summary>
        /// معرف نوع الكيان
        /// Property type ID
        /// </summary>
        public Guid PropertyTypeId { get; set; }

        /// <summary>
        /// اسم الكيان
        /// Property name
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// وصف الكيان (اختياري)
        /// Property description (optional)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// عنوان الكيان
        /// Property address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// المدينة
        /// City
        /// </summary>
        public string City { get; set; } = string.Empty;


        /// <summary>
        /// خط العرض (اختياري)
        /// Latitude (optional)
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// خط الطول (اختياري)
        /// Longitude (optional)
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// تقييم النجوم
        /// Star rating
        /// </summary>
        public int StarRating { get; set; } = 1;

        /// <summary>
        /// الصور الأولية للكيان (اختياري)
        /// Initial images for property (optional)
        /// </summary>
        public List<PropertyImageDto>? InitialImages { get; set; }

    }
} 