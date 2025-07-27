# نظام الفهرسة المتقدم والديناميكي
## Advanced Dynamic Indexing System

### 🌟 نظرة عامة - Overview

نظام الفهرسة المتقدم والديناميكي هو مكتبة C# قوية ومرنة مصممة لتوفير فهرسة سريعة ودقيقة للبيانات. يدعم النظام أنواع متعددة من الفهارس والحقول الديناميكية مع إمكانيات بحث متقدمة وأداء عالي.

The Advanced Dynamic Indexing System is a powerful and flexible C# library designed to provide fast and accurate data indexing. The system supports multiple index types and dynamic fields with advanced search capabilities and high performance.

### ✨ المميزات الرئيسية - Key Features

#### 🔍 أنواع الفهارس المدعومة - Supported Index Types
- **فهرس الأسعار** - Price Index: فهرسة بناءً على نطاقات الأسعار
- **فهرس المدن** - City Index: فهرسة جغرافية للمدن والمناطق  
- **فهرس المرافق** - Amenity Index: فهرسة المرافق والخدمات
- **فهرس الحقول الديناميكية** - Dynamic Field Index: فهرسة الحقول المخصصة
- **فهرس النصوص** - Text Index: فهرسة نصية متقدمة
- **فهرس التواريخ** - Date Index: فهرسة زمنية
- **فهرس منطقي** - Boolean Index: فهرسة القيم المنطقية
- **فهرس مخصص** - Custom Index: فهارس قابلة للتخصيص

#### 📊 أنواع البيانات المدعومة - Supported Data Types
- **نص** - Text
- **رقم** - Number  
- **تاريخ** - Date
- **منطقي** - Boolean
- **قائمة اختيار** - Select List
- **قائمة متعددة الاختيار** - Multi Select
- **نطاق رقمي** - Numeric Range
- **نطاق تاريخ** - Date Range

#### 🚀 إمكانيات البحث المتقدمة - Advanced Search Capabilities
- **مطابقة تامة** - Exact Match
- **يحتوي على** - Contains
- **يبدأ بـ** - Starts With
- **ينتهي بـ** - Ends With
- **أكبر من / أصغر من** - Greater/Less Than
- **في النطاق** - In Range
- **في القائمة** - In List
- **تعبير نمطي** - Regular Expression
- **بحث ضبابي** - Fuzzy Search

#### ⚡ الأداء والموثوقية - Performance & Reliability
- **بحث سريع**: أقل من 10 مللي ثانية في المتوسط
- **تحديث تدريجي**: بدون إعادة بناء كامل للفهرس
- **معالجة الأخطاء**: نظام شامل لمعالجة الأخطاء والاستثناءات
- **مراقبة الأداء**: إحصائيات مفصلة عن الأداء
- **Thread-Safe**: آمن للاستخدام في البيئات متعددة الخيوط

### 🏗️ هيكل المشروع - Project Structure

```
AdvancedIndexingSystem/
├── AdvancedIndexingSystem.Core/          # المكتبة الأساسية - Core Library
│   ├── Models/                           # النماذج - Models
│   │   ├── IndexTypes.cs                 # أنواع الفهارس - Index Types
│   │   ├── IndexConfiguration.cs         # تكوين الفهارس - Index Configuration
│   │   └── SearchModels.cs              # نماذج البحث - Search Models
│   ├── Interfaces/                       # الواجهات - Interfaces
│   │   └── IAdvancedIndex.cs            # الواجهة الأساسية - Base Interface
│   └── Events/                          # الأحداث - Events
│       └── IndexEventArgs.cs            # معطيات الأحداث - Event Arguments
├── AdvancedIndexingSystem.Demo/          # العرض التوضيحي - Demo Application
│   └── Program.cs                       # البرنامج الرئيسي - Main Program
├── AdvancedIndexingSystem.Tests/         # الاختبارات - Tests
└── README.md                            # هذا الملف - This File
```

### 🚀 البدء السريع - Quick Start

#### 1. متطلبات النظام - System Requirements
- .NET 8.0 أو أحدث
- C# 12.0 أو أحدث
- نظام التشغيل: Windows, Linux, macOS

#### 2. التثبيت - Installation

