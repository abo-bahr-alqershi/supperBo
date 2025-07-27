namespace YemenBooking.Application.DTOs.Properties
{
    /// <summary>
    /// DTO لسياسة الكيان
    /// DTO for property policy
    /// </summary>
    public class PropertyPolicyDto
    {
        /// <summary>
        /// نوع السياسة
        /// Policy type
        /// </summary>
        public string PolicyType { get; set; } = string.Empty;

        /// <summary>
        /// محتوى السياسة
        /// Policy content
        /// </summary>
        public string PolicyContent { get; set; } = string.Empty;

        /// <summary>
        /// هل نشطة
        /// Is active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
} 