import { useState } from 'react';
import { Table, Tag, Typography } from 'antd';
import { useQuery } from '@tanstack/react-query';
import { adminTokenService } from '../../../services/adminTokenService';
import type { AdminTokenTransaction } from '../../../types';

const typeColor: Record<string, string> = {
  purchase: 'green',
  use: 'blue',
  admin: 'purple',
  deduct: 'red',
};

export default function AdminTransactionsPage() {
  const [page, setPage] = useState(1);

  const { data, isLoading } = useQuery({
    queryKey: ['admin-transactions', page],
    queryFn: () => adminTokenService.getAllTransactions({ page, pageSize: 30 }),
  });

  const columns = [
    {
      title: 'Người dùng', dataIndex: 'username', key: 'username',
      render: (v: string) => <Typography.Text strong>{v}</Typography.Text>,
    },
    {
      title: 'Loại', dataIndex: 'type', key: 'type',
      render: (v: string) => <Tag color={typeColor[v] ?? 'default'}>{v}</Tag>,
    },
    {
      title: 'Số lượng', dataIndex: 'amount', key: 'amount',
      render: (v: number) => (
        <Typography.Text style={{ color: v > 0 ? '#52c41a' : '#f5222d', fontWeight: 600 }}>
          {v > 0 ? '+' : ''}{v.toLocaleString()}
        </Typography.Text>
      ),
    },
    { title: 'Ghi chú', dataIndex: 'description', key: 'description', render: (v?: string) => v ?? '—' },
    {
      title: 'Thời gian', dataIndex: 'createdAt', key: 'createdAt',
      render: (v: string) => new Date(v).toLocaleString('vi-VN'),
    },
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <Typography.Title level={4} style={{ margin: 0 }}>Lịch sử giao dịch token</Typography.Title>
      </div>

      <Table
        columns={columns}
        dataSource={data?.items as AdminTokenTransaction[]}
        rowKey="id"
        loading={isLoading}
        pagination={{
          current: page, total: data?.totalCount, pageSize: 30,
          onChange: setPage, showTotal: t => `Tổng ${t} giao dịch`,
        }}
        size="middle"
      />
    </div>
  );
}
