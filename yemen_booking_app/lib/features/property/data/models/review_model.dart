import 'package:equatable/equatable.dart';

/// <summary>
/// نموذج بيانات المراجع/المقيم
/// Reviewer data model
/// </summary>
class ReviewerModel extends Equatable {
  /// <summary>
  /// معرف المراجع
  /// Reviewer ID
  /// </summary>
  final String id;

  /// <summary>
  /// اسم المراجع
  /// Reviewer name
  /// </summary>
  final String name;

  /// <summary>
  /// صورة المراجع
  /// Reviewer avatar
  /// </summary>
  final String? avatarUrl;

  /// <summary>
  /// هل المراجع معتمد
  /// Is reviewer verified
  /// </summary>
  final bool isVerified;

  /// <summary>
  /// عدد المراجعات السابقة
  /// Previous reviews count
  /// </summary>
  final int reviewsCount;

  /// <summary>
  /// تاريخ الانضمام
  /// Join date
  /// </summary>
  final DateTime? joinDate;

  /// <summary>
  /// البلد
  /// Country
  /// </summary>
  final String? country;

  const ReviewerModel({
    required this.id,
    required this.name,
    this.avatarUrl,
    required this.isVerified,
    required this.reviewsCount,
    this.joinDate,
    this.country,
  });

  factory ReviewerModel.fromJson(Map<String, dynamic> json) {
    return ReviewerModel(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      avatarUrl: json['avatarUrl'],
      isVerified: json['isVerified'] ?? false,
      reviewsCount: json['reviewsCount'] ?? 0,
      joinDate: json['joinDate'] != null 
          ? DateTime.parse(json['joinDate']) 
          : null,
      country: json['country'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'avatarUrl': avatarUrl,
      'isVerified': isVerified,
      'reviewsCount': reviewsCount,
      'joinDate': joinDate?.toIso8601String(),
      'country': country,
    };
  }

  @override
  List<Object?> get props => [id, name, avatarUrl, isVerified, reviewsCount, joinDate, country];
}

/// <summary>
/// نموذج بيانات صور المراجعة
/// Review images data model
/// </summary>
class ReviewImageModel extends Equatable {
  /// <summary>
  /// معرف الصورة
  /// Image ID
  /// </summary>
  final String id;

  /// <summary>
  /// رابط الصورة
  /// Image URL
  /// </summary>
  final String imageUrl;

  /// <summary>
  /// رابط الصورة المصغرة
  /// Thumbnail URL
  /// </summary>
  final String? thumbnailUrl;

  /// <summary>
  /// تسمية توضيحية
  /// Caption
  /// </summary>
  final String? caption;

  /// <summary>
  /// ترتيب العرض
  /// Display order
  /// </summary>
  final int displayOrder;

  const ReviewImageModel({
    required this.id,
    required this.imageUrl,
    this.thumbnailUrl,
    this.caption,
    required this.displayOrder,
  });

  factory ReviewImageModel.fromJson(Map<String, dynamic> json) {
    return ReviewImageModel(
      id: json['id'] ?? '',
      imageUrl: json['imageUrl'] ?? '',
      thumbnailUrl: json['thumbnailUrl'],
      caption: json['caption'],
      displayOrder: json['displayOrder'] ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'imageUrl': imageUrl,
      'thumbnailUrl': thumbnailUrl,
      'caption': caption,
      'displayOrder': displayOrder,
    };
  }

  @override
  List<Object?> get props => [id, imageUrl, thumbnailUrl, caption, displayOrder];
}

/// <summary>
/// نموذج بيانات تقييمات فرعية (النظافة، الخدمة، الموقع، إلخ)
/// Sub-ratings data model (cleanliness, service, location, etc.)
/// </summary>
class SubRatingModel extends Equatable {
  /// <summary>
  /// النظافة
  /// Cleanliness
  /// </summary>
  final double? cleanliness;

  /// <summary>
  /// جودة الخدمة
  /// Service quality
  /// </summary>
  final double? service;

