import { Tabs, Empty } from 'antd';
import { TableOutlined, EyeOutlined, ApartmentOutlined } from '@ant-design/icons';
import { useAppStore } from '../../stores/appStore';
import DataTab from './tabs/DataTab';
import PreviewTab from './tabs/PreviewTab';
import MappingTab from './tabs/MappingTab';

export default function WorkspacePage() {
  const { selectedGroup, activeTab, setActiveTab } = useAppStore();

  if (!selectedGroup) {
    return (
      <div style={{ height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <Empty
          description={
            <span style={{ color: '#8c8c8c' }}>
              Chọn một nhóm hồ sơ từ sidebar để bắt đầu
            </span>
          }
        />
      </div>
    );
  }

  const items = [
    {
      key: 'data',
      label: <span><TableOutlined /> Dữ liệu</span>,
      children: <DataTab group={selectedGroup} />,
    },
    {
      key: 'preview',
      label: <span><EyeOutlined /> Preview</span>,
      children: <PreviewTab group={selectedGroup} />,
    },
    {
      key: 'mapping',
      label: <span><ApartmentOutlined /> Mapping</span>,
      children: <MappingTab group={selectedGroup} />,
    },
  ];

  return (
    <div style={{ height: '100%', padding: '0 16px', display: 'flex', flexDirection: 'column' }}>
      <div style={{ padding: '8px 0 4px', borderBottom: '1px solid #f0f0f0', marginBottom: 0 }}>
        <span style={{ fontWeight: 600, fontSize: 14, color: '#1a1a1a' }}>
          {selectedGroup.name}
        </span>
      </div>
      <Tabs
        activeKey={activeTab}
        onChange={(key) => setActiveTab(key as 'data' | 'preview' | 'mapping')}
        items={items}
        style={{ flex: 1 }}
        tabBarStyle={{ marginBottom: 0 }}
      />
    </div>
  );
}
