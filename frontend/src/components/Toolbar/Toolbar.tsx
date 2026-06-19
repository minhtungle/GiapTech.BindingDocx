import { Button, Select, Space, Badge, Dropdown, type MenuProps } from 'antd';
import {
  ImportOutlined,
  ThunderboltOutlined,
  FileZipOutlined,
  WalletOutlined,
  SettingOutlined,
  LogoutOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { profileGroupService } from '../../services/profileGroupService';
import { tokenService } from '../../services/tokenService';
import { useAppStore } from '../../stores/appStore';
import { useAuthStore } from '../../stores/authStore';
import { authService } from '../../services/authService';
import { useNavigate } from 'react-router-dom';

export default function Toolbar() {
  const navigate = useNavigate();
  const {
    sidebarCollapsed,
    setSidebarCollapsed,
    selectedGroupId,
    setSelectedGroup,
    setTokenDrawerOpen,
    setSettingsDrawerOpen,
  } = useAppStore();

  const logout = useAuthStore((s) => s.logout);
  const user = useAuthStore((s) => s.user);

  const { data: groups = [] } = useQuery({
    queryKey: ['profileGroups'],
    queryFn: () => profileGroupService.getAll(),
  });

  const { data: balance } = useQuery({
    queryKey: ['tokenBalance'],
    queryFn: () => tokenService.getBalance(),
    retry: false,
  });

  const handleGroupChange = (groupId: string) => {
    const group = groups.find((g) => g.id === groupId) || null;
    setSelectedGroup(group);
  };

  const handleLogout = async () => {
    try { await authService.logout(); } catch { /* ignore */ }
    logout();
    navigate('/login');
  };

  const userMenuItems: MenuProps['items'] = [
    { key: 'logout', label: 'Đăng xuất', icon: <LogoutOutlined />, onClick: handleLogout },
  ];

  return (
    <Space size={8} style={{ width: '100%', justifyContent: 'space-between' }}>
      <Space size={8}>
        <Button
          type="text"
          icon={sidebarCollapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
          onClick={() => setSidebarCollapsed(!sidebarCollapsed)}
          style={{ padding: '0 8px' }}
        />

        <Select
          placeholder="Chọn nhóm hồ sơ"
          style={{ width: 200 }}
          value={selectedGroupId || undefined}
          onChange={handleGroupChange}
          options={groups.map((g) => ({ value: g.id, label: g.name }))}
          size="small"
        />

        <Button size="small" icon={<ImportOutlined />} type="default">
          Import
        </Button>

        <Button size="small" icon={<ThunderboltOutlined />} type="primary">
          Generate
        </Button>

        <Button size="small" icon={<FileZipOutlined />}>
          Export ZIP
        </Button>
      </Space>

      <Space size={8}>
        <Badge count={balance?.currentToken ?? 0} showZero overflowCount={99999}
          style={{ backgroundColor: '#52c41a' }}>
          <Button
            size="small"
            icon={<WalletOutlined />}
            onClick={() => setTokenDrawerOpen(true)}
          >
            Token
          </Button>
        </Badge>

        <Button
          size="small"
          icon={<SettingOutlined />}
          onClick={() => setSettingsDrawerOpen(true)}
        />

        <Dropdown menu={{ items: userMenuItems }} trigger={['click']}>
          <Button size="small" type="text">
            {user?.username}
          </Button>
        </Dropdown>
      </Space>
    </Space>
  );
}