  /// <summary>
  /// الموقع
  /// Location
  /// </summary>
  final double? location;

  /// <summary>
  /// قيمة مقابل السعر
  /// Value for money
  /// </summary>
  final double? valueForMoney;

  /// <summary>
  /// الراحة
  /// Comfort
  /// </summary>
  final double? comfort;

  /// <summary>
  /// الإفطار (إن وجد)
  /// Breakfast (if applicable)
  /// </summary>
  final double? breakfast;

  /// <summary>
  /// الواي فاي
  /// WiFi
  /// </summary>
  final double? wifi;

  const SubRatingModel({
    this.cleanliness,
    this.service,
    this.location,
    this.valueForMoney,
    this.comfort,
    this.breakfast,
    this.wifi,
  });

  factory SubRatingModel.fromJson(Map<String, dynamic> json) {
    return SubRatingModel(
      cleanliness: json['cleanliness']?.toDouble(),
      service: json['service']?.toDouble(),
      location: json['location']?.toDouble(),
      valueForMoney: json['valueForMoney']?.toDouble(),
      comfort: json['comfort']?.toDouble(),
      breakfast: json['breakfast']?.toDouble(),
      wifi: json['wifi']?.toDouble(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'cleanliness': cleanliness,
      'service': service,
      'location': location,
      'valueForMoney': valueForMoney,
      'comfort': comfort,
      'breakfast': breakfast,
      'wifi': wifi,
    };
  }

  @override
  List<Object?> get props => [cleanliness, service, location, valueForMoney, comfort, breakfast, wifi];
}

/// <summary>
/// نموذج بيانات المراجعة/التقييم
/// Review data model
/// </summary>
class ReviewModel extends Equatable {
  /// <summary>
  /// معرف المراجعة
  /// Review ID
  /// </summary>
  final String id;

  /// <summary>
  /// معرف العقار
  /// Property ID
  /// </summary>
  final String propertyId;

  /// <summary>
  /// معرف الحجز
  /// Booking ID
  /// </summary>
  final String? bookingId;

  /// <summary>
  /// بيانات المراجع
  /// Reviewer data
  /// </summary>
  final ReviewerModel reviewer;

  /// <summary>
  /// التقييم العام (من 1 إلى 5)
  /// Overall rating (1 to 5)
  /// </summary>
  final double overallRating;

  /// <summary>
  /// التقييمات الفرعية
  /// Sub-ratings
  /// </summary>
  final SubRatingModel subRatings;

  /// <summary>
  /// عنوان المراجعة
  /// Review title
  /// </summary>
  final String title;

  /// <summary>
  /// نص المراجعة
  /// Review content
  /// </summary>
  final String content;

  /// <summary>
  /// صور المراجعة
  /// Review images
  /// </summary>
  final List<ReviewImageModel> images;

  /// <summary>
  /// تاريخ الإقامة
  /// Stay date
  /// </summary>
  final DateTime? stayDate;

  /// <summary>
  /// تاريخ المراجعة
  /// Review date
  /// </summary>
  final DateTime reviewDate;

  /// <summary>
  /// نوع الإقامة
  /// Stay type
  /// </summary>
  final String? stayType;

  /// <summary>
  /// مدة الإقامة بالليالي
  /// Stay duration in nights
  /// </summary>
  final int? stayDurationNights;

  /// <summary>
  /// هل مراجعة موصى بها
  /// Is recommended review
  /// </summary>
  final bool isRecommended;

  /// <summary>
  /// عدد الإعجابات
  /// Likes count
  /// </summary>
  final int likesCount;

  /// <summary>
  /// هل مفيدة للمستخدم الحالي
  /// Is helpful to current user
  /// </summary>
  final bool isHelpful;

  /// <summary>
  /// رد إدارة العقار
  /// Management reply
  /// </summary>
  final String? managementReply;

  /// <summary>
  /// تاريخ رد الإدارة
  /// Management reply date
  /// </summary>
  final DateTime? managementReplyDate;

