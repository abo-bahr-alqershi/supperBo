using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.DTOs.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Infrastructure.Settings;
using System.Collections.Generic;
using System.Linq;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// تنفيذ خدمة المصادقة وإدارة الجلسة
    /// Authentication and session management service implementation
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        // إضافة حقن مستودعات المستخدم والأدوار والخصائص والموظفين وإعدادات JWT
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthenticationService> _logger;
        
        /// <summary>
        /// المُنشئ مع حقن التبعيات اللازمة
        /// Constructor with required dependencies
        /// </summary>
        public AuthenticationService(
            IPasswordHashingService passwordHashingService,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository,
            IPropertyRepository propertyRepository,
            IStaffRepository staffRepository,
            IOptions<JwtSettings> jwtOptions,
            ILogger<AuthenticationService> logger)
        {
            _passwordHashingService = passwordHashingService;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _propertyRepository = propertyRepository;
            _staffRepository = staffRepository;
            _jwtSettings = jwtOptions.Value;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<AuthResultDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("بدء عملية المصادقة للمستخدم: {Email}", email);
            var user = await _userRepository.GetUserByEmailAsync(email.Trim(), cancellationToken);
            if (user == null) throw new Exception("المستخدم غير موجود");
            // التحقق من كلمة المرور
            var valid = await _passwordHashingService.VerifyPasswordAsync(password, user.Password!, cancellationToken);
            if (!valid) throw new Exception("بيانات الاعتماد غير صحيحة");

            // جلب الأدوار
            var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id, cancellationToken);
            var roleNames = new List<string>();
            foreach (var ur in userRoles)
            {
                var role = await _roleRepository.GetRoleByIdAsync(ur.RoleId, cancellationToken);
                if (role != null) roleNames.Add(role.Name);
            }

            // إنشاء مجلد المطالبات
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("correlationId", Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin") // مؤقت
            };
            foreach (var rn in roleNames)
                claims.Add(new Claim(ClaimTypes.Role, rn));

            // إضافة صلاحيات لموظفي الكيان
            if (roleNames.Contains("Staff"))
            {
                var staff = await _staffRepository.GetStaffByUserAsync(user.Id, cancellationToken);
                if (staff != null)
                {
                    // صلاحيات من JSON
                    var perms = System.Text.Json.JsonSerializer.Deserialize<string[]>(staff.Permissions);
                    if (perms != null)
                        foreach (var p in perms)
                            claims.Add(new Claim("permission", p));
                    // إضافة معلومات الكيان
                    claims.Add(new Claim("propertyId", staff.PropertyId.ToString()));
                    var prop = await _propertyRepository.GetPropertyByIdAsync(staff.PropertyId, cancellationToken);
                    if (prop != null)
                        claims.Add(new Claim("propertyName", prop.Name));
                }
            }
            // إضافة معلومات الكيان لمالكي الكيان
            if (roleNames.Contains("Owner"))
            {
                var props = await _propertyRepository.GetPropertiesByOwnerAsync(user.Id, cancellationToken);
                var firstProp = props.FirstOrDefault();
                if (firstProp != null)
                {
                    claims.Add(new Claim("propertyId", firstProp.Id.ToString()));
                    claims.Add(new Claim("propertyName", firstProp.Name));
                }
            }

            // إعداد التوكن
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            // إنشاء Access Token
            var accessTokenExpires = now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = accessTokenExpires,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            string accessToken;
            try
            {
                accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(accessTokenDescriptor));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating access token for user {Email}", email);
                throw;
            }

            // إنشاء Refresh Token كـJWT
            var refreshTokenExpires = now.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            var refreshClaims = new List<Claim>(claims) { new Claim("tokenType", "refresh") };
            var refreshDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(refreshClaims),
                Expires = refreshTokenExpires,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = creds
            };
            var refreshToken = tokenHandler.WriteToken(tokenHandler.CreateToken(refreshDescriptor));

            return new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = accessTokenExpires,
                UserId = user.Id,
                UserName = user.Name,
                Email = user.Email,
                Role = roleNames.FirstOrDefault() ?? string.Empty,
                ProfileImage = user.ProfileImage,
                PropertyName = claims.FirstOrDefault(c => c.Type == "propertyName")?.Value,
                PropertyId = claims.FirstOrDefault(c => c.Type == "propertyId")?.Value,
                StaffId = claims.FirstOrDefault(c => c.Type == "staffId")?.Value
            };
        }

        /// <inheritdoc />
        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("بدء تجديد رمز المصادقة");
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = true
            }, out var validatedToken);
            // التحقق من نوع التوكن
            if (principal.FindFirst("tokenType")?.Value != "refresh")
                throw new SecurityTokenException("رمز التحديث غير صالح");

            var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            // إعادة إنشاء توكن بنفس البيانات
            return await LoginAsync(principal.FindFirst(ClaimTypes.Email)!.Value, string.Empty, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("بدء عملية تغيير كلمة المرور للمستخدم: {UserId}", userId);
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("طلب إعادة تعيين كلمة المرور للمستخدم: {Email}", email);
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إعادة تعيين كلمة المرور باستخدام التوكن");
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> VerifyEmailAsync(string token, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("تأكيد البريد الإلكتروني باستخدام التوكن");
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> ActivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("تفعيل المستخدم بعد التحقق: {UserId}", userId);
            throw new NotImplementedException();
        }
    }
} 