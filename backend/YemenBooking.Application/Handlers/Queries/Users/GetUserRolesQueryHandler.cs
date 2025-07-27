using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;
using YemenBooking.Application.Queries.Users;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Users
{
    /// <summary>
    /// معالج استعلام الحصول على أدوار المستخدم
    /// Query handler for GetUserRolesQuery
    /// </summary>
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, PaginatedResult<RoleDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserRolesQueryHandler> _logger;

        public GetUserRolesQueryHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetUserRolesQueryHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<RoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام أدوار المستخدم: {UserId}, الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.UserId, request.PageNumber, request.PageSize);

            var currentUserId = _currentUserService.UserId;
            if (currentUserId != request.UserId && !_currentUserService.UserRoles.Contains("Admin"))
            {
                throw new UnauthorizedAccessException("ليس لديك صلاحية لعرض أدوار هذا المستخدم");
            }

            var rolesList = (await _userRepository.GetUserRolesAsync(request.UserId, cancellationToken)).ToList();

            var totalCount = rolesList.Count;
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var pagedRoles = rolesList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(ur => ur.Role)
                .ToList();

            var dtos = pagedRoles.Select(r => _mapper.Map<RoleDto>(r)).ToList();

            _logger.LogInformation("تم جلب {Count} دور من إجمالي {TotalCount} للمستخدم: {UserId}", dtos.Count, totalCount, request.UserId);
            return new PaginatedResult<RoleDto>(dtos, pageNumber, pageSize, totalCount);
        }
    }
} 