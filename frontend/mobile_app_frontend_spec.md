# تطبيق حجز اليمن: مواصفات الواجهة الأمامية للشاشة الرئيسية الديناميكية

## 1. مقدمة

يقدم هذا المستند مواصفات فنية شاملة لبناء واجهة المستخدم (UI) للشاشة الرئيسية الديناميكية لتطبيق "حجز اليمن". الشاشة الرئيسية ليست ثابتة؛ بل يتم عرضها ديناميكيًا بناءً على إعدادات وسلسلة من "الأقسام" التي يتم جلبها من الواجهة الخلفية (Backend API).

الغرض من هذا المستند هو أن يكون المصدر الوحيد الموثوق لمطوري الواجهة الأمامية. حيث يفصّل هياكل البيانات، ونماذج الكيانات، والتعدادات (Enums)، ومنطق العرض الذي يتوقعه تطبيق الموبايل. الالتزام بهذه المواصفات سيضمن التوافق الكامل مع خدمات الواجهة الخلفية وبنية تطبيق الموبايل.

## 2. المفاهيم الأساسية

تتحدد بنية الشاشة الرئيسية من خلال ثلاثة مكونات رئيسية:

1.  **إعدادات الشاشة الرئيسية (`HomeConfig`)**: كائن عام يحدد السلوك العام، والمظهر (Theme)، والإعدادات الخاصة بالشاشة الرئيسية (مثل فترات التحديث، وقواعد التخزين المؤقت، والأنماط العامة).
2.  **أقسام الشاشة الرئيسية (`HomeSection`)**: قائمة مرتبة من كتل المحتوى التي تشكل الشاشة الرئيسية. كل قسم له `type` (نوع) محدد (على سبيل المثال، قائمة أفقية للعقارات، شبكة للمدن، إعلان مميز) وله إعداداته الخاصة.
3.  **المحتوى الديناميكي (`DynamicContent`)**: عناصر البيانات الفعلية التي يتم عرضها داخل القسم (على سبيل المثال، عقار معين، عرض خاص، وجهة سياحية).

تدفق عملية العرض هو كما يلي:
1.  يقوم تطبيق الموبايل أولاً بجلب `HomeConfig`.
2.  ثم يقوم بجلب قائمة كائنات `HomeSection`.
3.  يتصفح التطبيق قائمة `HomeSection`، وبناءً على `type` (نوع) كل قسم، يقوم بعرض مكون واجهة المستخدم المناسب.
4.  يتم ملء كل مكون واجهة مستخدم بالبيانات من مصفوفة `content` الموجودة داخل كائن `HomeSection` المقابل له.

---

## 3. مواصفات نماذج البيانات والكيانات

يفصّل هذا القسم كل نموذج بيانات يتلقاه تطبيق الموبايل من الواجهة الخلفية.

### 3.1. كيان `HomeConfig`

يحدد هذا الكيان الإعدادات العامة للشاشة الرئيسية.

| الخاصية | النوع | الوصف | مثال |
| :--- | :--- | :--- | :--- |
| `id` | `String` | معرّف فريد للإعدادات. | `"config_v1.2"` |
| `version` | `String` | سلسلة إصدار هذه الإعدادات. | `"1.2.0"` |
| `isActive` | `bool` | ما إذا كانت هذه الإعدادات نشطة حاليًا. | `true` |
| `publishedAt` | `DateTime?` | الطابع الزمني لنشر هذه الإعدادات. | `"2025-08-04T10:00:00Z"` |
| `globalSettings`| `Map<String, dynamic>` | أزواج (مفتاح-قيمة) للسلوك العام للتطبيق. | `{"refreshInterval": 300}` |
| `themeSettings` | `Map<String, dynamic>` | أزواج (مفتاح-قيمة) للمظهر العام للتطبيق. | `{"primaryColor": "#2E7D32"}` |
| `layoutSettings`| `Map<String, dynamic>` | أزواج (مفتاح-قيمة) لقواعد التخطيط العامة. | `{"sectionSpacing": 16.0}` |
| `cacheSettings` | `Map<String, dynamic>` | أزواج (مفتاح-قيمة) لاستراتيجيات التخزين المؤقت. | `{"maxAge": 3600}` |
| `analyticsSettings`| `Map<String, dynamic>`| أزواج (مفتاح-قيمة) لتتبع التحليلات. | `{"enabled": true}` |
| `enabledFeatures`| `List<String>` | قائمة بالميزات المفعلة. | `["chat", "booking"]` |
| `experimentalFeatures`| `Map<String, dynamic>`| أزواج (مفتاح-قيمة) للميزات التجريبية. | `{"newSearchUI": true}` |

