import React, { useEffect } from 'react';
import { TextareaAutosize as BaseTextareaAutosize } from '@mui/base';
import { styled } from '@mui/system';

interface TextAreaProps {
    maxRows?: number | string | undefined;
    minRows?: number | string | undefined;
    placeholder?: string | undefined;
    value: string;
    onChange: (event: React.ChangeEvent<HTMLTextAreaElement>) => void;
}

export default React.memo( function TextArea(props: TextAreaProps) {
    const { maxRows = "-", minRows = 3, placeholder = "", value, onChange } = props;

    const Textarea = styled(BaseTextareaAutosize)(
        ({ theme }) => `
        box-sizing: border-box;
        font-family: 'Open Sans', sans-serif;
        font-size: 16px;
        font-weight: 400;
        line-height: 1.5;
        height: auto!important;
        padding: 8px 12px;
        border-radius: 8px;
        color: ${theme.palette.text.primary};
        background: ${theme.palette.mode === 'dark' ? theme.palette.grey[900] : '#fff'};
        border: 1px solid ${theme.palette.grey[700]};
        box-shadow: 0px 2px 2px ${theme.palette.mode === 'dark' ? theme.palette.grey[900] : theme.palette.grey[50]};
    
        &:hover {
          border-color: ${theme.palette.grey[400]};
        }
    
        &:focus {
          border-color: ${theme.palette.grey[400]};
          box-shadow: 0 0 0 3px ${theme.palette.mode === 'dark' ? theme.palette.grey[600] : theme.palette.grey[200]};
        }
    
        // firefox
        &:focus-visible {
          outline: 0;
        }
      `,
    );

    return (
        <Textarea aria-label="minimum height" maxRows={maxRows} minRows={minRows} placeholder={placeholder} defaultValue={value} onChange={onChange} />
    );
});
