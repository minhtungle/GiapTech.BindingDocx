import { api } from './api';
import type { AdminTokenTransaction, ApiResponse, PagedResult, TokenPackage } from '../types';

export const adminTokenService = {
  getAllTransactions: async (params?: { userId?: string; page?: number; pageSize?: number }) => {
    const { data } = await api.get<ApiResponse<PagedResult<AdminTokenTransaction>>>('/admin/transactions', { params });
    return data.data!;
  },

  getAllPackages: async () => {
    const { data } = await api.get<ApiResponse<TokenPackage[]>>('/admin/token-packages');
    return data.data!;
  },

  createPackage: async (payload: {
    name: string;
    tokenAmount: number;
    pricePerToken: number;
    totalPrice: number;
    isActive: boolean;
    sortOrder: number;
  }) => {
    const { data } = await api.post<ApiResponse<string>>('/admin/token-packages', payload);
    return data.data!;
  },

  updatePackage: async (id: string, payload: {
    name: string;
    tokenAmount: number;
    pricePerToken: number;
    totalPrice: number;
    isActive: boolean;
    sortOrder: number;
  }) => {
    const { data } = await api.put<ApiResponse<null>>(`/admin/token-packages/${id}`, payload);
    return data;
  },

  deletePackage: async (id: string) => {
    const { data } = await api.delete<ApiResponse<null>>(`/admin/token-packages/${id}`);
    return data;
  },

  togglePackageActive: async (id: string) => {
    const { data } = await api.patch<ApiResponse<null>>(`/admin/token-packages/${id}/toggle-active`);
    return data;
  },
};