### 3.2. كيان `HomeSection`

هذا هو الكيان الرئيسي لكتلة محتوى واحدة على الشاشة الرئيسية.

| الخاصية | النوع | الوصف | مثال |
| :--- | :--- | :--- | :--- |
| `id` | `String` | معرّف فريد للقسم. | `"sec_featured_properties"` |
| `type` | `SectionType` (Enum) | **مهم جدًا**: نوع القسم، الذي يحدد مكون واجهة المستخدم الذي سيتم عرضه. انظر القسم 4.1 لجميع القيم الممكنة. | `"HORIZONTAL_PROPERTY_LIST"` |
| `order` | `int` | ترتيب عرض القسم على الشاشة (يبدأ من 0). | `0` |
| `isActive` | `bool` | ما إذا كان يجب عرض هذا القسم. | `true` |
| `title` | `String?` | العنوان المعروض للقسم (مثل "عقارات مميزة"). | `"عقارات مميزة"` |
| `subtitle` | `String?` | العنوان الفرعي المعروض للقسم. | `"توصياتنا لك"` |
| `config` | `SectionConfig` | الإعدادات التفصيلية لهذا القسم المحدد. انظر القسم 3.3. | `{...}` |
| `content` | `List<DynamicContent>` | **مهم جدًا**: قائمة عناصر المحتوى لعرضها داخل هذا القسم. انظر القسم 3.4. | `[...]` |
| `metadata` | `Map<String, dynamic>` | أي بيانات وصفية إضافية للقسم. | `{"source": "manual"}` |
| `scheduledAt` | `DateTime?` | إذا تم تعيينه، فلن يكون القسم مرئيًا إلا بعد هذا الوقت. | `null` |
| `expiresAt` | `DateTime?` | إذا تم تعيينه، فسيتم إخفاء القسم بعد هذا الوقت. | `null` |
| `targetAudience`| `List<String>` | يحدد شرائح المستخدمين التي يجب أن ترى هذا القسم. | `["new_users", "has_favorites"]` |
| `priority` | `int` | رقم أولوية للترتيب أو الفرز. | `100` |

### 3.3. كيان `SectionConfig`

يوفر هذا الكيان، المتداخل داخل `HomeSection`، إعدادات مفصلة لكيفية ظهور القسم وتصرفه.

| الخاصية | النوع | الوصف | مثال |
| :--- | :--- | :--- | :--- |
| `id` | `String` | معرّف فريد لكائن الإعدادات نفسه. | `"cfg_hlist_1"` |
| `sectionType` | `SectionType` (Enum) | نوع القسم الذي تنطبق عليه هذه الإعدادات. | `"HORIZONTAL_PROPERTY_LIST"` |
| `displaySettings`| `Map<String, dynamic>` | يتحكم في ما يتم عرضه. | `{"showBadge": true}` |
| `layoutSettings` | `Map<String, dynamic>` | يتحكم في تخطيط المكون. | `{"itemSpacing": 8.0}` |
| `styleSettings` | `Map<String, dynamic>` | يتحكم في النمط المرئي. | `{"borderRadius": 12.0}` |
| `behaviorSettings`| `Map<String, dynamic>` | يتحكم في السلوك التفاعلي. | `{"autoPlay": true}` |
| `animationSettings`| `Map<String, dynamic>`| يتحكم في الرسوم المتحركة. | `{"animationType": "FADE"}` |
| `cacheSettings` | `Map<String, dynamic>` | قواعد التخزين المؤقت لمحتوى هذا القسم. | `{"enableCache": true}` |
| `propertyIds` | `List<String>` | قائمة بمعرفات العقارات ليتم جلبها وعرضها ديناميكيًا. | `["prop_1", "prop_2"]` |
| `title` | `String?` | العنوان باللغة الإنجليزية (يتجاوز عنوان القسم). | `"Featured"` |
| `titleAr` | `String?` | العنوان باللغة العربية (يتجاوز عنوان القسم). | `"مميز"` |
| `subtitle` | `String?` | العنوان الفرعي باللغة الإنجليزية. | `"Our recommendations"` |
| `subtitleAr` | `String?` | العنوان الفرعي باللغة العربية. | `"توصياتنا"` |
| `backgroundColor`| `String?` | رمز اللون السداسي لخلفية القسم. | `"#F5F5F5"` |
| `textColor` | `String?` | رمز اللون السداسي لنص القسم. | `"#212121"` |
| `customImage` | `String?` | رابط لصورة خلفية أو بانر مخصص. | `"https://.../banner.png"` |
| `customData` | `Map<String, dynamic>` | أي بيانات مخصصة أخرى. | `{}` |

