import { useRef, useState } from 'react';
import {
  Button, Collapse, Form, Input, Table, Typography,
  Space, Tag, message, Empty, Spin, Switch, Modal,
} from 'antd';
import {
  DownloadOutlined, UploadOutlined, TableOutlined,
  InfoCircleOutlined, FileWordOutlined, FileExcelOutlined,
  SyncOutlined,
} from '@ant-design/icons';
import type { ColumnType } from 'antd/es/table';
import { useAppStore } from '../../../stores/appStore';
import { workspaceService } from '../../../services/workspaceService';

const { Text } = Typography;

interface KeysTabProps {
  groupId: string;
}

export default function KeysTab({ groupId }: KeysTabProps) {
  const {
    groupKeys, formData, syncEnabled,
    setSingleField, setAllSingleFields, setTableData,
    setTotalRows, setSyncEnabled,
  } = useAppStore();

  const [exportingTemplate, setExportingTemplate] = useState(false);
  const [importingData, setImportingData] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  if (!groupKeys) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
        <Spin />
      </div>
    );
  }

  const handleExportTemplate = async () => {
    setExportingTemplate(true);
    try {
      await workspaceService.exportTemplate(groupId);
      message.success('Đã tải Excel mẫu thành công');
    } catch {
      message.error('Không thể xuất Excel mẫu');
    } finally {
      setExportingTemplate(false);
    }
  };

  const handleImportClick = () => fileInputRef.current?.click();

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    e.target.value = '';

    setImportingData(true);
    try {
      const result = await workspaceService.importData(groupId, file);
      setAllSingleFields(result.singleFields);
      for (const [tableKey, rows] of Object.entries(result.tableData)) {
        setTableData(tableKey, rows);
      }
      const rowCount = Object.values(result.tableData).reduce((s, r) => s + r.length, 0);
      setTotalRows(Math.max(1, rowCount));
      message.success(`Đã import: ${Object.keys(result.singleFields).length} trường + ${rowCount} dòng dữ liệu`);
    } catch {
      message.error('Không thể đọc file Excel');
    } finally {
      setImportingData(false);
    }
  };

  const handleSyncToggle = (checked: boolean) => {
    if (!checked) {
      Modal.confirm({
        title: 'Tắt đồng bộ dữ liệu?',
        content: 'Khi tắt đồng bộ, mỗi file sẽ có dữ liệu riêng. Dữ liệu hiện tại sẽ được giữ nguyên theo từng file.',
        okText: 'Tắt đồng bộ',
        cancelText: 'Huỷ',
        onOk: () => setSyncEnabled(false),
      });
    } else {
      setSyncEnabled(true);
    }
  };

  // Build per-file key panels
  const filePanels = groupKeys.files.map((file) => {
    const isDocx = file.fileType === 'docx';
    const fileFields = formData.singleFieldsByFile[file.fileName] ?? {};

    // Tables that belong to this file
    const fileTables = groupKeys.tableFiles.filter((t) => t.fileName === file.fileName);

    const hasKeys = file.keys.length > 0 || fileTables.length > 0;

    return {
      key: file.fileName,
      label: (
        <Space size={6}>
          {isDocx
            ? <FileWordOutlined style={{ color: '#2b579a' }} />
            : <FileExcelOutlined style={{ color: '#217346' }} />}
          <Text strong style={{ fontSize: 13 }}>{file.displayName}</Text>
          <Tag color={isDocx ? 'blue' : 'green'} style={{ fontSize: 10, padding: '0 4px' }}>
            {file.fileType.toUpperCase()}
          </Tag>
          {file.keys.length > 0 && (
            <Tag color="default" style={{ fontSize: 11 }}>
              {file.keys.length} trường
            </Tag>
          )}
          {fileTables.length > 0 && (
            <Tag color="cyan" style={{ fontSize: 11 }}>
              {fileTables.length} bảng
            </Tag>
          )}
        </Space>
      ),
      children: !hasKeys ? (
        <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} description="File này không có key nào" style={{ margin: '8px 0' }} />
      ) : (
        <div>
          {/* Single fields for this file */}
          {file.keys.length > 0 && (
            <Form layout="vertical" size="small">
              <div style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fill, minmax(260px, 1fr))',
                gap: '0 24px',
              }}>
                {file.keys.map((key) => (
                  <Form.Item
                    key={key}
                    label={<Text style={{ fontSize: 12, fontFamily: 'monospace', color: '#595959' }}>{key}</Text>}
                    style={{ marginBottom: 10 }}
                  >
                    <Input
                      value={fileFields[key] ?? ''}
                      onChange={(e) => setSingleField(file.fileName, key, e.target.value)}
                      placeholder={`Nhập ${key}...`}
                      allowClear
                    />
                  </Form.Item>
                ))}
              </div>
            </Form>
          )}

          {/* Tables for this file */}
          {fileTables.map((tableFile) => {
            const rows = formData.tableData[tableFile.tableKey] ?? [];
            const columns: ColumnType<Record<string, unknown>>[] = tableFile.columns.map((col) => ({
              title: <span style={{ fontFamily: 'monospace', fontSize: 11 }}>{col}</span>,
              dataIndex: col,
              key: col,
              width: 120,
              ellipsis: true,
              render: (val: unknown) => (val as string) || <Text type="secondary">—</Text>,
            }));

            return (
              <div key={tableFile.tableKey} style={{ marginBottom: 16, marginTop: 8 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 8 }}>
                  <TableOutlined style={{ color: '#217346' }} />
                  <Text strong style={{ fontSize: 12 }}>{tableFile.displayName}</Text>
                  <Tag color={rows.length > 0 ? 'green' : 'default'} style={{ fontSize: 11 }}>
                    {rows.length > 0 ? `${rows.length} dòng` : 'Chưa có dữ liệu'}
                  </Tag>
                </div>
                {rows.length === 0 ? (
                  <Empty
                    image={Empty.PRESENTED_IMAGE_SIMPLE}
                    description={
                      <span style={{ fontSize: 12, color: '#8c8c8c' }}>
                        <InfoCircleOutlined style={{ marginRight: 4 }} />
                        Xuất Excel mẫu → điền dữ liệu → Import
                      </span>
                    }
                    style={{ margin: '8px 0' }}
                  />
                ) : (
                  <Table
                    columns={columns}
                    dataSource={rows.map((r, i) => ({ ...r, _key: i }))}
                    rowKey="_key"
                    size="small"
                    scroll={{ x: 'max-content' }}
                    pagination={rows.length > 10 ? { pageSize: 10, size: 'small' } : false}
                    style={{ fontSize: 12 }}
                  />
                )}
              </div>
            );
          })}
        </div>
      ),
    };
  });

  return (
    <div style={{ padding: '12px 16px', height: '100%', overflowY: 'auto' }}>
      {/* Toolbar */}
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 16 }}>
        <Space>
          <Button icon={<DownloadOutlined />} onClick={handleExportTemplate} loading={exportingTemplate}>
            Xuất Excel mẫu
          </Button>
          <Button
            icon={<UploadOutlined />}
            type="primary"
            onClick={handleImportClick}
            loading={importingData}
          >
            Import Excel đã điền
          </Button>
          <input
            ref={fileInputRef}
            type="file"
            accept=".xlsx,.xls"
            style={{ display: 'none' }}
            onChange={handleFileChange}
          />
        </Space>

        <Space size={8} style={{ fontSize: 13 }}>
          <SyncOutlined style={{ color: syncEnabled ? '#1677ff' : '#bfbfbf' }} />
          <Text style={{ fontSize: 13 }}>Đồng bộ dữ liệu</Text>
          <Switch
            checked={syncEnabled}
            onChange={handleSyncToggle}
            size="small"
          />
          {syncEnabled && (
            <Tag color="blue" style={{ fontSize: 11, marginLeft: 2 }}>
              Dữ liệu chung sẽ điền vào tất cả file
            </Tag>
          )}
        </Space>
      </div>

      {/* Per-file collapsible sections */}
      {filePanels.length === 0 ? (
        <Empty description="Không tìm thấy key nào trong nhóm này" />
      ) : (
        <Collapse
          defaultActiveKey={filePanels.map((p) => p.key)}
          items={filePanels}
          size="small"
        />
      )}
    </div>
  );
}
