/// كيان فلتر البحث - يمثل منطق العمل للبحث
/// منفصل عن نموذج البيانات لضمان نظافة البنية
class SearchFilter {
  /// معرف المدينة للبحث
  final String? city;
  
  /// الحد الأدنى للسعر
  final double? minPrice;
  
  /// الحد الأقصى للسعر
  final double? maxPrice;
  
  /// نوع العقار
  final String? propertyType;
  
  /// عدد الغرف
  final int? bedrooms;
  
  /// عدد الحمامات
  final int? bathrooms;
  
  /// عدد الضيوف
  final int? guests;
  
  /// تاريخ البداية
  final DateTime? checkIn;
  
  /// تاريخ النهاية
  final DateTime? checkOut;
  
  /// خط العرض للموقع
  final double? latitude;
  
  /// خط الطول للموقع
  final double? longitude;
  
  /// نصف قطر البحث بالكيلومتر
  final double? radius;
  
  /// هل العقار مميز
  final bool? isFeatured;
  
  /// تصنيف النجوم الأدنى
  final int? minStarRating;
  
  /// هل يسمح بالحيوانات الأليفة
  final bool? petsAllowed;
  
  /// هل يوجد إنترنت
  final bool? hasWifi;
  
  /// هل يوجد موقف سيارات
  final bool? hasParking;

  SearchFilter({
    this.city,
    this.minPrice,
    this.maxPrice,
    this.propertyType,
    this.bedrooms,
    this.bathrooms,
    this.guests,
    this.checkIn,
    this.checkOut,
    this.latitude,
    this.longitude,
    this.radius,
    this.isFeatured,
    this.minStarRating,
    this.petsAllowed,
    this.hasWifi,
    this.hasParking,
  });

  /// التحقق إذا كان الفلتر فارغاً
  bool get isEmpty => 
      city == null && 
      minPrice == null && 
      maxPrice == null && 
      propertyType == null &&
      bedrooms == null && 
      bathrooms == null && 
      guests == null &&
      checkIn == null && 
      checkOut == null &&
      latitude == null && 
      longitude == null &&
      radius == null &&
      isFeatured == null &&
      minStarRating == null &&
      petsAllowed == null &&
      hasWifi == null &&
      hasParking == null;

  /// التحقق إذا كان الفلتر يحتوي على قيم
  bool get isNotEmpty => !isEmpty;

  /// الحصول على نسخة من الفلتر مع إعادة تعيين جميع القيم
  SearchFilter copyWith({
    String? city,
    double? minPrice,
    double? maxPrice,
    String? propertyType,
    int? bedrooms,
    int? bathrooms,
    int? guests,
    DateTime? checkIn,
    DateTime? checkOut,
    double? latitude,
    double? longitude,
    double? radius,
    bool? isFeatured,
    int? minStarRating,
    bool? petsAllowed,
    bool? hasWifi,
    bool? hasParking,
  }) {
    return SearchFilter(
      city: city ?? this.city,
      minPrice: minPrice ?? this.minPrice,
      maxPrice: maxPrice ?? this.maxPrice,
      propertyType: propertyType ?? this.propertyType,
      bedrooms: bedrooms ?? this.bedrooms,
      bathrooms: bathrooms ?? this.bathrooms,
      guests: guests ?? this.guests,
      checkIn: checkIn ?? this.checkIn,
      checkOut: checkOut ?? this.checkOut,
      latitude: latitude ?? this.latitude,
      longitude: longitude ?? this.longitude,
      radius: radius ?? this.radius,
      isFeatured: isFeatured ?? this.isFeatured,
      minStarRating: minStarRating ?? this.minStarRating,
      petsAllowed: petsAllowed ?? this.petsAllowed,
      hasWifi: hasWifi ?? this.hasWifi,
      hasParking: hasParking ?? this.hasParking,
    );
  }

  /// إعادة تعيين جميع القيم
  SearchFilter reset() {
    return SearchFilter();
  }

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
  
    return other is SearchFilter &&
      other.city == city &&
      other.minPrice == minPrice &&
      other.maxPrice == maxPrice &&
      other.propertyType == propertyType &&
      other.bedrooms == bedrooms &&
      other.bathrooms == bathrooms &&
      other.guests == guests &&
      other.checkIn == checkIn &&
      other.checkOut == checkOut &&
      other.latitude == latitude &&
      other.longitude == longitude &&
      other.radius == radius &&
      other.isFeatured == isFeatured &&
      other.minStarRating == minStarRating &&
      other.petsAllowed == petsAllowed &&
      other.hasWifi == hasWifi &&
      other.hasParking == hasParking;
  }

  @override
  int get hashCode {
    return Object.hashAll([
      city,
      minPrice,
      maxPrice,
      propertyType,
      bedrooms,
      bathrooms,
      guests,
      checkIn,
      checkOut,
      latitude,
      longitude,
      radius,
      isFeatured,
      minStarRating,
      petsAllowed,
      hasWifi,
      hasParking,
    ]);
  }

  @override
  String toString() {
    return 'SearchFilter(city: $city, minPrice: $minPrice, maxPrice: $maxPrice, '
           'propertyType: $propertyType, bedrooms: $bedrooms, bathrooms: $bathrooms)';
  }
}