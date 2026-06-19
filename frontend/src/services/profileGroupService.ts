import { api } from './api';
import type { ApiResponse, ProfileGroup } from '../types';

export const profileGroupService = {
  getAll: async (activeOnly = true): Promise<ProfileGroup[]> => {
    const { data } = await api.get<ApiResponse<ProfileGroup[]>>(`/profilegroups?activeOnly=${activeOnly}`);
    if (!data.success || !data.data) return [];
    return data.data;
  },
};
