import { Button, Space, Badge, Dropdown, type MenuProps } from 'antd';
import {
  WalletOutlined,
  SettingOutlined,
  LogoutOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  UserOutlined,
  CrownOutlined,
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { tokenService } from '../../services/tokenService';
import { useAppStore } from '../../stores/appStore';
import { useAuthStore } from '../../stores/authStore';
import { authService } from '../../services/authService';
import { useNavigate } from 'react-router-dom';

export default function Toolbar() {
  const navigate = useNavigate();
  const { sidebarCollapsed, setSidebarCollapsed, setTokenDrawerOpen, setSettingsDrawerOpen } = useAppStore();

  const logout = useAuthStore((s) => s.logout);
  const user = useAuthStore((s) => s.user);
  const isAdmin = useAuthStore((s) => s.isAdmin);

  const { data: balance } = useQuery({
    queryKey: ['tokenBalance'],
    queryFn: () => tokenService.getBalance(),
    retry: false,
  });

  const handleLogout = async () => {
    try { await authService.logout(); } catch { /* ignore */ }
    logout();
    navigate('/login');
  };

  const userMenuItems: MenuProps['items'] = [
    { key: 'profile', label: 'Hồ sơ cá nhân', icon: <UserOutlined />, onClick: () => navigate('/profile') },
    ...(isAdmin ? [{ key: 'admin', label: 'Trang quản trị', icon: <CrownOutlined />, onClick: () => navigate('/admin') }] : []),
    { type: 'divider' as const },
    { key: 'logout', label: 'Đăng xuất', icon: <LogoutOutlined />, danger: true, onClick: handleLogout },
  ];

  return (
    <Space size={8} style={{ width: '100%', justifyContent: 'space-between' }}>
      <Button
        type="text"
        icon={sidebarCollapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
        onClick={() => setSidebarCollapsed(!sidebarCollapsed)}
        style={{ padding: '0 8px' }}
      />

      <Space size={8}>
        <Badge count={balance?.currentToken ?? 0} showZero overflowCount={99999}
          style={{ backgroundColor: '#52c41a' }}>
          <Button size="small" icon={<WalletOutlined />} onClick={() => setTokenDrawerOpen(true)}>
            Token
          </Button>
        </Badge>

        <Button size="small" icon={<SettingOutlined />} onClick={() => setSettingsDrawerOpen(true)} />

        <Dropdown menu={{ items: userMenuItems }} trigger={['click']}>
          <Button size="small" type="text">{user?.username}</Button>
        </Dropdown>
      </Space>
    </Space>
  );
}
