import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { AuthResponse } from '../types';

interface AuthState {
  user: Omit<AuthResponse, 'accessToken' | 'refreshToken' | 'expiresAt'> | null;
  isAuthenticated: boolean;
  login: (auth: AuthResponse) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      isAuthenticated: false,

      login: (auth: AuthResponse) => {
        localStorage.setItem('accessToken', auth.accessToken);
        localStorage.setItem('refreshToken', auth.refreshToken);
        set({
          user: { userId: auth.userId, username: auth.username, email: auth.email },
          isAuthenticated: true,
        });
      },

      logout: () => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        set({ user: null, isAuthenticated: false });
      },
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({ user: state.user, isAuthenticated: state.isAuthenticated }),
    }
  )
);
