import { Layout } from 'antd';
import { Outlet } from 'react-router-dom';
import Toolbar from '../components/Toolbar/Toolbar';
import Sidebar from '../components/Sidebar/Sidebar';
import TokenDrawer from '../components/TokenDrawer/TokenDrawer';
import SettingsDrawer from '../components/SettingsDrawer/SettingsDrawer';
import { useAppStore } from '../stores/appStore';

const { Header, Sider, Content } = Layout;

export default function MainLayout() {
  const sidebarCollapsed = useAppStore((s) => s.sidebarCollapsed);

  return (
    <Layout style={{ height: '100vh', overflow: 'hidden' }}>
      <Header style={{
        padding: '0 16px',
        background: '#fff',
        borderBottom: '1px solid #f0f0f0',
        height: 48,
        lineHeight: '48px',
        display: 'flex',
        alignItems: 'center',
      }}>
        <Toolbar />
      </Header>

      <Layout>
        <Sider
          collapsible
          collapsed={sidebarCollapsed}
          collapsedWidth={60}
          width={260}
          style={{ background: '#fff', borderRight: '1px solid #f0f0f0', overflow: 'auto' }}
          trigger={null}
        >
          <Sidebar />
        </Sider>

        <Content style={{ overflow: 'auto', background: '#fafafa' }}>
          <Outlet />
        </Content>
      </Layout>

      <TokenDrawer />
      <SettingsDrawer />
    </Layout>
  );
}
