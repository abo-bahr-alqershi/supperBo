import React, { useState } from 'react';
// أيقونات بسيطة بدلاً من lucide-react
const PlusIcon = () => <span>➕</span>;
const EditIcon = () => <span>✏️</span>;
const TrashIcon = () => <span>🗑️</span>;
const SearchIcon = () => <span>🔍</span>;
const BuildingIcon = () => <span>🏢</span>;
const DollarIcon = () => <span>💰</span>;
const TagIcon = () => <span>🏷️</span>;
const EyeIcon = () => <span>👁️</span>;
import {
  usePropertyServices,
  useServiceDetails,
  useServicesByType,
  useCreatePropertyService,
  useUpdatePropertyService,
  useDeletePropertyService,
} from '../../hooks/useAdminPropertyServices';
import { useAdminProperties } from '../../hooks/useAdminProperties';
import type {
  ServiceDto,
  ServiceDetailsDto,
  CreatePropertyServiceCommand,
  UpdatePropertyServiceCommand,
  PricingModel,
} from '../../types/service.types';
import type { MoneyDto } from '../../types/amenity.types';
import DataTable from '../../components/common/DataTable';
import Modal from '../../components/common/Modal';
import PropertySelector from '../../components/selectors/PropertySelector';
import CurrencyInput from '../../components/inputs/CurrencyInput';
import ActionsDropdown from '../../components/ui/ActionsDropdown';
import { useCurrencies } from '../../hooks/useCurrencies';

