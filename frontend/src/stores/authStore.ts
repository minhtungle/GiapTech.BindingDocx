import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { AuthResponse } from '../types';

interface UserInfo {
  userId: string;
  username: string;
  email: string;
  role: string;
}

interface AuthState {
  user: UserInfo | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: (auth: AuthResponse) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      isAuthenticated: false,
      isAdmin: false,

      login: (auth: AuthResponse) => {
        localStorage.setItem('accessToken', auth.accessToken);
        localStorage.setItem('refreshToken', auth.refreshToken);
        set({
          user: { userId: auth.userId, username: auth.username, email: auth.email, role: auth.role },
          isAuthenticated: true,
          isAdmin: auth.role === 'admin',
        });
      },

      logout: () => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        set({ user: null, isAuthenticated: false, isAdmin: false });
      },
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({ user: state.user, isAuthenticated: state.isAuthenticated, isAdmin: state.isAdmin }),
    }
  )
);
