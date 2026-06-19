import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { ProfileGroup } from '../types';

interface AppState {
  sidebarCollapsed: boolean;
  selectedGroupId: string | null;
  selectedGroup: ProfileGroup | null;
  activeTab: 'data' | 'preview' | 'mapping';
  tokenDrawerOpen: boolean;
  settingsDrawerOpen: boolean;
  setSidebarCollapsed: (collapsed: boolean) => void;
  setSelectedGroup: (group: ProfileGroup | null) => void;
  setActiveTab: (tab: 'data' | 'preview' | 'mapping') => void;
  setTokenDrawerOpen: (open: boolean) => void;
  setSettingsDrawerOpen: (open: boolean) => void;
}

export const useAppStore = create<AppState>()(
  persist(
    (set) => ({
      sidebarCollapsed: false,
      selectedGroupId: null,
      selectedGroup: null,
      activeTab: 'data',
      tokenDrawerOpen: false,
      settingsDrawerOpen: false,

      setSidebarCollapsed: (collapsed) => set({ sidebarCollapsed: collapsed }),
      setSelectedGroup: (group) => set({ selectedGroup: group, selectedGroupId: group?.id ?? null }),
      setActiveTab: (tab) => set({ activeTab: tab }),
      setTokenDrawerOpen: (open) => set({ tokenDrawerOpen: open }),
      setSettingsDrawerOpen: (open) => set({ settingsDrawerOpen: open }),
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
