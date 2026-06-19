import { api } from './api';
import type { ApiResponse, AuthResponse } from '../types';

export const authService = {
  login: async (username: string, password: string): Promise<AuthResponse> => {
    const { data } = await api.post<ApiResponse<AuthResponse>>('/auth/login', { username, password });
    if (!data.success || !data.data) throw new Error(data.message || 'Login failed');
    return data.data;
  },

  logout: async (): Promise<void> => {
    await api.post('/auth/logout');
  },
};
