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
  tableKey: string;
  columns: string[];
}

export interface TemplateFileInfo {
  fileName: string;
  displayName: string;
  fileType: 'docx' | 'xlsx';
  keyCount: number;
  keys: string[];
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
  // per-file single fields: fileName → { key → value }
  singleFieldsByFile: Record<string, Record<string, string>>;
  tableData: Record<string, Record<string, string>[]>;
}

export interface GenerateFilesRequest {
  syncMode: boolean;
  singleFields: Record<string, string>;
  singleFieldsByFile: Record<string, Record<string, string>>;
  tableData: Record<string, Record<string, string>[]>;
}

export interface PreviewRenderedRequest {
  singleFields: Record<string, string>;
  tableData: Record<string, Record<string, string>[]>;
}