### 3.4. كيان `DynamicContent`

يمثل هذا الكيان عنصرًا واحدًا داخل قائمة `content` الخاصة بالقسم.

| الخاصية | النوع | الوصف | مثال |
| :--- | :--- | :--- | :--- |
| `id` | `String` | معرّف فريد لعنصر المحتوى. | `"prop_123"` |
| `sectionId` | `String` | معرّف القسم الذي ينتمي إليه هذا المحتوى. | `"sec_featured_properties"` |
| `contentType` | `String` | **مهم جدًا**: نوع البيانات الموجودة في حقل `data`. يمكن أن يكون `PROPERTY`، `OFFER`، `ADVERTISEMENT`، `DESTINATION`، إلخ. | `"PROPERTY"` |
| `data` | `Map<String, dynamic>` | **مهم جدًا**: حمولة البيانات الفعلية. يعتمد هيكل هذه الخريطة على `contentType`. سوف يتطابق مع أحد الكيانات الرئيسية مثل `FeaturedProperty`، `CityDestination`، إلخ. | `{ "id": "prop_123", "name": "...", ... }` |
| `metadata` | `Map<String, dynamic>` | بيانات وصفية إضافية لعنصر المحتوى هذا. | `{"score": 0.98}` |
| `expiresAt` | `DateTime?` | إذا تم تعيينه، فسينتهي صلاحية عنصر المحتوى هذا. | `null` |
| `createdAt` | `DateTime` | الطابع الزمني للإنشاء. | `"2025-08-04T10:00:00Z"` |
| `updatedAt` | `DateTime` | الطابع الزمني لآخر تحديث. | `"2025-08-04T10:00:00Z"` |

### 3.5. كيان `FeaturedProperty`

هذا هو الكيان الرئيسي لتمثيل عقار. يتم استخدامه في معظم أقسام القوائم.

| الخاصية | النوع | الوصف | مثال |
| :--- | :--- | :--- | :--- |
| `id` | `String` | معرّف فريد للعقار. | `"prop_12345"` |
| `name` | `String` | اسم العقار. | `"فندق وأجنحة سبأ"` |
| `address` | `String` | عنوان الشارع للعقار. | `"شارع الزبيري، صنعاء"` |
| `city` | `String` | المدينة التي يقع فيها العقار. | `"صنعاء"` |
| `latitude` | `double` | خط العرض GPS. | `15.3547` |
| `longitude` | `double` | خط الطول GPS. | `44.2068` |
| `starRating` | `int` | تصنيف النجوم للعقار (1-5). | `4` |
| `description` | `String` | وصف موجز للعقار. | `"فندق فاخر بقلب العاصمة..."` |
| `images` | `List<String>` | قائمة بعناوين URL لصور العقار. | `["url1.jpg", "url2.jpg"]` |
| `basePrice` | `double?` | السعر الأساسي لليلة الواحدة. | `25000.0` |
| `currency` | `String?` | رمز العملة. | `"YER"` |
| `amenities` | `List<String>` | قائمة بأسماء أبرز وسائل الراحة. | `["Wi-Fi", "Parking", "Pool"]` |
| `averageRating`| `double` | متوسط تقييم المستخدمين (0.0 - 5.0). | `4.7` |
| `viewCount` | `int` | عدد مرات مشاهدة العقار. | `1205` |
| `bookingCount`| `int` | عدد مرات حجز العقار. | `88` |
| `mainImageUrl`| `String?` | عنوان URL للصورة الرئيسية لعرضها في القوائم. | `"url1.jpg"` |
| `isFeatured` | `bool` | يشير إلى ما إذا كان العقار مميزًا. | `true` |
| `discountPercentage`| `double?` | نسبة الخصم، إن وجدت. | `15.0` |
| `propertyType`| `String?` | نوع العقار (مثل "فندق"، "شقة"). | `"Hotel"` |
| `featuredReason`| `String?` | سبب تمييز العقار. | `"Popular Choice"` |
| `badgeText` | `String?` | نص لشارة ترويجية (مثل "تخفيض"). | `"عرض خاص"` |
| `badgeColor` | `String?` | رمز اللون السداسي لخلفية الشارة. | `"#FF6F00"` |
| `promotionalMessage`| `String?` | رسالة ترويجية قصيرة. | `"خصم 15% لفترة محدودة"` |

