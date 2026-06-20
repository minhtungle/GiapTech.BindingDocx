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
  syncEnabled: boolean;
  formData: FormData;
  totalRows: number;
  setSidebarCollapsed: (collapsed: boolean) => void;
  setSelectedGroup: (group: ProfileGroup | null) => void;
  setActiveTab: (tab: 'keys' | 'preview') => void;
  setTokenDrawerOpen: (open: boolean) => void;
  setSettingsDrawerOpen: (open: boolean) => void;
  setGroupKeys: (keys: GroupKeys | null) => void;
  setKeysLoading: (loading: boolean) => void;
  setSyncEnabled: (enabled: boolean) => void;
  setSingleField: (fileName: string, key: string, value: string) => void;
  setAllSingleFields: (fields: Record<string, string>) => void;
  setTableData: (tableKey: string, rows: Record<string, string>[]) => void;
  setTotalRows: (rows: number) => void;
  resetFormData: () => void;
  // Derive flat merged singleFields for sync mode
  getMergedSingleFields: () => Record<string, string>;
}

const emptyFormData: FormData = { singleFieldsByFile: {}, tableData: {} };

export const useAppStore = create<AppState>()(
  persist(
    (set, get) => ({
      sidebarCollapsed: false,
      selectedGroupId: null,
      selectedGroup: null,
      activeTab: 'keys',
      tokenDrawerOpen: false,
      settingsDrawerOpen: false,
      groupKeys: null,
      keysLoading: false,
      syncEnabled: true,
      formData: emptyFormData,
      totalRows: 0,

      setSidebarCollapsed: (collapsed) => set({ sidebarCollapsed: collapsed }),
      setSelectedGroup: (group) => set({
        selectedGroup: group,
        selectedGroupId: group?.id ?? null,
        groupKeys: null,
        keysLoading: false,
        syncEnabled: true,
        formData: emptyFormData,
        totalRows: 0,
        activeTab: 'keys',
      }),
      setActiveTab: (tab) => set({ activeTab: tab }),
      setTokenDrawerOpen: (open) => set({ tokenDrawerOpen: open }),
      setSettingsDrawerOpen: (open) => set({ settingsDrawerOpen: open }),
      setGroupKeys: (keys) => set({ groupKeys: keys }),
      setKeysLoading: (loading) => set({ keysLoading: loading }),
      setSyncEnabled: (enabled) => set({ syncEnabled: enabled }),

      setSingleField: (fileName, key, value) =>
        set((state) => {
          const { syncEnabled, groupKeys, formData } = state;
          const prev = formData.singleFieldsByFile;

          if (syncEnabled && groupKeys) {
            // Propagate value to every file that has this key
            const updated = { ...prev };
            for (const file of groupKeys.files) {
              if (file.keys.includes(key)) {
                updated[file.fileName] = { ...(updated[file.fileName] ?? {}), [key]: value };
              }
            }
            return { formData: { ...formData, singleFieldsByFile: updated } };
          } else {
            return {
              formData: {
                ...formData,
                singleFieldsByFile: {
                  ...prev,
                  [fileName]: { ...(prev[fileName] ?? {}), [key]: value },
                },
              },
            };
          }
        }),

      setAllSingleFields: (fields) =>
        set((state) => {
          const { groupKeys, formData } = state;
          if (!groupKeys) return {};

          // Distribute flat fields to every file that declares each key
          const updated = { ...formData.singleFieldsByFile };
          for (const file of groupKeys.files) {
            const fileFields: Record<string, string> = { ...(updated[file.fileName] ?? {}) };
            for (const key of file.keys) {
              if (key in fields) fileFields[key] = fields[key];
            }
            updated[file.fileName] = fileFields;
          }
          return { formData: { ...formData, singleFieldsByFile: updated } };
        }),

      setTableData: (tableKey, rows) =>
        set((state) => ({
          formData: {
            ...state.formData,
            tableData: { ...state.formData.tableData, [tableKey]: rows },
          },
        })),

      setTotalRows: (rows) => set({ totalRows: rows }),
      resetFormData: () => set({ formData: emptyFormData, totalRows: 0 }),

      getMergedSingleFields: () => {
        const { formData } = get();
        const merged: Record<string, string> = {};
        for (const fields of Object.values(formData.singleFieldsByFile)) {
          Object.assign(merged, fields);
        }
        return merged;
      },
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
