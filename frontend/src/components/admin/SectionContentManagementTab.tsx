import React from 'react';
import HomeScreenManagement from '../../pages/admin/HomeScreenManagement';

interface ContentManagementTabProps {
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}

const ContentManagementTab: React.FC<ContentManagementTabProps> = ({ formData, onChange }) => {
  return (
    <HomeScreenManagement
      sectionType={formData.sectionType}
      currentContent={formData.content || []}
      onContentChange={(content) => onChange('content', content)}
      maxItems={formData.displaySettings?.maxItems || 10}
    />
  );
};

export default ContentManagementTab;