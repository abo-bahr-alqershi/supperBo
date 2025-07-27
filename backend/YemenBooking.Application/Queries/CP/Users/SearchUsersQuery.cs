using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Queries.Users
{
    /// <summary>
    /// استعلام للبحث عن المستخدمين
    /// Query to search users
    /// </summary>
    public class SearchUsersQuery : IRequest<PaginatedResult<UserDto>>
    {
        /// <summary>
        /// نص البحث
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// معايير التصفية (اختياري)
        /// </summary>
        public string? FilterCriteria { get; set; }

        /// <summary>
        /// رقم الصفحة
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// حجم الصفحة
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// فلترة بالدور (اختياري)
        /// Role filter (optional)
        /// </summary>
        public Guid? RoleId { get; set; }

        /// <summary>
        /// فلترة بالحالة النشطة (اختياري)
        /// Active status filter (optional)
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// فلترة بتاريخ التسجيل بعد (اختياري)
        /// Filter by registration date after (optional)
        /// </summary>
        public DateTime? CreatedAfter { get; set; }

        /// <summary>
        /// فلترة بتاريخ التسجيل قبل (اختياري)
        /// Filter by registration date before (optional)
        /// </summary>
        public DateTime? CreatedBefore { get; set; }

        /// <summary>
        /// فلترة بالنشاط الأخير بعد (اختياري)
        /// Filter by last login after (optional)
        /// </summary>
        public DateTime? LastLoginAfter { get; set; }

        /// <summary>
        /// فلترة بفئة الولاء (اختياري)
        /// Loyalty tier filter (optional)
        /// </summary>
        public string? LoyaltyTier { get; set; }

        /// <summary>
        /// فلترة بالإنفاق الأدنى (اختياري)
        /// Filter by minimum total spent (optional)
        /// </summary>
        public decimal? MinTotalSpent { get; set; }

        /// <summary>
        /// خيارات الترتيب المتقدمة: registration_date, last_login, total_spent
        /// advanced sort options: registration_date, last_login, total_spent
        /// </summary>
        public string? SortBy { get; set; }
    }
} 