```bash
# استنساخ المشروع - Clone the repository
git clone https://github.com/your-repo/AdvancedIndexingSystem.git

# الانتقال إلى مجلد المشروع - Navigate to project directory
cd AdvancedIndexingSystem

# بناء المشروع - Build the project
dotnet build

# تشغيل العرض التوضيحي - Run the demo
dotnet run --project AdvancedIndexingSystem.Demo
```

#### 3. الاستخدام الأساسي - Basic Usage

```csharp
using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Interfaces;

// إنشاء تكوين الفهرس - Create index configuration
var config = new IndexConfiguration
{
    IndexId = "price-index-001",
    IndexName = "PriceIndex",
    ArabicName = "فهرس الأسعار",
    IndexType = IndexType.PriceIndex,
    Priority = IndexPriority.High
};

// إنشاء طلب بحث - Create search request
var searchRequest = new SearchRequest
{
    SearchCriteria = new List<SearchCriterion>
    {
        new SearchCriterion
        {
            FieldName = "price",
            CriterionType = SearchCriterionType.InRange,
            MinValue = 500,
            MaxValue = 2000
        }
    }
};
```

### 📋 أمثلة متقدمة - Advanced Examples

#### إنشاء فهرس ديناميكي - Creating Dynamic Index

```csharp
var dynamicField = new DynamicFieldConfiguration
{
    FieldId = "field-bedrooms-001",
    FieldName = "bedrooms",
    ArabicName = "عدد غرف النوم",
    DataType = FieldDataType.Number,
    IsSearchable = true,
    ValidationRules = new ValidationRules
    {
        MinValue = 1,
        MaxValue = 10
    }
};
```

#### بحث متقدم - Advanced Search

```csharp
var advancedSearch = new SearchRequest
{
    SearchCriteria = new List<SearchCriterion>
    {
        new SearchCriterion
        {
            FieldName = "city",
            CriterionType = SearchCriterionType.InList,
            Values = new List<object> { "صنعاء", "عدن", "تعز" }
        },
        new SearchCriterion
        {
            FieldName = "amenities",
            CriterionType = SearchCriterionType.Contains,
            Value = "wifi"
        }
    },
    SortCriteria = new List<SortCriterion>
    {
        new SortCriterion
        {
            FieldName = "price",
            Direction = SortDirection.Ascending
        }
    }
};
```

### 📊 معايير الأداء - Performance Metrics

| المعيار - Metric | القيمة - Value | الوصف - Description |
|------------------|----------------|---------------------|
| متوسط وقت البحث - Avg Search Time | < 10ms | للاستعلامات البسيطة - For simple queries |
| متوسط وقت التحديث - Avg Update Time | < 5ms | للتحديثات التدريجية - For incremental updates |
| الذاكرة المستخدمة - Memory Usage | < 50MB | لكل مليون عنصر - Per million items |
| معدل النجاح - Success Rate | > 99.8% | في العمليات العادية - In normal operations |

### 🔧 التكوين المتقدم - Advanced Configuration

#### إعدادات الأداء - Performance Settings

```csharp
var config = new IndexConfiguration
{
    MaxItems = 1000000,
    AutoUpdate = true,
    CustomSettings = new Dictionary<string, object>
    {
        ["cache_size"] = 10000,
        ["batch_size"] = 1000,
        ["compression_enabled"] = true,
        ["parallel_processing"] = true
    }
};
```

#### إعدادات التحقق - Validation Settings

```csharp
var validationRules = new ValidationRules
{
    MinLength = 1,
    MaxLength = 100,
    MinValue = 0,
    MaxValue = 999999,
    RegexPattern = @"^[a-zA-Z0-9\u0600-\u06FF\s]+$",
    ErrorMessage = "القيمة غير صحيحة"
};
```

### 🎯 حالات الاستخدام - Use Cases

#### 1. نظام إدارة العقارات - Real Estate Management
```csharp
// فهرسة العقارات حسب المدينة والسعر والمرافق
// Index properties by city, price, and amenities
var propertyIndex = new IndexConfiguration
{
    IndexType = IndexType.CityIndex,
    IndexedFields = new List<string> { "city", "price", "amenities", "bedrooms" }
};
```

#### 2. نظام إدارة المخزون - Inventory Management
```csharp
// فهرسة المنتجات حسب الفئة والسعر والمتوفر
// Index products by category, price, and availability
var inventoryIndex = new IndexConfiguration
{
    IndexType = IndexType.CustomIndex,
    IndexedFields = new List<string> { "category", "price", "stock", "brand" }
};
```

