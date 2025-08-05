import React from 'react';
import Form from '@rjsf/core';
import type { IChangeEvent } from '@rjsf/core';
import { JSONSchema7 } from 'json-schema';
import validator from '@rjsf/validator-ajv8';
import DynamicSection from '../home/DynamicSection';
import sectionSchema from '../../schemas/DynamicHomeSection.schema.json';

const uiSchema = {};

interface DynamicSectionFormProps {
  formData: any;
  onChange: (formData: any) => void;
}

const DynamicSectionForm: React.FC<DynamicSectionFormProps> = ({ formData, onChange }) => {
  const handleChange = (data: IChangeEvent<any, JSONSchema7, any>, id?: string) => onChange(data.formData);

  return (
    <div style={{ display: 'flex', gap: '16px' }}>
      <div style={{ flex: 1 }}>
        <Form<any, JSONSchema7, any>
          validator={validator}
          schema={sectionSchema as JSONSchema7}
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