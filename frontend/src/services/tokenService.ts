import { api } from './api';
import type { ApiResponse, TokenBalance, TokenPackage, TokenTransaction, PagedResult } from '../types';

export const tokenService = {
  getBalance: async (): Promise<TokenBalance> => {
    const { data } = await api.get<ApiResponse<TokenBalance>>('/tokens/balance');
    if (!data.success || !data.data) throw new Error(data.message || 'Failed to get balance');
    return data.data;
  },

  getPackages: async (): Promise<TokenPackage[]> => {
    const { data } = await api.get<ApiResponse<TokenPackage[]>>('/tokens/packages');
    return data.data || [];
  },

  getHistory: async (page = 1, pageSize = 20): Promise<PagedResult<TokenTransaction>> => {
    const { data } = await api.get<ApiResponse<PagedResult<TokenTransaction>>>(`/tokens/history?page=${page}&pageSize=${pageSize}`);
    return data.data || { items: [], totalCount: 0, pageNumber: page, pageSize, totalPages: 0, hasPreviousPage: false, hasNextPage: false };
  },
};