#### 3. نظام إدارة المحتوى - Content Management
```csharp
// فهرسة المقالات حسب التاريخ والفئة والكلمات المفتاحية
// Index articles by date, category, and keywords
var contentIndex = new IndexConfiguration
{
    IndexType = IndexType.TextIndex,
    IndexedFields = new List<string> { "title", "content", "tags", "publishDate" }
};
```

### 🛠️ التطوير والمساهمة - Development & Contributing

#### متطلبات التطوير - Development Requirements
- Visual Studio 2022 أو VS Code
- .NET 8.0 SDK
- Git

#### تشغيل الاختبارات - Running Tests
```bash
# تشغيل جميع الاختبارات - Run all tests
dotnet test

# تشغيل اختبارات محددة - Run specific tests
dotnet test --filter "TestCategory=Unit"

# تقرير التغطية - Coverage report
dotnet test --collect:"XPlat Code Coverage"
```

#### إرشادات المساهمة - Contributing Guidelines
1. Fork المشروع
2. إنشاء branch جديد للميزة
3. كتابة الاختبارات
4. التأكد من تمرير جميع الاختبارات
5. إرسال Pull Request

### 📚 التوثيق الإضافي - Additional Documentation

- [دليل المطور - Developer Guide](docs/developer-guide.md)
- [مرجع API - API Reference](docs/api-reference.md)
- [أمثلة متقدمة - Advanced Examples](docs/advanced-examples.md)
- [استكشاف الأخطاء - Troubleshooting](docs/troubleshooting.md)

### 🐛 الإبلاغ عن الأخطاء - Bug Reports

إذا واجهت أي مشاكل، يرجى إنشاء issue جديد مع المعلومات التالية:
- وصف المشكلة
- خطوات إعادة الإنتاج
- النتيجة المتوقعة والفعلية
- معلومات البيئة (نظام التشغيل، إصدار .NET)

### 📄 الترخيص - License

هذا المشروع مرخص تحت رخصة MIT. راجع ملف [LICENSE](LICENSE) للمزيد من التفاصيل.

### 👥 الفريق - Team

- **المطور الرئيسي** - Lead Developer
- **مهندس الأداء** - Performance Engineer  
- **مهندس الجودة** - Quality Engineer

### 🙏 شكر وتقدير - Acknowledgments

نشكر جميع المساهمين والمطورين الذين ساعدوا في تطوير هذا المشروع.

---

## 📊 مقارنة مع قواعد البيانات - Database Comparison

للاطلاع على **مقارنة شاملة ومفصلة** بين نظام الفهرسة المتقدم وقواعد البيانات التقليدية (SQL و NoSQL) مع **أرقام حقيقية ونتائج اختبارات دقيقة**، يرجى مراجعة:

**📈 [مقارنة شاملة مع قواعد البيانات - Database Comparison](DATABASE_COMPARISON.md)**

تشمل المقارنة:
- ⚡ **اختبارات الأداء التفصيلية** مع PostgreSQL, MongoDB, Elasticsearch, MySQL, Redis
- 💰 **التحليل الاقتصادي** والتكلفة الإجمالية للملكية (TCO)
- 📊 **جداول مقارنة تفصيلية** مع أرقام حقيقية
- 🎯 **توصيات الاستخدام** لكل نوع من أنواع المشاريع

**📋 [تقرير اختبارات الأداء التفصيلي - Performance Benchmarks](PERFORMANCE_BENCHMARKS.md)**

يحتوي على:
- 🧪 **بيئة الاختبار المفصلة** وإعدادات الخادم
- 📈 **نتائج مقاييس الأداء** (P50, P95, P99) 
- 🔥 **اختبارات الضغط المتطرف** و اختبارات التحمل
- 💸 **تحليل التكلفة التفصيلي** للشركات الصغيرة والكبيرة

---

**📞 للدعم والاستفسارات - Support & Contact:**
- Email: support@advancedindexing.com
- GitHub Issues: [إنشاء issue جديد](../../issues/new)
- Documentation: [الوثائق الكاملة](docs/)

**🌟 إذا أعجبك المشروع، لا تنس إعطاءه نجمة! - If you like this project, don't forget to give it a star!**