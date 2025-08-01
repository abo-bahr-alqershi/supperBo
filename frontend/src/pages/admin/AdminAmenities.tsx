import { useState } from 'react';
import { useAdminAmenities } from '../../hooks/useAdminAmenities';
import { useAdminProperties } from '../../hooks/useAdminProperties';
import DataTable, { type Column } from '../../components/common/DataTable';
import SearchAndFilter, { type FilterOption } from '../../components/common/SearchAndFilter';
import Modal from '../../components/common/Modal';
import PropertySelector from '../../components/selectors/PropertySelector';
import type {
  AmenityDto,
  CreateAmenityCommand,
  UpdateAmenityCommand,
  GetAllAmenitiesQuery,
  AssignAmenityToPropertyCommand,
  UpdatePropertyAmenityCommand,
  MoneyDto
} from '../../types/amenity.types';
import CurrencyInput from '../../components/inputs/CurrencyInput';
import { useCurrencies } from '../../hooks/useCurrencies';

const AdminAmenities = () => {
  // Fetch currencies for extra cost
  const { currencies, loading: currenciesLoading } = useCurrencies();
  const currencyCodes = currenciesLoading ? [] : currencies.map(c => c.code);

  // استخدام الهوكات لإدارة البيانات والعمليات
  const [searchTerm, setSearchTerm] = useState('');
  const [showAdvancedFilters, setShowAdvancedFilters] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [filterValues, setFilterValues] = useState<Record<string, any>>({ category: '', isAssigned: undefined, propertyId: undefined, isFree: undefined });

  // بناء معايير الاستعلام
  const queryParams: GetAllAmenitiesQuery = {
    pageNumber: currentPage,
    pageSize,
    searchTerm: searchTerm || undefined,
    propertyId: filterValues.propertyId || undefined,
    isAssigned: filterValues.isAssigned,
    isFree: filterValues.isFree
  };
  
  // استعلام المرافق عبر هوك مخصص
  const {
    amenitiesData,
    isLoading: isLoadingAmenities,
    error: amenitiesError,
    createAmenity,
    updateAmenity,
    deleteAmenity,
    assignAmenityToProperty,
  } = useAdminAmenities(queryParams);
  // جلب قائمة الكيانات للربط
  const { propertiesData } = useAdminProperties({});

  // State for modals
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [selectedAmenity, setSelectedAmenity] = useState<AmenityDto | null>(null);
  const [selectedRows, setSelectedRows] = useState<string[]>([]);

  // State for forms
  const [createForm, setCreateForm] = useState<CreateAmenityCommand>({
    name: '',
    description: '',
  });

  const [editForm, setEditForm] = useState<UpdateAmenityCommand>({
    amenityId: '',
    name: '',
    description: '',
  });

  const [assignForm, setAssignForm] = useState({
    propertyId: '',
    extraCost: { amount: 0, currency: 'YER', formattedAmount: '' } as MoneyDto,
    isAvailable: true,
    description: '',
  });

  // يتم تنفيذ العمليات (إنشاء، تحديث، حذف، ربط) عبر الهوك

  // Helper functions
  const resetCreateForm = () => {
    setCreateForm({
      name: '',
      description: '',
    });
  };

  const handleViewDetails = (amenity: AmenityDto) => {
    setSelectedAmenity(amenity);
    setShowDetailsModal(true);
  };

  const handleEdit = (amenity: AmenityDto) => {
    setSelectedAmenity(amenity);
    setEditForm({
      amenityId: amenity.id,
      name: amenity.name,
      description: amenity.description,
    });
    setShowEditModal(true);
  };

  const handleDelete = (amenity: AmenityDto) => {
    if (confirm(`هل أنت متأكد من حذف المرفق "${amenity.name}"؟ هذا الإجراء لا يمكن التراجع عنه.`)) {
      deleteAmenity.mutate(amenity.id, {
        onSuccess: () => {
          setShowEditModal(false);
          setSelectedAmenity(null);
        },
      });
    }
  };

  const handleAssignToProperty = (amenity: AmenityDto) => {
    setSelectedAmenity(amenity);
    setShowAssignModal(true);
  };

  const handleFilterChange = (key: string, value: any) => {
    setFilterValues(prev => ({ ...prev, [key]: value }));
    setCurrentPage(1);
  };

  const handleResetFilters = () => {
    setFilterValues({
      category: '',
      isAssigned: undefined,
      propertyId: undefined,
      isFree: undefined
    });
    setSearchTerm('');
    setCurrentPage(1);
  };

  // Get amenity icon based on name/category
  const getAmenityIcon = (name: string) => {
    const iconMap: Record<string, string> = {
      'wifi': '📶',
      'مسبح': '🏊‍♂️',
      'جيم': '🏋️‍♂️',
      'موقف': '🚗',
      'مطبخ': '🍳',
      'مكيف': '❄️',
      'تلفزيون': '📺',
      'غسالة': '🧺',
      'شرفة': '🏡',
      'حديقة': '🌳',
    };
    
    const lowerName = name.toLowerCase();
    for (const [key, icon] of Object.entries(iconMap)) {
      if (lowerName.includes(key)) {
        return icon;
      }
    }
    return '🏠'; // Default icon
  };

  // Statistics calculation
  const stats = {
    total: amenitiesData?.items?.length || 0,
    totalCount: amenitiesData?.totalCount || 0,
  };

  // Filter options
  const filterOptions: FilterOption[] = [
    {
      key: 'propertyId',
      label: 'الكيان',
      type: 'custom',
      render: (value, onChange) => (
        <PropertySelector
          value={value}
          onChange={(id) => onChange(id)}
          placeholder="اختر الكيان"
          className="w-full"
        />
      ),
    },
    {
      key: 'isAssigned',
      label: 'مربوط بكيانات',
      type: 'boolean',
    },
    {
      key: 'isFree',
      label: 'مجاني',
      type: 'boolean',
    },
    {
      key: 'category',
      label: 'فئة المرفق',
      type: 'select',
      options: [
        { value: 'basic', label: 'أساسية' },
        { value: 'entertainment', label: 'ترفيهية' },
        { value: 'sports', label: 'رياضية' },
        { value: 'services', label: 'خدمات' },
        { value: 'technology', label: 'تقنية' },
      ],
    },
  ];

  // Table columns
  const columns: Column<AmenityDto>[] = [
    {
      key: 'name',
      title: 'المرفق',
      sortable: true,
      render: (value: string, record: AmenityDto) => (
        <div className="flex items-center">
          <span className="text-2xl ml-3">{getAmenityIcon(value)}</span>
          <div>
            <span className="font-medium text-gray-900">{value}</span>
            <p className="text-sm text-gray-500 mt-1">{record.description}</p>
          </div>
        </div>
      ),
    },
    {
      key: 'id',
      title: 'المعرف',
      render: (value: string) => (
        <span className="font-mono text-sm text-gray-600">
          {value.substring(0, 8)}...
        </span>
      ),
    },
  ];

  // Table actions
  const tableActions = [
    {
      label: 'عرض التفاصيل',
      icon: '👁️',
      color: 'blue' as const,
      onClick: handleViewDetails,
    },
    {
      label: 'تعديل',
      icon: '✏️',
      color: 'blue' as const,
      onClick: handleEdit,
    },
    {
      label: 'ربط بكيان',
      icon: '🔗',
      color: 'green' as const,
      onClick: handleAssignToProperty,
    },
    {
      label: 'حذف',
      icon: '🗑️',
      color: 'red' as const,
      onClick: handleDelete,
    },
  ];

  if (amenitiesError) {
    return (
      <div className="bg-white rounded-lg shadow-sm p-8 text-center">
        <div className="text-red-500 text-6xl mb-4">⚠️</div>
        <h2 className="text-xl font-bold text-gray-900 mb-2">خطأ في تحميل البيانات</h2>
        <p className="text-gray-600">حدث خطأ أثناء تحميل بيانات المرافق. يرجى المحاولة مرة أخرى.</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">إدارة المرافق</h1>
            <p className="text-gray-600 mt-1">
              إنشاء وتحديث المرافق المتاحة في النظام وربطها بالكيانات المختلفة
            </p>
          </div>
          <button
            onClick={() => setShowCreateModal(true)}
            className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            ➕ إضافة مرفق جديد
          </button>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-blue-100 p-2 rounded-lg">
              <span className="text-2xl">🏠</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">إجمالي المرافق</p>
              <p className="text-2xl font-bold text-gray-900">{stats.totalCount}</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-green-100 p-2 rounded-lg">
              <span className="text-2xl">🔗</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">مربوطة بكيانات</p>
              <p className="text-2xl font-bold text-green-600">-</p>
            </div>
          </div>
        </div>

        <div className="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div className="flex items-center">
            <div className="bg-orange-100 p-2 rounded-lg">
              <span className="text-2xl">📝</span>
            </div>
            <div className="mr-3">
              <p className="text-sm font-medium text-gray-600">غير مستخدمة</p>
              <p className="text-2xl font-bold text-orange-600">-</p>
            </div>
          </div>
        </div>
      </div>

      {/* Search and Filters */}
      <SearchAndFilter
        searchPlaceholder="البحث في المرافق (الاسم أو الوصف)..."
        searchValue={searchTerm}
        onSearchChange={setSearchTerm}
        filters={filterOptions}
        filterValues={filterValues}
        onFilterChange={handleFilterChange}
        onReset={handleResetFilters}
        showAdvanced={showAdvancedFilters}
        onToggleAdvanced={() => setShowAdvancedFilters(!showAdvancedFilters)}
      />

      {/* Amenities Table */}
      <DataTable
        data={amenitiesData?.items || []}
        columns={columns}
        loading={isLoadingAmenities}
        pagination={{
          current: currentPage,
          total: amenitiesData?.totalCount || 0,
          pageSize,
          onChange: (page, size) => {
            setCurrentPage(page);
            setPageSize(size);
          },
        }}
        rowSelection={{
          selectedRowKeys: selectedRows,
          onChange: setSelectedRows,
        }}
        actions={tableActions}
        onRowClick={handleViewDetails}
      />

      {/* Create Amenity Modal */}
      <Modal
        isOpen={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        title="إضافة مرفق جديد"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => setShowCreateModal(false)}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => createAmenity.mutate(createForm, {
                onSuccess: () => {
                  setShowCreateModal(false);
                  resetCreateForm();
                },
              })}
              disabled={createAmenity.status === 'pending' || !createForm.name.trim()}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {createAmenity.status === 'pending' ? 'جارٍ الإضافة...' : 'إضافة'}
            </button>
          </div>
        }
      >
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              اسم المرفق *
            </label>
            <input
              type="text"
              value={createForm.name}
              onChange={(e) => setCreateForm(prev => ({ ...prev, name: e.target.value }))}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              placeholder="أدخل اسم المرفق"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              وصف المرفق *
            </label>
            <textarea
              rows={3}
              value={createForm.description}
              onChange={(e) => setCreateForm(prev => ({ ...prev, description: e.target.value }))}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              placeholder="أدخل وصف تفصيلي للمرفق"
            />
          </div>

          {/* Icon Preview */}
          <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-md">
            <span className="text-3xl">{getAmenityIcon(createForm.name)}</span>
            <div>
              <p className="text-sm font-medium text-gray-700">معاينة الأيقونة</p>
              <p className="text-xs text-gray-500">سيتم اختيار الأيقونة تلقائياً حسب اسم المرفق</p>
            </div>
          </div>
        </div>
      </Modal>

      {/* Edit Amenity Modal */}
      <Modal
        isOpen={showEditModal}
        onClose={() => {
          setShowEditModal(false);
          setSelectedAmenity(null);
        }}
        title="تعديل بيانات المرفق"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowEditModal(false);
                setSelectedAmenity(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => updateAmenity.mutate({ amenityId: editForm.amenityId, data: editForm }, {
                onSuccess: () => {
                  setShowEditModal(false);
                  setSelectedAmenity(null);
                },
              })}
              disabled={updateAmenity.status === 'pending' || !editForm.name?.trim()}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              {updateAmenity.status === 'pending' ? 'جارٍ التحديث...' : 'تحديث'}
            </button>
          </div>
        }
      >
        {selectedAmenity && (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                اسم المرفق
              </label>
              <input
                type="text"
                value={editForm.name || ''}
                onChange={(e) => setEditForm(prev => ({ ...prev, name: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                وصف المرفق
              </label>
              <textarea
                rows={3}
                value={editForm.description || ''}
                onChange={(e) => setEditForm(prev => ({ ...prev, description: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              />
            </div>

            {/* Icon Preview */}
            <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-md">
              <span className="text-3xl">{getAmenityIcon(editForm.name || '')}</span>
              <div>
                <p className="text-sm font-medium text-gray-700">معاينة الأيقونة</p>
                <p className="text-xs text-gray-500">سيتم تحديث الأيقونة حسب الاسم الجديد</p>
              </div>
            </div>
          </div>
        )}
      </Modal>

      {/* Amenity Details Modal */}
      <Modal
        isOpen={showDetailsModal}
        onClose={() => {
          setShowDetailsModal(false);
          setSelectedAmenity(null);
        }}
        title="تفاصيل المرفق"
        size="lg"
      >
        {selectedAmenity && (
          <div className="space-y-6">
            <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-lg">
              <span className="text-6xl">{getAmenityIcon(selectedAmenity.name)}</span>
              <div>
                <h3 className="text-2xl font-bold text-gray-900">{selectedAmenity.name}</h3>
                <p className="text-gray-600 mt-1">{selectedAmenity.description}</p>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">معرف المرفق</label>
                <p className="mt-1 text-sm text-gray-900 font-mono">{selectedAmenity.id}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">اسم المرفق</label>
                <p className="mt-1 text-sm text-gray-900">{selectedAmenity.name}</p>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">الوصف</label>
              <p className="mt-1 text-sm text-gray-900">{selectedAmenity.description}</p>
            </div>

            <div className="bg-blue-50 border border-blue-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-blue-400 text-xl">ℹ️</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-blue-800">
                    معلومات إضافية
                  </h3>
                  <p className="mt-2 text-sm text-blue-700">
                    يمكن ربط هذا المرفق بالكيانات مع تحديد تكلفة إضافية وحالة التوفر لكل كيان.
                  </p>
                </div>
              </div>
            </div>
          </div>
        )}
      </Modal>
      {/* Assign to Property Modal */}
      <Modal
        isOpen={showAssignModal}
        onClose={() => {
          setShowAssignModal(false);
          setSelectedAmenity(null);
        }}
        title="ربط المرفق بكيان"
        size="lg"
        footer={
          <div className="flex justify-end gap-3">
            <button
              onClick={() => {
                setShowAssignModal(false);
                setSelectedAmenity(null);
              }}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
            >
              إلغاء
            </button>
            <button
              onClick={() => assignAmenityToProperty.mutate({ amenityId: selectedAmenity!.id, propertyId: assignForm.propertyId, data: { amenityId: selectedAmenity!.id, propertyId: assignForm.propertyId } }, {
                onSuccess: () => {
                  setShowAssignModal(false);
                  setSelectedAmenity(null);
                },
              })}
              disabled={assignAmenityToProperty.status === 'pending' || !assignForm.propertyId}
              className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 disabled:opacity-50"
            >
              {assignAmenityToProperty.status === 'pending' ? 'جارٍ الربط...' : 'ربط المرفق'}
            </button>
          </div>
        }
      >
        {selectedAmenity && (
          <div className="space-y-4">
            <div className="bg-green-50 border border-green-200 rounded-md p-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <span className="text-green-400 text-xl">🔗</span>
                </div>
                <div className="mr-3">
                  <h3 className="text-sm font-medium text-green-800">
                    ربط المرفق بكيان
                  </h3>
                  <p className="mt-2 text-sm text-green-700">
                    سيتم ربط المرفق "<strong>{selectedAmenity.name}</strong>" بالكيان المحدد مع إمكانية تحديد تكلفة إضافية.
                  </p>
                </div>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                معرف الكيان *
              </label>
              <select
                value={assignForm.propertyId}
                onChange={(e) => setAssignForm(prev => ({ ...prev, propertyId: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
              >
                <option value="">اختر كيان</option>
                {propertiesData?.items.map(p => (
                  <option key={p.id} value={p.id}>{p.name}</option>
                ))}
              </select>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  التكلفة الإضافية
                </label>
                <CurrencyInput
                  value={assignForm.extraCost.amount}
                  currency={assignForm.extraCost.currency}
                  onValueChange={(amount, currency) => setAssignForm(prev => ({
                    ...prev,
                    extraCost: { amount, currency }
                  }))}
                  placeholder="0"
                  required={false}
                  showSymbol={true}
                  supportedCurrencies={currencyCodes}
                  direction="ltr"
                />
              </div>
              {/* currency is handled by CurrencyInput component */}
            </div>

            <div className="flex items-center">
              <input
                type="checkbox"
                id="isAvailable"
                checked={assignForm.isAvailable}
                onChange={(e) => setAssignForm(prev => ({ ...prev, isAvailable: e.target.checked }))}
                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
              />
              <label htmlFor="isAvailable" className="mr-2 block text-sm text-gray-900">
                متاح في الكيان
              </label>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                وصف إضافي
              </label>
              <textarea
                rows={2}
                value={assignForm.description}
                onChange={(e) => setAssignForm(prev => ({ ...prev, description: e.target.value }))}
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                placeholder="أدخل وصف إضافي للمرفق في هذا الكيان (اختياري)"
              />
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default AdminAmenities;