const AdminPropertyServices = () => {
  const [selectedPropertyId, setSelectedPropertyId] = useState<string>('');
  const [selectedServiceType, setSelectedServiceType] = useState<string>('');
  const [searchQuery, setSearchQuery] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  // Fetch currencies for price component
  const { currencies, loading: currenciesLoading } = useCurrencies();
  const currencyCodes = currenciesLoading ? [] : currencies.map(c => c.code);
  
  // حالات المودالات
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDetailsModalOpen, setIsDetailsModalOpen] = useState(false);
  const [selectedService, setSelectedService] = useState<ServiceDto | null>(null);

  // بيانات النماذج
  const [createForm, setCreateForm] = useState<CreatePropertyServiceCommand>({
    propertyId: '',
    name: '',
    price: { amount: 0, currency: 'SAR' },
    pricingModel: 'PerBooking',
  });

  const [editForm, setEditForm] = useState<UpdatePropertyServiceCommand>({
    serviceId: '',
    name: '',
    price: { amount: 0, currency: 'SAR' },
    pricingModel: 'PerBooking',
  });

  // جلب البيانات
  // Fetch properties with maximum allowed page size (API supports up to 100)
  const { propertiesData: properties } = useAdminProperties({
    pageNumber: 1,
    pageSize: 100,
  });

  const { data: propertyServices, isLoading: isLoadingPropertyServices } = usePropertyServices({
    propertyId: selectedPropertyId,
  });

  const { data: servicesByType, isLoading: isLoadingServicesByType } = useServicesByType({
    serviceType: selectedServiceType,
    pageNumber,
    pageSize,
  });

  const { data: serviceDetails } = useServiceDetails({
    serviceId: selectedService?.id || '',
  });

  // الطفرات
  const createMutation = useCreatePropertyService();
  const updateMutation = useUpdatePropertyService();
  const deleteMutation = useDeletePropertyService();

  // خيارات نماذج التسعير
  const pricingModelOptions = [
    { value: 'PerBooking', label: 'لكل حجز' },
    { value: 'PerDay', label: 'لكل يوم' },
    { value: 'PerPerson', label: 'لكل شخص' },
    { value: 'PerUnit', label: 'لكل وحدة' },
  ];

  // دالة لتحديد البيانات المعروضة
  const getDisplayData = () => {
    if (selectedPropertyId && propertyServices?.success && propertyServices.data) {
      return propertyServices.data;
    }
    if (selectedServiceType && servicesByType?.items) {
      return servicesByType.items;
    }
    return [];
  };

  const isLoading = selectedPropertyId ? isLoadingPropertyServices : isLoadingServicesByType;

  // التعامل مع الأحداث
  const handleCreate = () => {
    setCreateForm({
      propertyId: selectedPropertyId || '',
      name: '',
      price: { amount: 0, currency: 'SAR' },
      pricingModel: 'PerBooking',
    });
    setIsCreateModalOpen(true);
  };

  const handleEdit = (service: ServiceDto) => {
    setSelectedService(service);
    setEditForm({
      serviceId: service.id,
      name: service.name,
      price: service.price,
      pricingModel: Object.keys(service.pricingModel)[0] as string,
    });
    setIsEditModalOpen(true);
  };

  const handleDelete = (service: ServiceDto) => {
    if (confirm('هل أنت متأكد من حذف هذه الخدمة؟')) {
      deleteMutation.mutate(service.id, {
        onSuccess: () => {
          // إشعار نجاح
        },
        onError: () => {
          // إشعار خطأ
        }
      });
    }
  };

  const handleViewDetails = (service: ServiceDto) => {
    setSelectedService(service);
    setIsDetailsModalOpen(true);
  };

  const handleCreateSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    createMutation.mutate(createForm, {
      onSuccess: () => {
        setIsCreateModalOpen(false);
      },
      onError: () => {
        // إشعار خطأ
      }
    });
  };

  const handleEditSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    updateMutation.mutate({
      serviceId: editForm.serviceId,
      data: editForm,
    }, {
      onSuccess: () => {
        setIsEditModalOpen(false);
      },
      onError: () => {
        // إشعار خطأ
      }
    });
  };

  // أعمدة الجدول
  const columns = [
    {
      header: 'اسم الخدمة',
      title: 'اسم الخدمة',
      key: 'name',
      render: (service: ServiceDto) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
            <TagIcon />
          </div>
          <div>
            <div className="font-medium text-gray-900">{service.name}</div>
            <div className="text-sm text-gray-500">ID: {service.id}</div>
          </div>
        </div>
      ),
    },
    {
      header: 'العقار',
      title: 'العقار',
      key: 'property',
      render: (service: ServiceDto) => (
        <div className="flex items-center gap-2">
          <BuildingIcon />
          <div>
            <div className="font-medium">{service.propertyName}</div>
            <div className="text-sm text-gray-500">{service.propertyId}</div>
          </div>
        </div>
      ),
    },
    {
      header: 'السعر',
      title: 'السعر',
      key: 'price',
      render: (service: ServiceDto) => (
        <div className="flex items-center gap-2">
          <DollarIcon />
          <div>
            <div className="font-medium text-green-600">
              {service.price.amount} {service.price.currency}
            </div>
            <div className="text-sm text-gray-500">
              {pricingModelOptions.find(option => 
                option.value === Object.keys(service.pricingModel)[0]
              )?.label}
            </div>
          </div>
        </div>
      ),
    },
    {
      header: 'الإجراءات',
      title: 'الإجراءات',
      key: 'actions',
      render: (service: ServiceDto) => (
        <ActionsDropdown
          actions={[
            {
              label: 'عرض التفاصيل',
              icon: '👁️',
              onClick: () => handleViewDetails(service),
            },
            {
              label: 'تعديل',
              icon: '✏️',
              onClick: () => handleEdit(service),
            },
            {
              label: 'حذف',
              icon: '🗑️',
              onClick: () => handleDelete(service),
              variant: 'danger',
            },
          ]}
        />
      ),
    },
  ];

  const filteredData = getDisplayData().filter(service =>
    service.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    service.propertyName.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">إدارة الخدمات</h1>
          <p className="text-gray-600">إدارة خدمات العقارات والتحكم في الأسعار</p>
        </div>
        <button
          onClick={handleCreate}
          disabled={!selectedPropertyId}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <PlusIcon />
          إضافة خدمة جديدة
        </button>
      </div>

      {/* الفلاتر */}
      <div className="bg-white rounded-lg shadow p-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              العقار
            </label>
            <PropertySelector
              value={selectedPropertyId}
              onChange={(id) => { setSelectedPropertyId(id); setSelectedServiceType(''); }}
              placeholder="اختر العقار"
              className="w-full"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              نوع الخدمة
            </label>
            <input
              type="text"
              value={selectedServiceType}
              onChange={(e) => {
                setSelectedServiceType(e.target.value);
                setSelectedPropertyId('');
              }}
              placeholder="ادخل نوع الخدمة"
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              البحث
            </label>
            <div className="relative">
              <div className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400">
                <SearchIcon />
              </div>
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="البحث في الخدمات..."
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>
        </div>

        {!selectedPropertyId && !selectedServiceType && (
          <div className="text-center py-8 text-gray-500">
            الرجاء اختيار عقار أو إدخال نوع خدمة لعرض النتائج
          </div>
        )}
      </div>

      {/* الجدول */}
      {(selectedPropertyId || selectedServiceType) && (
        <div className="bg-white rounded-lg shadow">
          <DataTable
            data={filteredData}
            columns={columns}
            loading={isLoading}
            pagination={selectedServiceType ? {
              current: pageNumber,
              total: servicesByType?.totalPages || 1,
              pageSize,
              onChange: (page, size) => {
                setPageNumber(page);
                setPageSize(size);
              },
            } : undefined}
            onRowClick={() => {}}
          />
        </div>
      )}

      {/* مودال إنشاء خدمة */}
      <Modal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        title="إضافة خدمة جديدة"
      >
        <form onSubmit={handleCreateSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              العقار
            </label>
            <PropertySelector
              value={createForm.propertyId}
              onChange={(id) => setCreateForm(prev => ({ ...prev, propertyId: id }))}
              required
              className="w-full"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              اسم الخدمة
            </label>
            <input
              type="text"
              value={createForm.name}
              onChange={(e) => setCreateForm(prev => ({ ...prev, name: e.target.value }))}
              required
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                السعر
              </label>
              <CurrencyInput
                value={createForm.price.amount}
                currency={createForm.price.currency}
                supportedCurrencies={currencyCodes}
                onValueChange={(amount, currency) => setCreateForm(prev => ({
                  ...prev,
                  price: { amount, currency }
                }))}
                required
                className="w-full"
                min={0}
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                نموذج التسعير
              </label>
              <select
                value={createForm.pricingModel}
                onChange={(e) => setCreateForm(prev => ({ ...prev, pricingModel: e.target.value }))}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                {pricingModelOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="flex gap-2 pt-4">
            <button
              type="submit"
              disabled={createMutation.isPending}
              className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors"
            >
              {createMutation.isPending ? 'جاري الإنشاء...' : 'إنشاء'}
            </button>
            <button
              type="button"
              onClick={() => setIsCreateModalOpen(false)}
              className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
            >
              إلغاء
            </button>
          </div>
        </form>
      </Modal>

      {/* مودال تعديل خدمة */}
      <Modal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        title="تعديل الخدمة"
      >
        <form onSubmit={handleEditSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              اسم الخدمة
            </label>
            <input
              type="text"
              value={editForm.name}
              onChange={(e) => setEditForm(prev => ({ ...prev, name: e.target.value }))}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                السعر
              </label>
              <CurrencyInput
                value={editForm.price?.amount ?? 0}
                currency={editForm.price?.currency ?? 'SAR'}
                supportedCurrencies={currencyCodes}
                onValueChange={(amount, currency) => setEditForm(prev => ({
                  ...prev,
                  price: { amount, currency }
                }))}
                required
                className="w-full"
                min={0}
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                نموذج التسعير
              </label>
              <select
                value={editForm.pricingModel}
                onChange={(e) => setEditForm(prev => ({ ...prev, pricingModel: e.target.value }))}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                {pricingModelOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="flex gap-2 pt-4">
            <button
              type="submit"
              disabled={updateMutation.isPending}
              className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors"
            >
              {updateMutation.isPending ? 'جاري التحديث...' : 'تحديث'}
            </button>
            <button
              type="button"
              onClick={() => setIsEditModalOpen(false)}
              className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
            >
              إلغاء
            </button>
          </div>
        </form>
      </Modal>

      {/* مودال تفاصيل الخدمة */}
      <Modal
        isOpen={isDetailsModalOpen}
        onClose={() => setIsDetailsModalOpen(false)}
        title="تفاصيل الخدمة"
        size="lg"
      >
        {selectedService && serviceDetails?.success && (
          <div className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 p-4 bg-gray-50 rounded-lg">
              <div>
                <div className="text-sm text-gray-500">اسم الخدمة</div>
                <div className="font-medium">{serviceDetails.data.name}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">معرف الخدمة</div>
                <div className="font-medium">{serviceDetails.data.id}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">العقار</div>
                <div className="font-medium">{serviceDetails.data.propertyName}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">معرف العقار</div>
                <div className="font-medium">{serviceDetails.data.propertyId}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500">السعر</div>
                <div className="font-medium text-green-600">
                  {serviceDetails.data.price.amount} {serviceDetails.data.price.currency}
                </div>
              </div>
              <div>
                <div className="text-sm text-gray-500">نموذج التسعير</div>
                <div className="font-medium">
                  {pricingModelOptions.find(option => 
                    option.value === Object.keys(serviceDetails.data.pricingModel)[0]
                  )?.label}
                </div>
              </div>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default AdminPropertyServices;