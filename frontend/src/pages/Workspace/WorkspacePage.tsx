import { useState } from 'react';
import './WorkspacePage.css';
import { Tabs, Empty, Button, Space, Typography, Tag, Spin, message, Tooltip } from 'antd';
import {
  FormOutlined,
  EyeOutlined,
  ExportOutlined,
  ThunderboltOutlined,
} from '@ant-design/icons';
import { useAppStore } from '../../stores/appStore';
import KeysTab from './tabs/KeysTab';
import PreviewTab from './tabs/PreviewTab';
import { workspaceService } from '../../services/workspaceService';

const { Text } = Typography;

export default function WorkspacePage() {
  const {
    selectedGroup, selectedGroupId, activeTab, setActiveTab,
    formData, totalRows, keysLoading, syncEnabled, getMergedSingleFields,
  } = useAppStore();
  const [generating, setGenerating] = useState(false);

  if (!selectedGroup || !selectedGroupId) {
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

  const handleGenerate = async () => {
    setGenerating(true);
    try {
      const mergedSingleFields = getMergedSingleFields();
      await workspaceService.generateFiles(selectedGroupId, {
        syncMode: syncEnabled,
        singleFields: syncEnabled ? mergedSingleFields : {},
        singleFieldsByFile: syncEnabled ? {} : formData.singleFieldsByFile,
        tableData: formData.tableData,
      });
      message.success('Xuất file thành công! File ZIP đang được tải về.');
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      const errMsg = axiosErr?.response?.data?.message ?? 'Không thể xuất file';
      message.error(errMsg);
    } finally {
      setGenerating(false);
    }
  };

  const tokenCost = totalRows;
  const hasAnyData =
    Object.values(formData.singleFieldsByFile).some((fields) =>
      Object.values(fields).some((v) => v?.trim())
    ) ||
    Object.values(formData.tableData).some((rows) => rows.length > 0);

  const items = [
    {
      key: 'keys',
      label: (
        <span>
          <FormOutlined />
          Nhập dữ liệu
        </span>
      ),
      children: <KeysTab groupId={selectedGroupId} />,
    },
    {
      key: 'preview',
      label: (
        <span>
          <EyeOutlined />
          Preview
        </span>
      ),
      children: <PreviewTab />,
    },
  ];

  return (
    <div style={{ height: '100%', display: 'flex', flexDirection: 'column', overflow: 'hidden' }}>
      {/* Header */}
      <div
        style={{
          padding: '8px 16px',
          borderBottom: '1px solid #f0f0f0',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          background: '#fff',
          flexShrink: 0,
        }}
      >
        <Space>
          <Text strong style={{ fontSize: 14, color: '#1a1a1a' }}>
            {selectedGroup.name}
          </Text>
        </Space>
      </div>

      {/* Loading banner */}
      {keysLoading && (
        <div style={{
          display: 'flex', alignItems: 'center', gap: 8,
          padding: '6px 16px', background: '#e6f4ff',
          borderBottom: '1px solid #91caff', flexShrink: 0,
          fontSize: 12, color: '#1677ff',
        }}>
          <Spin size="small" />
          Đang tải dữ liệu nhóm...
        </div>
      )}

      {/* Tabs */}
      <Tabs
        className="workspace-tabs"
        activeKey={activeTab}
        onChange={(key) => setActiveTab(key as 'keys' | 'preview')}
        items={items}
        tabBarStyle={{ marginBottom: 0, padding: '0 16px' }}
        tabBarExtraContent={null}
      />

      {/* Fullscreen loading overlay when generating ZIP */}
      {generating && (
        <Spin
          spinning
          fullscreen
          tip="Đang tạo file ZIP, vui lòng chờ..."
          size="large"
        />
      )}

      {/* Footer */}
      <div
        style={{
          padding: '10px 16px',
          borderTop: '1px solid #f0f0f0',
          background: '#fafafa',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          flexShrink: 0,
        }}
      >
        <Space>
          <ThunderboltOutlined style={{ color: '#faad14' }} />
          <Text style={{ fontSize: 13 }}>Token cần dùng:</Text>
          <Tag color={tokenCost > 0 ? 'orange' : 'default'} style={{ fontWeight: 600, fontSize: 13 }}>
            {tokenCost} token
          </Tag>
          {tokenCost === 0 && (
            <Text type="secondary" style={{ fontSize: 12 }}>
              (tối thiểu 1 sau khi có dữ liệu)
            </Text>
          )}
        </Space>

        <Tooltip title={!hasAnyData ? 'Nhập dữ liệu trước khi xuất' : ''}>
          <Button
            type="primary"
            icon={generating ? <Spin size="small" /> : <ExportOutlined />}
            onClick={handleGenerate}
            loading={generating}
            disabled={!hasAnyData}
            size="middle"
            style={{ minWidth: 140 }}
          >
            {generating ? 'Đang xuất...' : 'Xuất ZIP'}
          </Button>
        </Tooltip>
      </div>
    </div>
  );
}
