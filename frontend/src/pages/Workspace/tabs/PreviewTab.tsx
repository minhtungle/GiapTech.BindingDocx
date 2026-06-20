import { Descriptions, Table, Tag, Typography, Empty, Divider, Space } from 'antd';
import { TableOutlined, CheckCircleOutlined, CloseCircleOutlined } from '@ant-design/icons';
import type { ColumnType } from 'antd/es/table';
import { useAppStore } from '../../../stores/appStore';

const { Text } = Typography;

export default function PreviewTab() {
  const { groupKeys, formData } = useAppStore();

  if (!groupKeys) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
        <Empty description="Chọn nhóm và nhập dữ liệu để xem preview" />
      </div>
    );
  }

  const filledCount = groupKeys.singleFields.filter((f) => formData.singleFields[f]?.trim()).length;
  const allFilled = filledCount === groupKeys.singleFields.length && groupKeys.singleFields.length > 0;

  return (
    <div style={{ padding: '12px 16px', height: '100%', overflowY: 'auto' }}>
      {/* Single fields preview */}
      {groupKeys.singleFields.length > 0 && (
        <>
          <Divider style={{ marginTop: 0 }}>
            <Space>
              <Text strong style={{ fontSize: 13 }}>Thông tin chung</Text>
              <Tag color={allFilled ? 'green' : 'orange'} style={{ fontSize: 11 }}>
                {allFilled ? (
                  <><CheckCircleOutlined /> Đầy đủ</>
                ) : (
                  <><CloseCircleOutlined /> {filledCount}/{groupKeys.singleFields.length} trường</>
                )}
              </Tag>
            </Space>
          </Divider>

          <Descriptions
            bordered
            size="small"
            column={{ xs: 1, sm: 2, md: 2, lg: 3 }}
            style={{ marginBottom: 24 }}
          >
            {groupKeys.singleFields.map((field) => {
              const value = formData.singleFields[field];
              return (
                <Descriptions.Item
                  key={field}
                  label={
                    <Text style={{ fontFamily: 'monospace', fontSize: 11, color: '#595959' }}>
                      {field}
                    </Text>
                  }
                >
                  {value ? (
                    <Text>{value}</Text>
                  ) : (
                    <Text type="secondary" italic style={{ fontSize: 12 }}>
                      (chưa nhập)
                    </Text>
                  )}
                </Descriptions.Item>
              );
            })}
          </Descriptions>
        </>
      )}

      {/* Table previews */}
      {groupKeys.tableFiles.map((tableFile) => {
        const rows = formData.tableData[tableFile.fileName] ?? [];

        const columns: ColumnType<Record<string, unknown>>[] = tableFile.columns.map((col) => ({
          title: <span style={{ fontFamily: 'monospace', fontSize: 11 }}>{col}</span>,
          dataIndex: col,
          key: col,
          width: 120,
          ellipsis: true,
          render: (val: unknown) =>
            val ? <Text style={{ fontSize: 12 }}>{val as string}</Text> : <Text type="secondary">—</Text>,
        }));

        return (
          <div key={tableFile.fileName} style={{ marginBottom: 24 }}>
            <Divider style={{ marginTop: 8 }}>
              <Space>
                <TableOutlined style={{ color: '#217346' }} />
                <Text strong style={{ fontSize: 13 }}>{tableFile.displayName}</Text>
                <Tag color={rows.length > 0 ? 'green' : 'default'} style={{ fontSize: 11 }}>
                  {rows.length > 0 ? `${rows.length} dòng dữ liệu` : 'Chưa có dữ liệu'}
                </Tag>
              </Space>
            </Divider>

            {rows.length === 0 ? (
              <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description="Chưa có dữ liệu bảng — Import Excel để thêm"
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
    </div>
  );
}
