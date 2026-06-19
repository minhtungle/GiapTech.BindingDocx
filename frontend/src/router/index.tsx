import { createBrowserRouter, Navigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import LoginPage from '../pages/Login/LoginPage';
import WorkspacePage from '../pages/Workspace/WorkspacePage';
import MainLayout from '../layouts/MainLayout';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  return <>{children}</>;
}

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <WorkspacePage /> },
    ],
  },
  {
    path: '*',
    element: <Navigate to="/" replace />,
  },
]);
