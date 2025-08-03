using System;
using System.Linq;
using AutoMapper;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Amenities;
using YemenBooking.Application.DTOs.Bookings;
using YemenBooking.Application.DTOs.Properties;
using YemenBooking.Core.Entities;
using YemenBooking.Core.ValueObjects;
using YemenBooking.Application.DTOs.Users;

namespace YemenBooking.Application.Mappings
{
    /// <summary>
    /// ملف تعريف الخرائط لجميع الاستعلامات
    /// Mapping profile for all query DTOs and their corresponding entities and value objects
    /// </summary>
    public class QueryMappingProfile : Profile
    {
        public QueryMappingProfile()
        {
            // Amenity mapping
            CreateMap<Amenity, AmenityDto>();

            // Booking mapping
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));

            // Booking details including payments and services
            CreateMap<Booking, BookingDetailsDto>()
                .IncludeBase<Booking, BookingDto>()
                .ForMember(dest => dest.PaymentDetails, opt => opt.MapFrom(src => src.Payments))
                .ForMember(dest => dest.ContactInfo, opt => opt.MapFrom(src => new ContactInfoDto { PhoneNumber = src.User.Phone ?? "", Email = src.User.Email ?? "" }));

            // Notification mapping
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.RecipientName, opt => opt.MapFrom(src => src.Recipient.Name))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Name));

            // Payment mapping
            CreateMap<Payment, PaymentDto>();

            // Property mapping
            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.PropertyType.Name))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.Name))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AverageRating));

            // Property details mapping
            CreateMap<Property, PropertyDetailsDto>()
                .IncludeBase<Property, PropertyDto>()
                .ForMember(dest => dest.Units, opt => opt.MapFrom(src => src.Units))
                // map full property amenities with availability & extra cost info
                .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.Amenities));

            // Property type mapping
            CreateMap<PropertyType, PropertyTypeDto>();

            // Staff mapping
            CreateMap<Staff, StaffDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.PropertyName, opt => opt.MapFrom(src => src.Property.Name));

            // Service mapping
            CreateMap<PropertyService, ServiceDto>()
                .ForMember(dest => dest.PropertyName, opt => opt.MapFrom(src => src.Property.Name));

            // Unit mapping
            CreateMap<Unit, UnitDto>()
                .ForMember(dest => dest.PropertyName, opt => opt.MapFrom(src => src.Property.Name))
                .ForMember(dest => dest.UnitTypeName, opt => opt.MapFrom(src => src.UnitType.Name))
                .ForMember(dest => dest.PricingMethod, opt => opt.MapFrom(src => src.PricingMethod))
                // include dynamic field values
                .ForMember(dest => dest.FieldValues, opt => opt.MapFrom(src => src.FieldValues));

            // User mapping
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.SettingsJson, opt => opt.MapFrom(src => src.SettingsJson))
                .ForMember(dest => dest.FavoritesJson, opt => opt.MapFrom(src => src.FavoritesJson));

            // Role mapping
            CreateMap<Role, RoleDto>();

            // --------- NEW / UPDATED MAPPINGS ---------

            // Property amenity mapping
            CreateMap<PropertyAmenity, YemenBooking.Application.DTOs.Properties.PropertyAmenityDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AmenityId, opt => opt.MapFrom(src => src.PropertyTypeAmenity.AmenityId))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable))
                .ForMember(dest => dest.ExtraCost, opt => opt.MapFrom(src => src.ExtraCost != null ? src.ExtraCost.Amount : (decimal?)null))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.PropertyTypeAmenity.Amenity.Description))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PropertyTypeAmenity.Amenity.Name))
                .ForMember(dest => dest.IconUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            // UnitType field mapping
            CreateMap<UnitTypeField, UnitTypeFieldDto>()
                .ForMember(dest => dest.FieldId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UnitTypeId, opt => opt.MapFrom(src => src.UnitTypeId.ToString()))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.FieldName))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                // ignore heavy JSON conversion for now
                .ForMember(dest => dest.FieldOptions, opt => opt.Ignore())
                .ForMember(dest => dest.ValidationRules, opt => opt.Ignore());

            // Unit field value mapping
            CreateMap<UnitFieldValue, UnitFieldValueDto>()
                .ForMember(dest => dest.ValueId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FieldId, opt => opt.MapFrom(src => src.UnitTypeFieldId))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.UnitTypeField.FieldName))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.UnitTypeField.DisplayName))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.FieldValue))
                .ForMember(dest => dest.FieldType, opt => opt.MapFrom(src => src.UnitTypeField.FieldTypeId))
                .ForMember(dest => dest.Field, opt => opt.MapFrom(src => src.UnitTypeField));

            // Money value object mapping
            CreateMap<Money, MoneyDto>();

            // Contact value object mapping
            CreateMap<Contact, ContactDto>();

            // Address value object mapping
            CreateMap<Address, AddressDto>();

            // Policy mapping
            CreateMap<PropertyPolicy, PolicyDto>();

            // Property image mapping
            CreateMap<PropertyImage, PropertyImageDto>();

            // Audit log mapping
            CreateMap<AuditLog, AuditLogDto>();

            // Report mapping
            CreateMap<Report, ReportDto>()
                .ForMember(dest => dest.ReporterUserName, opt => opt.MapFrom(src => src.ReporterUser.Name))
                .ForMember(dest => dest.ReportedUserName, opt => opt.MapFrom(src => src.ReportedUser != null ? src.ReportedUser.Name : string.Empty))
                .ForMember(dest => dest.ReportedPropertyName, opt => opt.MapFrom(src => src.ReportedProperty != null ? src.ReportedProperty.Name : string.Empty));

            // SearchLog mapping
            CreateMap<SearchLog, SearchLogDto>();

            // Chat conversation mapping
            CreateMap<ChatConversation, ChatConversationDto>()
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.Messages.OrderBy(m => m.CreatedAt).LastOrDefault()))
                .ForMember(dest => dest.UnreadCount, opt => opt.MapFrom(src => 0)) // يحسب لاحقاً بناءً على قراءات المستخدم
                .ForMember(dest => dest.IsArchived, opt => opt.MapFrom(src => src.IsArchived))
                .ForMember(dest => dest.IsMuted, opt => opt.MapFrom(src => src.IsMuted))
                .ForMember(dest => dest.PropertyId, opt => opt.MapFrom(src => src.PropertyId));

            // Updated chat message mapping with status, edit info, and delivery receipt
            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => src.Reactions))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => src.IsEdited))
                .ForMember(dest => dest.EditedAt, opt => opt.MapFrom(src => src.EditedAt))
                .ForMember(dest => dest.DeliveryReceipt, opt => opt.MapFrom(src => new DeliveryReceiptDto { DeliveredAt = src.DeliveredAt, ReadAt = src.ReadAt, ReadBy = null }));

            // Mapping for chat-specific users
            CreateMap<User, ChatUserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.ProfileImage))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.LastLoginDate.HasValue && (DateTime.UtcNow - src.LastLoginDate.Value).TotalMinutes < 5 ? "online" : "offline"))
                .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => src.LastLoginDate))
                .ForMember(dest => dest.PropertyId, opt => opt.MapFrom(src => src.Properties.Select(p => p.Id).FirstOrDefault()))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => false));

            // Message reaction mapping
            CreateMap<MessageReaction, MessageReactionDto>();

            // Chat attachment mapping
            CreateMap<ChatAttachment, ChatAttachmentDto>();

            // Chat settings mapping
            CreateMap<ChatSettings, ChatSettingsDto>();

            // Availability mapping
            CreateMap<UnitAvailability, UnitAvailabilityDetailDto>();

            // Pricing mapping
            CreateMap<PricingRule, PricingRuleDto>();

        }
    }
} 