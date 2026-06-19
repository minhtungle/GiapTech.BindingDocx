import { Layout, Menu, Typography, Avatar, Dropdown, Button } from 'antd';
import { Outlet, useNavigate, useLocation, Link } from 'react-router-dom';
import {
  TeamOutlined,
  GiftOutlined,
  LogoutOutlined,
  UserOutlined,
  BarChartOutlined,
  AppstoreOutlined,
} from '@ant-design/icons';
import { useAuthStore } from '../stores/authStore';
import { authService } from '../services/authService';

const { Header, Sider, Content } = Layout;
const { Text } = Typography;

const menuItems = [
  { key: '/admin/users', icon: <TeamOutlined />, label: <Link to="/admin/users">Quản lý người dùng</Link> },
  { key: '/admin/token-packages', icon: <GiftOutlined />, label: <Link to="/admin/token-packages">Gói token</Link> },
  { key: '/admin/transactions', icon: <BarChartOutlined />, label: <Link to="/admin/transactions">Lịch sử giao dịch</Link> },
];

export default function AdminLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuthStore();

  const handleLogout = async () => {
    try { await authService.logout(); } catch {}
    logout();
    navigate('/login');
  };

  const userMenu = {
    items: [
      { key: 'workspace', icon: <AppstoreOutlined />, label: 'Về workspace', onClick: () => navigate('/') },
      { key: 'logout', icon: <LogoutOutlined />, label: 'Đăng xuất', danger: true, onClick: handleLogout },
    ],
  };

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider width={220} style={{ background: '#001529' }}>
        <div style={{ padding: '16px 20px', borderBottom: '1px solid rgba(255,255,255,0.1)' }}>
          <Text style={{ color: '#fff', fontSize: 14, fontWeight: 600 }}>⚙️ Admin Panel</Text>
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[location.pathname]}
          items={menuItems}
          style={{ marginTop: 8 }}
        />
      </Sider>

      <Layout>
        <Header style={{
          background: '#fff',
          padding: '0 24px',
          borderBottom: '1px solid #f0f0f0',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
        }}>
          <Text strong style={{ fontSize: 16 }}>GiapTech BindingDocx — Quản trị</Text>
          <Dropdown menu={userMenu} placement="bottomRight">
            <Button type="text" style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
              <Avatar size={28} icon={<UserOutlined />} style={{ background: '#1677ff' }} />
              <span style={{ fontSize: 13 }}>{user?.username}</span>
            </Button>
          </Dropdown>
        </Header>

        <Content style={{ padding: 24, background: '#f5f5f5', minHeight: 'calc(100vh - 64px)', overflow: 'auto' }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
}