### 3.6. كيان `CityDestination`

يمثل مدينة يمكن تمييزها كوجهة سياحية.

| الخاصية | النوع | الوصف | مثال |
| :--- | :--- | :--- | :--- |
| `id` | `String` | معرّف فريد للمدينة. | `"city_sanaa"` |
| `name` | `String` | اسم المدينة باللغة الإنجليزية. | `"Sana'a"` |
| `nameAr` | `String` | اسم المدينة باللغة العربية. | `"صنعاء"` |
| `country` | `String` | اسم البلد باللغة الإنجليزية. | `"Yemen"` |
| `countryAr` | `String` | اسم البلد باللغة العربية. | `"اليمن"` |
| `imageUrl` | `String` | عنوان URL لصورة تمثل المدينة. | `"sanaa_image.jpg"` |
| `propertyCount`| `int` | عدد العقارات المتاحة في تلك المدينة. | `250` |
| `averagePrice` | `double` | متوسط سعر العقارات في المدينة. | `18000.0` |
| `isPopular` | `bool` | ما إذا كانت هذه وجهة شائعة. | `true` |
| `isFeatured` | `bool` | ما إذا كانت هذه الوجهة مميزة. | `false` |

### 3.7. كيان `SponsoredAd`

يمثل إعلانًا ممولاً يمكن أن يظهر كقسم.

| الخاصية | النوع | الوصف | مثال |
| :--- | :--- | :--- | :--- |
| `id` | `String` | معرّف فريد للإعلان. | `"ad_summer_promo"` |
| `title` | `String` | العنوان الرئيسي للإعلان. | `"عروض الصيف"` |
| `subtitle` | `String?` | العنوان الفرعي للإعلان. | `"خصومات تصل إلى 30%"` |
| `property` | `PropertySummary?` | ملخص لعقار واحد مرتبط. | `{ "id": "prop_5", "name": "..." }` |
| `propertyIds` | `List<String>` | قائمة بمعرفات العقارات التي يتعلق بها هذا الإعلان. | `["prop_5", "prop_6"]` |
| `customImageUrl`| `String?` | صورة بانر مخصصة للإعلان. | `"ad_banner.png"` |
| `backgroundColor`| `String?` | رمز اللون السداسي لخلفية الإعلان. | `"#E3F2FD"` |
| `ctaText` | `String` | نص زر الحث على اتخاذ إجراء (Call-to-action). | `"اكتشف الآن"` |
| `ctaAction` | `String` | الإجراء الذي سيتم تنفيذه عند النقر (مثل "navigate"، "open_url"). | `"navigate"` |
| `ctaData` | `Map<String, dynamic>` | بيانات لإجراء الحث (مثل `{"route": "/offers"}`). | `{"route": "/offers/summer"}` |
| `startDate` | `DateTime` | تاريخ بدء تفعيل الإعلان. | `"2025-08-01T00:00:00Z"` |
| `endDate` | `DateTime` | تاريخ انتهاء صلاحية الإعلان. | `"2025-09-01T00:00:00Z"` |

---

## 4. التعدادات (Enums)

يجب على تطبيق الموبايل تفسير هذه التعدادات بشكل صحيح لعرض واجهة المستخدم على النحو المنشود.

### 4.1. تعداد `SectionType`

هذا هو التعداد الأكثر أهمية. فهو يحدد مكون واجهة المستخدم الذي سيتم عرضه لـ `HomeSection`.

