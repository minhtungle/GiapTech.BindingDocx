import { useRef, useState } from 'react';
import {
  Button,
  Form,
  Input,
  Table,
  Typography,
  Divider,
  Space,
  Tag,
  message,
  Empty,
  Spin,
} from 'antd';
import {
  DownloadOutlined,
  UploadOutlined,
  TableOutlined,
  InfoCircleOutlined,
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
    groupKeys,
    formData,
    setSingleField,
    setAllSingleFields,
    setTableData,
    setTotalRows,
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
      for (const [fileName, rows] of Object.entries(result.tableData)) {
        setTableData(fileName, rows);
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

  const hasData = Object.keys(formData.singleFields).some((k) => formData.singleFields[k]) ||
    Object.values(formData.tableData).some((rows) => rows.length > 0);

  return (
    <div style={{ padding: '12px 16px', height: '100%', overflowY: 'auto' }}>
      {/* Action buttons */}
      <Space style={{ marginBottom: 16 }}>
        <Button
          icon={<DownloadOutlined />}
          onClick={handleExportTemplate}
          loading={exportingTemplate}
        >
          Xuất Excel mẫu
        </Button>
        <Button
          icon={<UploadOutlined />}
          type={hasData ? 'default' : 'primary'}
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

      {/* Single fields */}
      {groupKeys.singleFields.length > 0 && (
        <>
          <Divider style={{ marginTop: 0 }}>
            <Text strong style={{ fontSize: 13 }}>
              Thông tin chung
              <Tag color="blue" style={{ marginLeft: 8, fontSize: 11 }}>
                {groupKeys.singleFields.length} trường
              </Tag>
            </Text>
          </Divider>

          <Form layout="vertical" size="small">
            <div
              style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
                gap: '0 24px',
              }}
            >
              {groupKeys.singleFields.map((field) => (
                <Form.Item
                  key={field}
                  label={
                    <Text style={{ fontSize: 12, fontFamily: 'monospace', color: '#595959' }}>
                      {field}
                    </Text>
                  }
                  style={{ marginBottom: 12 }}
                >
                  <Input
                    value={formData.singleFields[field] ?? ''}
                    onChange={(e) => setSingleField(field, e.target.value)}
                    placeholder={`Nhập ${field}...`}
                    allowClear
                  />
                </Form.Item>
              ))}
            </div>
          </Form>
        </>
      )}

      {/* Table files */}
      {groupKeys.tableFiles.map((tableFile) => {
        const rows = formData.tableData[tableFile.fileName] ?? [];

        const columns: ColumnType<Record<string, unknown>>[] = tableFile.columns.map((col) => ({
          title: <span style={{ fontFamily: 'monospace', fontSize: 11 }}>{col}</span>,
          dataIndex: col,
          key: col,
          width: 120,
          ellipsis: true,
          render: (val: unknown) => (val as string) || <Text type="secondary">—</Text>,
        }));

        return (
          <div key={tableFile.fileName} style={{ marginBottom: 24 }}>
            <Divider style={{ marginTop: 8 }}>
              <Space>
                <TableOutlined style={{ color: '#217346' }} />
                <Text strong style={{ fontSize: 13 }}>
                  {tableFile.displayName}
                </Text>
                <Tag color={rows.length > 0 ? 'green' : 'default'} style={{ fontSize: 11 }}>
                  {rows.length > 0 ? `${rows.length} dòng` : 'Chưa có dữ liệu'}
                </Tag>
              </Space>
            </Divider>

            {rows.length === 0 ? (
              <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description={
                  <span style={{ fontSize: 12, color: '#8c8c8c' }}>
                    <InfoCircleOutlined style={{ marginRight: 4 }} />
                    Xuất Excel mẫu → điền dữ liệu → Import để thêm dữ liệu bảng
                  </span>
                }
                style={{ margin: '12px 0' }}
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

      {groupKeys.singleFields.length === 0 && groupKeys.tableFiles.length === 0 && (
        <Empty description="Không tìm thấy key nào trong nhóm này" />
      )}
    </div>
  );
}
