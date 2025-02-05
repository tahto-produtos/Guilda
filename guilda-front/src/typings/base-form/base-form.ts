export interface BaseForm<FormSchema, Error = unknown> {
  id: string;
  onSubmit: (data: FormSchema) => void;
  onFail?: (error: Error) => void;
  initialValues?: Partial<FormSchema>;
}
