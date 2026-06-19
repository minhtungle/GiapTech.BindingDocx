import { useState } from 'react';
import { Tree, Input, Button, Empty, Spin, Tooltip } from 'antd';
import { FolderOutlined, FileTextOutlined, ReloadOutlined, SearchOutlined } from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import type { DataNode } from 'antd/es/tree';
import { profileGroupService } from '../../services/profileGroupService';
import { useAppStore } from '../../stores/appStore';

export default function Sidebar() {
  const [searchValue, setSearchValue] = useState('');
  const { sidebarCollapsed, selectedGroupId, setSelectedGroup } = useAppStore();

  const { data: groups = [], isLoading, refetch } = useQuery({
    queryKey: ['profileGroups'],
    queryFn: () => profileGroupService.getAll(),
  });

  const filteredGroups = groups.filter((g) =>
    g.name.toLowerCase().includes(searchValue.toLowerCase())
  );

  const treeData: DataNode[] = filteredGroups.map((group) => ({
    key: group.id,
    title: group.name,
    icon: <FolderOutlined style={{ color: '#faad14' }} />,
    children: [
      {
        key: `${group.id}-templates`,
        title: 'Templates',
        icon: <FileTextOutlined style={{ color: '#1890ff' }} />,
        isLeaf: true,
      },
    ],
  }));

  const handleSelect = (selectedKeys: React.Key[]) => {
    if (selectedKeys.length === 0) return;
    const key = selectedKeys[0] as string;
    if (key.includes('-')) return;
    const group = groups.find((g) => g.id === key) || null;
    setSelectedGroup(group);
  };

  if (sidebarCollapsed) {
    return (
      <div style={{ padding: '8px 4px', display: 'flex', flexDirection: 'column', gap: 8 }}>
        <Tooltip title="Làm mới" placement="right">
          <Button
            type="text"
            icon={<ReloadOutlined />}
            onClick={() => refetch()}
            style={{ width: '100%' }}
          />
        </Tooltip>
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <div style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', display: 'flex', gap: 4 }}>
        <Input
          size="small"
          prefix={<SearchOutlined />}
          placeholder="Tìm kiếm..."
          value={searchValue}
          onChange={(e) => setSearchValue(e.target.value)}
          allowClear
        />
        <Tooltip title="Làm mới">
          <Button
            size="small"
            icon={<ReloadOutlined />}
            onClick={() => refetch()}
          />
        </Tooltip>
      </div>

      <div style={{ flex: 1, overflow: 'auto', padding: '4px 0' }}>
        {isLoading ? (
          <div style={{ textAlign: 'center', padding: 24 }}>
            <Spin size="small" />
          </div>
        ) : treeData.length === 0 ? (
          <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} description="Không có dữ liệu" style={{ margin: '24px 0' }} />
        ) : (
          <Tree
            showIcon
            defaultExpandAll
            treeData={treeData}
            selectedKeys={selectedGroupId ? [selectedGroupId] : []}
            onSelect={handleSelect}
            style={{ fontSize: 13 }}
          />
        )}
      </div>
    </div>
  );
}