| القيمة | مكون واجهة المستخدم المقترح | الوصف |
| :--- | :--- | :--- |
| `SINGLE_PROPERTY_AD` | `FeaturedPropertyCard` | بطاقة كبيرة وبارزة لعقار واحد ممول. غالبًا ما تستخدم تأثيرات اختلاف المنظر (Parallax). |
| `FEATURED_PROPERTY_AD` | `FeaturedPropertyCard` | مشابه لـ `SINGLE_PROPERTY_AD` ولكن قد يحتوي على شارة "مميز". |
| `MULTI_PROPERTY_AD` | `PropertiesGrid` | شبكة (مثل 2x2) تعرض عقارات ممولة متعددة. |
| `UNIT_SHOWCASE_AD` | `UnitShowcaseWidget` | مكون خاص لعرض *وحدات* محددة داخل عقار. |
| `SINGLE_PROPERTY_OFFER` | `OfferCard` | بطاقة تعرض عرضًا خاصًا لعقار واحد. |
| `LIMITED_TIME_OFFER` | `OfferCard` مع عداد تنازلي | بطاقة عرض تتضمن عدادًا تنازليًا لخلق شعور بالإلحاح. |
| `SEASONAL_OFFER` | `ThemedOfferCard` | بطاقة عرض ذات طابع موسمي (مثل الصيف، رمضان). |
| `MULTI_PROPERTY_OFFERS_GRID`| `OffersGrid` | شبكة تعرض عروضًا من عقارات متعددة. |
| `OFFERS_CAROUSEL` | `CarouselSlider` of `OfferCard` | شريط تمرير أفقي لبطاقات العروض. |
| `FLASH_DEALS` | `FlashDealsCarousel` | شريط تمرير للصفقات العاجلة قصيرة الأجل، غالبًا مع مؤقت بارز. |
| `HORIZONTAL_PROPERTY_LIST` | `HorizontalListView` of `PropertyCard` | قائمة قياسية قابلة للتمرير أفقيًا للعقارات. |
| `VERTICAL_PROPERTY_GRID` | `VerticalGridView` of `PropertyCard` | شبكة قابلة للتمرير عموديًا للعقارات (مثل عمودين). |
| `MIXED_LAYOUT_LIST` | `ComplexListView` | قائمة عمودية بعناصر ذات أحجام وتخطيطات متفاوتة لإضفاء جاذبية بصرية. |
| `COMPACT_PROPERTY_LIST` | `CompactListView` of `CompactPropertyCard` | قائمة عمودية تستخدم بطاقات أصغر وأكثر إحكامًا لعرض المزيد من العناصر. |
| `FEATURED_PROPERTIES_SHOWCASE`| `LargeCarouselSlider` | شريط تمرير كبير وغني بصريًا لعرض العقارات من الدرجة الأولى. |
| `CITY_CARDS_GRID` | `GridView` of `CityCard` | شبكة تعرض وجهات المدن. |
| `DESTINATION_CAROUSEL` | `CarouselSlider` of `CityCard` | شريط تمرير أفقي لوجهات المدن. |
| `EXPLORE_CITIES` | `LargeCityList` | قائمة أو شبكة جذابة بصريًا لاستكشاف المدن. |
| `PREMIUM_CAROUSEL` | `PremiumCarousel` | شريط تمرير مصمم بشكل كبير، ربما بتأثيرات ثلاثية الأبعاد أو انتقالات فريدة. |
| `INTERACTIVE_SHOWCASE` | `InteractiveWidget` | قسم يمكن أن يحتوي على عناصر تفاعلية مثل خريطة أو اختبار. |

### 4.2. تعداد `SectionAnimation`

يحدد الرسوم المتحركة لدخول قسم أو عناصره.

| القيمة | الوصف |
| :--- | :--- |
| `NONE` | لا توجد رسوم متحركة. |
| `FADE` | تأثير ظهور تدريجي بسيط. |
| `SLIDE` | تنزلق العناصر من الجانب أو الأسفل. |
| `SCALE` | تتوسع العناصر من حجم أصغر. |
| `ROTATE` | تدور العناصر إلى العرض. |
| `PARALLAX` | تتحرك صورة الخلفية بسرعة مختلفة عن محتوى المقدمة. |
| `SHIMMER` | تأثير لامع يستخدم كعنصر نائب للتحميل. |
| `PULSE` | تأثير نبض خفي لجذب الانتباه. |
| `BOUNCE` | ترتد العناصر إلى مكانها. |
| `FLIP` | تنقلب العناصر إلى العرض. |

### 4.3. تعداد `SectionSize`

يحدد حجمًا نسبيًا للقسم، مما قد يؤثر على الحشو وأحجام الخطوط وارتفاعات العناصر.

