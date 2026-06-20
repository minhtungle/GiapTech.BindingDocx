import { api } from './api';
import type {
  WorkspaceGroup, GroupKeys, ImportDataResult,
  GenerateFilesRequest, PreviewRenderedRequest,
} from '../types/workspace';
import type { ApiResponse } from '../types';

const BASE = '/workspace';

export const workspaceService = {
  getGroups: async (): Promise<WorkspaceGroup[]> => {
    const { data } = await api.get<ApiResponse<WorkspaceGroup[]>>(`${BASE}/groups`);
    return data.data ?? [];
  },

  getGroupKeys: async (groupId: string): Promise<GroupKeys> => {
    const { data } = await api.get<ApiResponse<GroupKeys>>(`${BASE}/groups/${groupId}/keys`);
    return data.data ?? { singleFields: [], tableFiles: [], files: [] };
  },

  getFilePreview: async (groupId: string, fileName: string): Promise<Blob> => {
    const { data } = await api.get(
      `${BASE}/groups/${groupId}/files/${encodeURIComponent(fileName)}`,
      { responseType: 'blob' }
    );
    return data as Blob;
  },

  previewRendered: async (
    groupId: string,
    fileName: string,
    request: PreviewRenderedRequest
  ): Promise<Blob> => {
    const { data } = await api.post(
      `${BASE}/groups/${groupId}/files/${encodeURIComponent(fileName)}/preview-rendered`,
      request,
      { responseType: 'blob' }
    );
    return data as Blob;
  },

  exportTemplate: async (groupId: string): Promise<void> => {
    const { data } = await api.get(`${BASE}/groups/${groupId}/export-template`, {
      responseType: 'blob',
    });
    const url = URL.createObjectURL(new Blob([data]));
    const a = document.createElement('a');
    a.href = url;
    a.download = `template_${groupId}.xlsx`;
    a.click();
    URL.revokeObjectURL(url);
  },

  importData: async (groupId: string, file: File): Promise<ImportDataResult> => {
    const form = new FormData();
    form.append('file', file);
    const { data } = await api.post<ApiResponse<ImportDataResult>>(
      `${BASE}/groups/${groupId}/import-data`,
      form,
      { headers: { 'Content-Type': 'multipart/form-data' } }
    );
    return data.data ?? { singleFields: {}, tableData: {}, totalRows: 0 };
  },

  generateFiles: async (groupId: string, request: GenerateFilesRequest): Promise<void> => {
    const { data } = await api.post(`${BASE}/groups/${groupId}/generate`, request, {
      responseType: 'blob',
    });
    const url = URL.createObjectURL(new Blob([data], { type: 'application/zip' }));
    const a = document.createElement('a');
    a.href = url;
    a.download = `${groupId}_output.zip`;
    a.click();
    URL.revokeObjectURL(url);
  },
};
