using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services; // Added for ICurrentUserService
using AutoMapper;

namespace YemenBooking.Application.Handlers.Commands.Chat
{
    /// <summary>
    /// معالج أمر إنشاء محادثة جديدة بين المستخدمين
    /// </summary>
    public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ResultDto<Guid>>
    {
        private readonly IChatConversationRepository _conversationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IWebSocketService _webSocketService;
        private readonly ILogger<CreateConversationCommandHandler> _logger;

        public CreateConversationCommandHandler(
            IChatConversationRepository conversationRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IWebSocketService webSocketService,
            ILogger<CreateConversationCommandHandler> logger)
        {
            _conversationRepository = conversationRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _webSocketService = webSocketService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("إنشاء محادثة جديدة للمستخدم {UserId}", _currentUserService.UserId);

                var conversation = new ChatConversation
                {
                    Id = Guid.NewGuid(),
                    ConversationType = request.ConversationType,
                    Title = request.Title,
                    Description = request.Description,
                    PropertyId = request.PropertyId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsArchived = false,
                    IsMuted = false
                };

                // إضافة المشاركين
                var currentUserId = _currentUserService.UserId;
                var participantIds = request.ParticipantIds.Contains(currentUserId)
                    ? request.ParticipantIds
                    : request.ParticipantIds.Append(currentUserId).ToList();
                foreach (var userId in participantIds)
                {
                    conversation.Participants.Add(new User { Id = userId });
                }

                await _unitOfWork.Repository<ChatConversation>().AddAsync(conversation, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Broadcast new conversation to participants
                var convDto = _mapper.Map<ChatConversationDto>(conversation);
                foreach (var participant in conversation.Participants.Where(p => p.Id != currentUserId))
                {
                    await _webSocketService.SendEventAsync(
                        participant.Id,
                        "conversation_created",
                        new { conversation = convDto },
                        cancellationToken);
                }

                return ResultDto<Guid>.Ok(conversation.Id, "تم إنشاء المحادثة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إنشاء المحادثة");
                return ResultDto<Guid>.Failed("حدث خطأ أثناء إنشاء المحادثة");
            }
        }
    }
} 