| القيمة | الوصف |
| :--- | :--- |
| `COMPACT` | أصغر من المعتاد، للمعلومات الكثيفة. |
| `SMALL` | أصغر قليلاً من الافتراضي. |
| `MEDIUM` | الحجم القياسي الافتراضي. |
| `LARGE` | أكبر من الافتراضي، للتأكيد. |
| `EXTRA_LARGE` | كبير جدًا، للأقسام الرئيسية (Hero sections). |
| `FULL_SCREEN` | يشغل منفذ العرض بالكامل. |

---

## 5. منطق عرض مكونات واجهة المستخدم

يقدم هذا القسم إرشادات حول كيفية عرض واجهة المستخدم بناءً على البيانات.

### 5.1. تدفق العرض العام

1.  **إنشاء ويدجت `HomeScreen`**: سيكون هذا الويدجت مسؤولاً عن جلب البيانات وإدارة الحالة (على سبيل المثال، باستخدام `flutter_bloc`).
2.  **استخدام `ListView.builder` أو `CustomScrollView`** كهيئة رئيسية قابلة للتمرير في `HomeScreen`.
3.  سيكون `itemCount` للقائمة هو طول قائمة `HomeSection` من الواجهة البرمجية.
4.  سيتلقى `itemBuilder` كائن `HomeSection` لكل فهرس.
5.  **داخل `itemBuilder`، استخدم عبارة `switch` على `section.type` (تعداد `SectionType`).**
6.  ستعيد كل `case` في الـ `switch` ويدجت مكون واجهة مستخدم محددًا يتوافق مع نوع هذا القسم (على سبيل المثال، `case SectionType.horizontalPropertyList: return HorizontalPropertyListWidget(section: section);`).

### 5.2. منطق خاص بالمكونات

#### `HorizontalPropertyListWidget` (`HORIZONTAL_PROPERTY_LIST`)
-   يجب أن يحتوي على `Column` به عنوان (`section.title`) و `ListView.builder` قابل للتمرير أفقيًا.
-   `itemCount` الخاص بـ `ListView` هو `section.content.length`.
-   كل عنصر في `ListView` هو ويدجت `PropertyCard`.
-   تأتي بيانات `PropertyCard` من `section.content[index].data`، والتي يجب تحويلها إلى خريطة `FeaturedProperty`.

#### `VerticalPropertyGridWidget` (`VERTICAL_PROPERTY_GRID`)
-   يجب أن يحتوي على عنوان و `GridView.builder`.
-   استخدم `SliverGridDelegateWithFixedCrossAxisCount` مع `crossAxisCount` يتم تعيينه عادةً على 2.
-   `itemCount` هو `section.content.length`.
-   كل عنصر هو `PropertyCard`.

#### `OffersCarouselWidget` (`OFFERS_CAROUSEL`)
-   استخدم حزمة خارجية مثل `carousel_slider`.
-   العناصر في شريط التمرير هي ويدجات `OfferCard`.
-   يجب أن تعرض `OfferCard` عنوان العرض والخصم ومؤقت العد التنازلي إذا كان `type` هو `LIMITED_TIME_OFFER`. يمكن حساب الوقت المتبقي من حقل `expiresAt` في `data` الخاص بـ `DynamicContent`.

#### `CityCardsGridWidget` (`CITY_CARDS_GRID`)
-   مشابه لشبكة العقارات، ولكنه يستخدم ويدجات `CityCard`.
-   يجب أن تعرض `CityCard` صورة المدينة (`imageUrl`) واسمها العربي (`nameAr`) وعدد العقارات (`propertyCount`).

### 5.3. التعامل مع الترجمة (Localization)

-   يستخدم تطبيق الموبايل فئة `AppLocalizations` لتحديد اللغة الحالية (مثل `isArabic`).
-   عند عرض نص من الواجهة البرمجية، تحقق دائمًا من الإصدار المترجم أولاً.
-   **مثال:** بالنسبة لـ `CityDestination`، يجب أن يكون الاسم المراد عرضه هو `isArabic ? city.nameAr : city.name`.

---

## 6. ملخص نقاط النهاية (API Endpoints) (للسياق)

هذا ملخص لنقاط نهاية الواجهة البرمجية التي سيستدعيها تطبيق الموبايل.

-   **`GET /api/client/HomeSections/config`**
    -   **الوصف:** يجلب الإعدادات العامة للشاشة الرئيسية.
    -   **الاستجابة:** كائن `HomeConfig` واحد.

