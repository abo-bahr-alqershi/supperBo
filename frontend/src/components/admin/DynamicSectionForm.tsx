import React, { useState } from 'react';
import Form from '@rjsf/core';
import DynamicSection from '../home/DynamicSection';
import sectionSchema from '../../schemas/DynamicHomeSection.schema.json';

const uiSchema = {};

const DynamicSectionForm: React.FC = () => {
  const [formData, setFormData] = useState<Record<string, any>>({});
  const handleChange = ({ formData }: any) => setFormData(formData);

  return (
    <div style={{ display: 'flex', gap: '16px' }}>
      <div style={{ flex: 1 }}>
        <Form
          schema={sectionSchema}
          uiSchema={uiSchema}
          formData={formData}
          onChange={handleChange}
        >
          <></>
        </Form>
      </div>

      <div style={{ flex: 1, border: '1px solid #ddd', padding: '16px', borderRadius: '4px' }}>
        <h3>Live Preview</h3>
        <DynamicSection section={formData} config={formData.config || {}} />
      </div>
    </div>
  );
};

export default DynamicSectionForm;