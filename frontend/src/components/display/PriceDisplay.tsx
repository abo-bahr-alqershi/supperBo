import React from 'react';

interface MoneyDto {
  amount: number;
  currency: string;
}

interface PriceDisplayProps {
  price: MoneyDto;
  className?: string;
  variant?: 'default' | 'card' | 'featured' | 'minimal' | 'large';
  showCurrency?: boolean;
  showFlag?: boolean;
  direction?: 'rtl' | 'ltr';
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl';
}

const PriceDisplay: React.FC<PriceDisplayProps> = ({
  price,
  className = '',
  variant = 'default',
  showCurrency = true,
  showFlag = true,
  direction = 'rtl',
  size = 'md'
}) => {
  // معلومات العملات
  const currencyInfo = {
    YER: { symbol: 'ر.ي', name: 'ريال يمني', flag: '🇾🇪', decimals: 2 },
    SAR: { symbol: 'ر.س', name: 'ريال سعودي', flag: '🇸🇦', decimals: 2 },
    USD: { symbol: '$', name: 'دولار أمريكي', flag: '🇺🇸', decimals: 2 },
    EUR: { symbol: '€', name: 'يورو', flag: '🇪🇺', decimals: 2 },
    AED: { symbol: 'د.إ', name: 'درهم إماراتي', flag: '🇦🇪', decimals: 2 },
    QAR: { symbol: 'ر.ق', name: 'ريال قطري', flag: '🇶🇦', decimals: 2 },
    KWD: { symbol: 'د.ك', name: 'دينار كويتي', flag: '🇰🇼', decimals: 3 },
    BHD: { symbol: 'د.ب', name: 'دينار بحريني', flag: '🇧🇭', decimals: 3 },
    OMR: { symbol: 'ر.ع', name: 'ريال عماني', flag: '🇴🇲', decimals: 3 }
  };

  const getCurrencyInfo = () => currencyInfo[price.currency] || currencyInfo.SAR;

  // تنسيق المبلغ
  const formatAmount = () => {
    const info = getCurrencyInfo();
    return new Intl.NumberFormat('ar-SA', {
      minimumFractionDigits: info.decimals,
      maximumFractionDigits: info.decimals
    }).format(price.amount);
  };

  // أحجام النص
  const sizeStyles = {
    xs: {
      amount: 'text-xs',
      symbol: 'text-xs',
      flag: 'text-xs',
      currency: 'text-xs'
    },
    sm: {
      amount: 'text-sm',
      symbol: 'text-xs',
      flag: 'text-sm',
      currency: 'text-xs'
    },
    md: {
      amount: 'text-lg font-semibold',
      symbol: 'text-sm',
      flag: 'text-base',
      currency: 'text-xs'
    },
    lg: {
      amount: 'text-xl font-bold',
      symbol: 'text-base',
      flag: 'text-lg',
      currency: 'text-sm'
    },
    xl: {
      amount: 'text-2xl font-bold',
      symbol: 'text-lg',
      flag: 'text-xl',
      currency: 'text-base'
    }
  };

  // أنماط التصميم
  const variantStyles = {
    default: 'text-gray-900',
    card: 'text-green-600 bg-green-50 px-3 py-1 rounded-full border border-green-200',
    featured: 'text-blue-700 bg-gradient-to-r from-blue-50 to-indigo-50 px-4 py-2 rounded-xl border border-blue-200 shadow-sm',
    minimal: 'text-gray-700',
    large: 'text-green-700 bg-gradient-to-r from-green-50 to-emerald-50 px-6 py-3 rounded-2xl border border-green-200 shadow-lg'
  };

  const currentSize = sizeStyles[size];
  const currentVariant = variantStyles[variant];
  const info = getCurrencyInfo();

  if (variant === 'featured' || variant === 'large') {
    return (
      <div className={`inline-flex items-center gap-2 ${currentVariant} ${className}`}>
        {showFlag && (
          <span className={currentSize.flag}>{info.flag}</span>
        )}
        <div className="flex items-baseline gap-1">
          <span className={currentSize.amount} dir={direction}>
            {formatAmount()}
          </span>
          <span className={`${currentSize.symbol} opacity-75`}>
            {info.symbol}
          </span>
        </div>
        {showCurrency && (
          <span className={`${currentSize.currency} opacity-60 font-medium`}>
            {price.currency}
          </span>
        )}
      </div>
    );
  }

  if (variant === 'card') {
    return (
      <div className={`inline-flex items-center gap-1.5 ${currentVariant} ${className}`}>
        {showFlag && (
          <span className={currentSize.flag}>{info.flag}</span>
        )}
        <span className={currentSize.amount} dir={direction}>
          {formatAmount()}
        </span>
        <span className={`${currentSize.symbol} opacity-75`}>
          {info.symbol}
        </span>
      </div>
    );
  }

  // Default و Minimal
  return (
    <span className={`inline-flex items-center gap-1 ${currentVariant} ${className}`} dir={direction}>
      {showFlag && (
        <span className={currentSize.flag}>{info.flag}</span>
      )}
      <span className={currentSize.amount}>
        {formatAmount()}
      </span>
      <span className={`${currentSize.symbol} opacity-75`}>
        {info.symbol}
      </span>
      {showCurrency && variant === 'default' && (
        <span className={`${currentSize.currency} opacity-60 mr-1`}>
          ({price.currency})
        </span>
      )}
    </span>
  );
};

export default PriceDisplay;