import { api } from './api';
import type { AdminUser, ApiResponse, PagedResult } from '../types';

const BASE = '/admin/users';

export const adminUserService = {
  getAll: async (params?: { search?: string; page?: number; pageSize?: number }) => {
    const { data } = await api.get<ApiResponse<PagedResult<AdminUser>>>(BASE, { params });
    return data.data!;
  },

  create: async (payload: {
    username: string;
    email: string;
    password: string;
    role: string;
    isActive: boolean;
  }) => {
    const { data } = await api.post<ApiResponse<string>>(BASE, payload);
    return data.data!;
  },

  update: async (id: string, payload: {
    username: string;
    email: string;
    password?: string;
    role: string;
    isActive: boolean;
  }) => {
    const { data } = await api.put<ApiResponse<null>>(`${BASE}/${id}`, payload);
    return data;
  },

  delete: async (id: string) => {
    const { data } = await api.delete<ApiResponse<null>>(`${BASE}/${id}`);
    return data;
  },

  toggleActive: async (id: string) => {
    const { data } = await api.patch<ApiResponse<boolean>>(`${BASE}/${id}/toggle-active`);
    return data.data!;
  },

  adjustTokens: async (id: string, amount: number, description: string) => {
    const { data } = await api.post<ApiResponse<null>>(`${BASE}/${id}/adjust-tokens`, { amount, description });
    return data;
  },
};
