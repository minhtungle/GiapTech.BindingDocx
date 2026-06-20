import { useState, useEffect } from 'react';
import { Select, Spin, Empty, Tag, Tooltip, Button } from 'antd';
import {
  FileWordOutlined,
  FileExcelOutlined,
  ReloadOutlined,
  FolderOpenOutlined,
  EyeOutlined,
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { workspaceService } from '../../services/workspaceService';
import { useAppStore } from '../../stores/appStore';
import FilePreviewModal from '../FilePreview/FilePreviewModal';

export default function Sidebar() {
  const { sidebarCollapsed, selectedGroupId, groupKeys, keysLoading, setSelectedGroup, setGroupKeys, setKeysLoading } = useAppStore();
  const [previewFile, setPreviewFile] = useState<string | null>(null);

  const { data: groups = [], isLoading, refetch } = useQuery({
    queryKey: ['workspaceGroups'],
    queryFn: workspaceService.getGroups,
  });

  const selectedGroup = groups.find((g) => g.id === selectedGroupId);

  // Auto-select first available group, or reload keys if group already set but keys missing (after refresh)
  useEffect(() => {
    if (groups.length === 0) return;
    if (!selectedGroupId) {
      const first = groups.find((g) => g.isAvailable);
      if (first) handleGroupChange(first.id);
    } else if (!groupKeys && !keysLoading) {
      handleGroupChange(selectedGroupId);
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [groups]);

  const handleGroupChange = async (groupId: string) => {
    const group = groups.find((g) => g.id === groupId);
    if (!group || !group.isAvailable) return;

    setSelectedGroup({
      id: group.id,
      name: group.name,
      sortOrder: 0,
      isActive: group.isAvailable,
      createdAt: '',
    });

    setKeysLoading(true);
    try {
      const keys = await workspaceService.getGroupKeys(groupId);
      setGroupKeys(keys);
    } catch {
      setGroupKeys(null);
    } finally {
      setKeysLoading(false);
    }
  };

  if (sidebarCollapsed) {
    return (
      <div style={{ padding: '8px 4px', display: 'flex', flexDirection: 'column', gap: 8 }}>
        <Tooltip title="Làm mới" placement="right">
          <Button type="text" icon={<ReloadOutlined />} onClick={() => refetch()} style={{ width: '100%' }} />
        </Tooltip>
        {selectedGroup && (
          <Tooltip title={selectedGroup.name} placement="right">
            <Button type="text" icon={<FolderOpenOutlined style={{ color: '#faad14' }} />} style={{ width: '100%' }} />
          </Tooltip>
        )}
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      {/* Group selector */}
      <div style={{ padding: '10px 12px', borderBottom: '1px solid #f0f0f0' }}>
        <div style={{
          fontSize: 11, color: '#8c8c8c', marginBottom: 6,
          fontWeight: 500, textTransform: 'uppercase', letterSpacing: 0.5,
        }}>
          Chọn nhóm hồ sơ
        </div>
        {isLoading ? (
          <Spin size="small" />
        ) : (
          <Select
            style={{ width: '100%' }}
            placeholder="Chọn nhóm..."
            value={selectedGroupId ?? undefined}
            onChange={handleGroupChange}
            options={groups.map((g) => ({
              value: g.id,
              label: g.isAvailable ? g.name : `🔒 ${g.name}`,
              disabled: !g.isAvailable,
            }))}
          />
        )}
      </div>

      {/* File list */}
      <div style={{ flex: 1, overflow: 'auto', padding: '8px 0' }}>
        {!selectedGroupId ? (
          <Empty
            image={Empty.PRESENTED_IMAGE_SIMPLE}
            description={<span style={{ fontSize: 12, color: '#bfbfbf' }}>Chọn nhóm để xem file</span>}
            style={{ margin: '24px 0' }}
          />
        ) : keysLoading ? (
          <div style={{ textAlign: 'center', padding: 24 }}>
            <Spin size="small" />
            <div style={{ marginTop: 8, fontSize: 11, color: '#8c8c8c' }}>Đang tải...</div>
          </div>
        ) : !groupKeys ? (
          <Empty
            image={Empty.PRESENTED_IMAGE_SIMPLE}
            description={<span style={{ fontSize: 12, color: '#ff4d4f' }}>Không thể tải dữ liệu</span>}
            style={{ margin: '24px 0' }}
          />
        ) : (
          <>
            <div style={{
              padding: '4px 12px 8px',
              fontSize: 11, color: '#8c8c8c',
              fontWeight: 500, textTransform: 'uppercase', letterSpacing: 0.5,
            }}>
              File đầu ra ({groupKeys.files.length} file)
            </div>

            {groupKeys.files.map((file) => {
              const isDocx = file.fileType === 'docx';
              return (
                <div
                  key={file.fileName}
                  onClick={() => setPreviewFile(file.fileName)}
                  style={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: 8,
                    padding: '7px 16px',
                    fontSize: 12,
                    color: '#262626',
                    cursor: 'pointer',
                    borderRadius: 4,
                    margin: '0 4px',
                    transition: 'background 0.15s',
                  }}
                  onMouseEnter={(e) => (e.currentTarget.style.background = '#f5f5f5')}
                  onMouseLeave={(e) => (e.currentTarget.style.background = 'transparent')}
                >
                  {isDocx
                    ? <FileWordOutlined style={{ color: '#2b579a', fontSize: 14, flexShrink: 0 }} />
                    : <FileExcelOutlined style={{ color: '#217346', fontSize: 14, flexShrink: 0 }} />
                  }

                  <Tooltip title={`${file.fileName} — click để xem`} placement="right">
                    <span style={{
                      flex: 1,
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                      whiteSpace: 'nowrap',
                      lineHeight: '1.4',
                    }}>
                      {file.displayName}
                    </span>
                  </Tooltip>

                  <div style={{ display: 'flex', alignItems: 'center', gap: 4, flexShrink: 0 }}>
                    <Tag
                      color={isDocx ? 'blue' : 'green'}
                      style={{ fontSize: 10, padding: '0 4px', margin: 0 }}
                    >
                      {file.fileType.toUpperCase()}
                    </Tag>
                    <EyeOutlined style={{ color: '#bfbfbf', fontSize: 11 }} />
                  </div>
                </div>
              );
            })}

            {groupKeys.files.length === 0 && (
              <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description={<span style={{ fontSize: 12 }}>Không có file template</span>}
                style={{ margin: '16px 0' }}
              />
            )}
          </>
        )}
      </div>

      {/* Refresh */}
      <div style={{ padding: '8px 12px', borderTop: '1px solid #f0f0f0' }}>
        <Tooltip title="Tải lại danh sách nhóm">
          <Button
            type="text" size="small"
            icon={<ReloadOutlined />}
            onClick={() => refetch()}
            style={{ width: '100%' }}
          >
            Làm mới
          </Button>
        </Tooltip>
      </div>

      {/* Preview modal */}
      {previewFile && selectedGroupId && (
        <FilePreviewModal
          groupId={selectedGroupId}
          fileName={previewFile}
          onClose={() => setPreviewFile(null)}
        />
      )}
    </div>
  );
}
