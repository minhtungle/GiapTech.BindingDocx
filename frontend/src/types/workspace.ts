export interface WorkspaceGroup {
  id: string;
  name: string;
  isAvailable: boolean;
  fileCount: number;
}

export interface TableFileInfo {
  fileName: string;
  displayName: string;
  sheetName: string;
  columns: string[];
}

export interface TemplateFileInfo {
  fileName: string;
  displayName: string;
  fileType: 'docx' | 'xlsx';
  keyCount: number;
}

export interface GroupKeys {
  singleFields: string[];
  tableFiles: TableFileInfo[];
  files: TemplateFileInfo[];
}

export interface ImportDataResult {
  singleFields: Record<string, string>;
  tableData: Record<string, Record<string, string>[]>;
  totalRows: number;
}

export interface FormData {
  singleFields: Record<string, string>;
  tableData: Record<string, Record<string, string>[]>;
}

export interface GenerateFilesRequest {
  singleFields: Record<string, string>;
  tableData: Record<string, Record<string, string>[]>;
}
