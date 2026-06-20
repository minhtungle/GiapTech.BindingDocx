import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { ProfileGroup } from '../types';
import type { GroupKeys, FormData } from '../types/workspace';

interface AppState {
  sidebarCollapsed: boolean;
  selectedGroupId: string | null;
  selectedGroup: ProfileGroup | null;
  activeTab: 'keys' | 'preview';
  tokenDrawerOpen: boolean;
  settingsDrawerOpen: boolean;
  groupKeys: GroupKeys | null;
  keysLoading: boolean;
  formData: FormData;
  totalRows: number;
  setSidebarCollapsed: (collapsed: boolean) => void;
  setSelectedGroup: (group: ProfileGroup | null) => void;
  setActiveTab: (tab: 'keys' | 'preview') => void;
  setTokenDrawerOpen: (open: boolean) => void;
  setSettingsDrawerOpen: (open: boolean) => void;
  setGroupKeys: (keys: GroupKeys | null) => void;
  setKeysLoading: (loading: boolean) => void;
  setSingleField: (key: string, value: string) => void;
  setAllSingleFields: (fields: Record<string, string>) => void;
  setTableData: (fileName: string, rows: Record<string, string>[]) => void;
  setTotalRows: (rows: number) => void;
  resetFormData: () => void;
}

const emptyFormData: FormData = { singleFields: {}, tableData: {} };

export const useAppStore = create<AppState>()(
  persist(
    (set) => ({
      sidebarCollapsed: false,
      selectedGroupId: null,
      selectedGroup: null,
      activeTab: 'keys',
      tokenDrawerOpen: false,
      settingsDrawerOpen: false,
      groupKeys: null,
      keysLoading: false,
      formData: emptyFormData,
      totalRows: 0,

      setSidebarCollapsed: (collapsed) => set({ sidebarCollapsed: collapsed }),
      setSelectedGroup: (group) => set({
        selectedGroup: group,
        selectedGroupId: group?.id ?? null,
        groupKeys: null,
        keysLoading: false,
        formData: emptyFormData,
        totalRows: 0,
        activeTab: 'keys',
      }),
      setActiveTab: (tab) => set({ activeTab: tab }),
      setTokenDrawerOpen: (open) => set({ tokenDrawerOpen: open }),
      setSettingsDrawerOpen: (open) => set({ settingsDrawerOpen: open }),
      setGroupKeys: (keys) => set({ groupKeys: keys }),
      setKeysLoading: (loading) => set({ keysLoading: loading }),
      setSingleField: (key, value) =>
        set((state) => ({
          formData: {
            ...state.formData,
            singleFields: { ...state.formData.singleFields, [key]: value },
          },
        })),
      setAllSingleFields: (fields) =>
        set((state) => ({
          formData: {
            ...state.formData,
            singleFields: { ...state.formData.singleFields, ...fields },
          },
        })),
      setTableData: (fileName, rows) =>
        set((state) => ({
          formData: {
            ...state.formData,
            tableData: { ...state.formData.tableData, [fileName]: rows },
          },
        })),
      setTotalRows: (rows) => set({ totalRows: rows }),
      resetFormData: () => set({ formData: emptyFormData, totalRows: 0 }),
    }),
    {
      name: 'app-storage',
      partialize: (state) => ({
        sidebarCollapsed: state.sidebarCollapsed,
        selectedGroupId: state.selectedGroupId,
      }),
    }
  )
);
