import { Drawer, Descriptions, Table, Tabs, Card, Button, Tag, Empty, Spin } from 'antd';
import { WalletOutlined } from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { tokenService } from '../../services/tokenService';
import { useAppStore } from '../../stores/appStore';
import { useAuthStore } from '../../stores/authStore';
import type { TokenTransaction, TokenPackage } from '../../types';

export default function TokenDrawer() {
  const tokenDrawerOpen = useAppStore((s) => s.tokenDrawerOpen);
  const setTokenDrawerOpen = useAppStore((s) => s.setTokenDrawerOpen);
  const user = useAuthStore((s) => s.user);

  const { data: balance, isLoading: balanceLoading } = useQuery({
    queryKey: ['tokenBalance'],
    queryFn: () => tokenService.getBalance(),
    enabled: tokenDrawerOpen && !!user,
  });

  const { data: packages = [], isLoading: packagesLoading } = useQuery({
    queryKey: ['tokenPackages'],
    queryFn: () => tokenService.getPackages(),
    enabled: tokenDrawerOpen,
  });

  const { data: history, isLoading: historyLoading } = useQuery({
    queryKey: ['tokenHistory'],
    queryFn: () => tokenService.getHistory(1, 20),
    enabled: tokenDrawerOpen && !!user,
  });

  const transactionColumns = [
    {
      title: 'Loại',
      dataIndex: 'type',
      key: 'type',
      render: (type: string) => (
        <Tag color={type === 'purchase' ? 'green' : 'red'}>
          {type === 'purchase' ? 'Nạp' : 'Sử dụng'}
        </Tag>
      ),
    },
    { title: 'Số lượng', dataIndex: 'amount', key: 'amount', render: (v: number) => v.toLocaleString() },
    { title: 'Mô tả', dataIndex: 'description', key: 'description', ellipsis: true },
    {
      title: 'Thời gian',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (v: string) => new Date(v).toLocaleString('vi-VN'),
    },
  ];

  const items = [
    {
      key: 'balance',
      label: 'Số dư',
      children: balanceLoading ? <Spin /> : (
        <Descriptions bordered size="small">
          <Descriptions.Item label="Token hiện tại" span={3}>
            <span style={{ fontSize: 24, fontWeight: 700, color: '#52c41a' }}>
              {(balance?.currentToken ?? 0).toLocaleString()}
            </span>
          </Descriptions.Item>
        </Descriptions>
      ),
    },
    {
      key: 'history',
      label: 'Lịch sử',
      children: historyLoading ? <Spin /> : (
        <Table<TokenTransaction>
          dataSource={history?.items ?? []}
          columns={transactionColumns}
          rowKey="id"
          size="small"
          pagination={{ pageSize: 10 }}
          locale={{ emptyText: <Empty description="Chưa có giao dịch" /> }}
        />
      ),
    },
    {
      key: 'packages',
      label: 'Mua token',
      children: packagesLoading ? <Spin /> : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
          {packages.map((pkg: TokenPackage) => (
            <Card key={pkg.id} size="small" style={{ borderRadius: 8 }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                  <div style={{ fontWeight: 600, fontSize: 14 }}>{pkg.name}</div>
                  <div style={{ color: '#8c8c8c', fontSize: 12 }}>
                    {pkg.pricePerToken.toLocaleString()} đ/token
                  </div>
                </div>
                <div style={{ textAlign: 'right' }}>
                  <div style={{ fontWeight: 700, color: '#1890ff', fontSize: 16 }}>
                    {pkg.tokenAmount.toLocaleString()} token
                  </div>
                  <div style={{ color: '#52c41a', fontSize: 13 }}>
                    {pkg.totalPrice.toLocaleString()} đ
                  </div>
                </div>
              </div>
              <Button type="primary" size="small" block style={{ marginTop: 8 }}>
                Mua ngay
              </Button>
            </Card>
          ))}
        </div>
      ),
    },
  ];

  return (
    <Drawer
      title={<span><WalletOutlined /> Token</span>}
      open={tokenDrawerOpen}
      onClose={() => setTokenDrawerOpen(false)}
      width={480}
      placement="right"
    >
      <Tabs items={items} />
    </Drawer>
  );
}