-   **`GET /api/client/HomeSections/sections`**
    -   **الوصف:** يجلب القائمة المرتبة للأقسام لعرضها على الشاشة الرئيسية.
    -   **الاستجابة:** `List<HomeSection>`.

-   **`GET /api/client/HomeSections/sponsored-ads`**
    -   **الوصف:** يجلب قائمة بالإعلانات الممولة النشطة.
    -   **الاستجابة:** `List<SponsoredAd>`.

-   **`GET /api/client/HomeSections/destinations`**
    -   **الوصف:** يجلب قائمة بوجهات المدن.
    -   **الاستجابة:** `List<CityDestination>`.

-   **`POST /api/client/HomeSections/sponsored-ads/{adId}/impression`**
    -   **الوصف:** يسجل أن المستخدم قد شاهد إعلانًا معينًا. يجب استدعاء هذا عندما يكون قسم الإعلان مرئيًا على الشاشة لمدة معينة (على سبيل المثال، ثانيتان).

-   **`POST /api/client/HomeSections/sponsored-ads/{adId}/click`**
    -   **الوصف:** يسجل أن المستخدم قد نقر على إعلان معين. يجب استدعاء هذا عندما ينقر المستخدم على مكون إعلان ممول.

---

## 7. سيناريو مثال: عرض شاشة رئيسية

**1. يستدعي تطبيق الموبايل `GET /api/client/HomeSections/sections` ويتلقى JSON التالي:**

```json
[
  {
    "id": "sec_1",
    "type": "FEATURED_PROPERTIES_SHOWCASE",
    "order": 0,
    "isActive": true,
    "title": "عقارات مميزة",
    "config": { "autoPlay": true, "layoutType": "carousel" },
    "content": [
      {
        "id": "prop_101",
        "contentType": "PROPERTY",
        "data": {
          "id": "prop_101",
          "name": "فندق موفنبيك صنعاء",
          "mainImageUrl": "movenpick.jpg",
          "city": "صنعاء",
          "averageRating": 4.8,
          "basePrice": 45000,
          "currency": "YER"
        }
      },
      {
        "id": "prop_102",
        "contentType": "PROPERTY",
        "data": {
          "id": "prop_102",
          "name": "فندق شمر",
          "mainImageUrl": "shemar.jpg",
          "city": "صنعاء",
          "averageRating": 4.5,
          "basePrice": 20000,
          "currency": "YER"
        }
      }
    ]
  },
  {
    "id": "sec_2",
    "type": "CITY_CARDS_GRID",
    "order": 1,
    "isActive": true,
    "title": "استكشف المدن",
    "config": { "layoutType": "grid", "columnsCount": 2 },
    "content": [
      {
        "id": "city_sanaa",
        "contentType": "DESTINATION",
        "data": {
          "id": "city_sanaa",
          "nameAr": "صنعاء",
          "imageUrl": "sanaa.jpg",
          "propertyCount": 250
        }
      },
      {
        "id": "city_aden",
        "contentType": "DESTINATION",
        "data": {
          "id": "city_aden",
          "nameAr": "عدن",
          "imageUrl": "aden.jpg",
          "propertyCount": 180
        }
      }
    ]
  }
]
```

**2. `HomeScreen` في التطبيق ستقوم بما يلي:**
-   إنشاء `ListView` عمودي.
-   **للعنصر الأول (الفهرس 0):**
    -   ترى أن `type` هو `FEATURED_PROPERTIES_SHOWCASE`.
    -   تقوم بعرض ويدجت `FeaturedShowcaseWidget`.
    -   سيكون هذا الويدجت عبارة عن شريط تمرير كبير يعمل تلقائيًا.
    -   سيحتوي على شريحتين، واحدة لـ "فندق موفنبيك صنعاء" وأخرى لـ "فندق شمر"، باستخدام البيانات من مصفوفة `content`.
-   **للعنصر الثاني (الفهرس 1):**
    -   ترى أن `type` هو `CITY_CARDS_GRID`.
    -   تقوم بعرض ويدجت `CityGridWidget`.
    -   سيكون هذا الويدجت عبارة عن `GridView` بعمودين.
    -   سيحتوي على ويدجتي `CityCard`، واحدة لـ "صنعاء" وأخرى لـ "عدن".

هذا النهج الديناميكي يسمح للواجهة الخلفية بالتحكم الكامل في تخطيط ومحتوى الشاشة الرئيسية دون الحاجة إلى تحديث التطبيق.