  /// <summary>
  /// حالة المراجعة
  /// Review status
  /// </summary>
  final String status;

  /// <summary>
  /// اللغة
  /// Language
  /// </summary>
  final String language;

  /// <summary>
  /// معلومات إضافية
  /// Additional information
  /// </summary>
  final Map<String, dynamic> additionalInfo;

  const ReviewModel({
    required this.id,
    required this.propertyId,
    this.bookingId,
    required this.reviewer,
    required this.overallRating,
    required this.subRatings,
    required this.title,
    required this.content,
    required this.images,
    this.stayDate,
    required this.reviewDate,
    this.stayType,
    this.stayDurationNights,
    required this.isRecommended,
    required this.likesCount,
    required this.isHelpful,
    this.managementReply,
    this.managementReplyDate,
    required this.status,
    required this.language,
    required this.additionalInfo,
  });

  /// <summary>
  /// إنشاء نموذج من JSON
  /// Create model from JSON
  /// </summary>
  factory ReviewModel.fromJson(Map<String, dynamic> json) {
    return ReviewModel(
      id: json['id'] ?? '',
      propertyId: json['propertyId'] ?? '',
      bookingId: json['bookingId'],
      reviewer: ReviewerModel.fromJson(json['reviewer'] ?? {}),
      overallRating: (json['overallRating'] ?? 0.0).toDouble(),
      subRatings: SubRatingModel.fromJson(json['subRatings'] ?? {}),
      title: json['title'] ?? '',
      content: json['content'] ?? '',
      images: (json['images'] as List<dynamic>?)
          ?.map((item) => ReviewImageModel.fromJson(item))
          .toList() ?? [],
      stayDate: json['stayDate'] != null 
          ? DateTime.parse(json['stayDate']) 
          : null,
      reviewDate: DateTime.parse(json['reviewDate'] ?? DateTime.now().toIso8601String()),
      stayType: json['stayType'],
      stayDurationNights: json['stayDurationNights'],
      isRecommended: json['isRecommended'] ?? false,
      likesCount: json['likesCount'] ?? 0,
      isHelpful: json['isHelpful'] ?? false,
      managementReply: json['managementReply'],
      managementReplyDate: json['managementReplyDate'] != null 
          ? DateTime.parse(json['managementReplyDate']) 
          : null,
      status: json['status'] ?? 'published',
      language: json['language'] ?? 'ar',
      additionalInfo: Map<String, dynamic>.from(json['additionalInfo'] ?? {}),
    );
  }

  /// <summary>
  /// تحويل النموذج إلى JSON
  /// Convert model to JSON
  /// </summary>
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'propertyId': propertyId,
      'bookingId': bookingId,
      'reviewer': reviewer.toJson(),
      'overallRating': overallRating,
      'subRatings': subRatings.toJson(),
      'title': title,
      'content': content,
      'images': images.map((item) => item.toJson()).toList(),
      'stayDate': stayDate?.toIso8601String(),
      'reviewDate': reviewDate.toIso8601String(),
      'stayType': stayType,
      'stayDurationNights': stayDurationNights,
      'isRecommended': isRecommended,
      'likesCount': likesCount,
      'isHelpful': isHelpful,
      'managementReply': managementReply,
      'managementReplyDate': managementReplyDate?.toIso8601String(),
      'status': status,
      'language': language,
      'additionalInfo': additionalInfo,
    };
  }

  /// <summary>
  /// تحويل إلى Entity
  /// Convert to Entity
  /// </summary>
  toEntity() {
    // سيتم تنفيذ هذا عند إنشاء Review entity
    return this;
  }

  @override
  List<Object?> get props => [
    id,
    propertyId,
    bookingId,
    reviewer,
    overallRating,
    subRatings,
    title,
    content,
    images,
    stayDate,
    reviewDate,
    stayType,
    stayDurationNights,
    isRecommended,
    likesCount,
    isHelpful,
    managementReply,
    managementReplyDate,
    status,
    language,
    additionalInfo,
  ];
}