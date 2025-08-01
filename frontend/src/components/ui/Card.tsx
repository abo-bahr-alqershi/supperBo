import React from 'react';

interface CardProps {
  children: React.ReactNode;
  className?: string;
  variant?: 'default' | 'outlined' | 'elevated';
  padding?: 'none' | 'sm' | 'md' | 'lg';
}

export const Card: React.FC<CardProps> = ({
  children,
  className = '',
  variant = 'default',
  padding = 'md'
}) => {
  const variantClasses = {
    default: 'glass-card border border-white/30 hover-lift',
    outlined: 'bg-white/50 border-2 border-gray-300/50 backdrop-blur-sm hover-lift',
    elevated: 'bg-white shadow-2xl border border-gray-100/50 hover-lift hover-glow'
  };

  const paddingClasses = {
    none: '',
    sm: 'p-3',
    md: 'p-4',
    lg: 'p-6'
  };

  return (
    <div
      className={`
        rounded-2xl transition-all duration-300
        ${variantClasses[variant]}
        ${paddingClasses[padding]}
        ${className}
      `}
    >
      {children}
    </div>
  );
};

export default Card;