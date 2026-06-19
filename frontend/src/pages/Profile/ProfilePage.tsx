import { useState } from 'react';
import {
  Card, Form, Input, Button, Tabs, Table, Tag, Typography,
  Space, Avatar, Statistic, message, Row, Col,
} from 'antd';
import { UserOutlined, WalletOutlined, HistoryOutlined } from '@ant-design/icons';
import { useQuery, useMutation } from '@tanstack/react-query';
import { useAuthStore } from '../../stores/authStore';
import { tokenService } from '../../services/tokenService';
import type { TokenTransaction } from '../../types';
import { api } from '../../services/api';

const { Title, Text } = Typography;

function ChangePasswordForm() {
  const [form] = Form.useForm();
  const mut = useMutation({
    mutationFn: async (values: { currentPassword: string; newPassword: string }) => {
      const { data } = await api.post('/users/change-password', values);
      return data;
    },
    onSuccess: () => { message.success('Đổi mật khẩu thành công'); form.resetFields(); },
    onError: () => message.error('Đổi mật khẩu thất bại. Kiểm tra lại mật khẩu hiện tại.'),
  });

  return (
    <Form form={form} layout="vertical" onFinish={v => mut.mutate(v)} style={{ maxWidth: 400 }}>
      <Form.Item name="currentPassword" label="Mật khẩu hiện tại" rules={[{ required: true }]}>
        <Input.Password />
      </Form.Item>
      <Form.Item name="newPassword" label="Mật khẩu mới" rules={[{ required: true }, { min: 6 }]}>
        <Input.Password />
      </Form.Item>
      <Form.Item name="confirm" label="Xác nhận mật khẩu mới"
        rules={[{ required: true }, ({ getFieldValue }) => ({
          validator(_, value) {
            if (!value || getFieldValue('newPassword') === value) return Promise.resolve();
            return Promise.reject(new Error('Mật khẩu xác nhận không khớp'));
          },
        })]}>
        <Input.Password />
      </Form.Item>
      <Button type="primary" htmlType="submit" loading={mut.isPending}>Đổi mật khẩu</Button>
    </Form>
  );
}

function TokenHistoryTab() {
  const [page, setPage] = useState(1);
  const { data, isLoading } = useQuery({
    queryKey: ['tokenHistory', page],
    queryFn: () => tokenService.getHistory(page, 20),
  });

  const typeColor: Record<string, string> = {
    purchase: 'green',
    use: 'blue',
    admin: 'purple',
    deduct: 'red',
  };

  const columns = [
    {
      title: 'Loại', dataIndex: 'type', key: 'type',
      render: (v: string) => <Tag color={typeColor[v] ?? 'default'}>{v}</Tag>,
    },
    {
      title: 'Số lượng', dataIndex: 'amount', key: 'amount',
      render: (v: number) => (
        <Text style={{ color: v > 0 ? '#52c41a' : '#f5222d', fontWeight: 600 }}>
          {v > 0 ? '+' : ''}{v.toLocaleString()}
        </Text>
      ),
    },
    { title: 'Ghi chú', dataIndex: 'description', key: 'description', render: (v?: string) => v ?? '—' },
    {
      title: 'Thời gian', dataIndex: 'createdAt', key: 'createdAt',
      render: (v: string) => new Date(v).toLocaleString('vi-VN'),
    },
  ];

  return (
    <Table
      columns={columns}
      dataSource={data?.items as TokenTransaction[]}
      rowKey="id"
      loading={isLoading}
      pagination={{
        current: page, total: data?.totalCount, pageSize: 20,
        onChange: setPage, showTotal: t => `Tổng ${t} giao dịch`,
      }}
      size="middle"
    />
  );
}

export default function ProfilePage() {
  const user = useAuthStore((s) => s.user);

  const { data: balance } = useQuery({
    queryKey: ['tokenBalance'],
    queryFn: tokenService.getBalance,
  });

  const tabItems = [
    {
      key: 'info', label: <Space><UserOutlined />Thông tin</Space>,
      children: (
        <div style={{ maxWidth: 400 }}>
          <Form layout="vertical">
            <Form.Item label="Tên đăng nhập">
              <Input value={user?.username} disabled />
            </Form.Item>
            <Form.Item label="Email">
              <Input value={user?.email} disabled />
            </Form.Item>
            <Form.Item label="Vai trò">
              <Input value={user?.role === 'admin' ? 'Quản trị viên' : 'Người dùng'} disabled />
            </Form.Item>
          </Form>
        </div>
      ),
    },
    {
      key: 'password', label: 'Đổi mật khẩu',
      children: <ChangePasswordForm />,
    },
    {
      key: 'tokens', label: <Space><HistoryOutlined />Lịch sử token</Space>,
      children: <TokenHistoryTab />,
    },
  ];

  return (
    <div style={{ padding: 24, maxWidth: 900, margin: '0 auto' }}>
      <Card style={{ marginBottom: 24 }}>
        <Row align="middle" gutter={24}>
          <Col>
            <Avatar size={64} icon={<UserOutlined />} style={{ background: '#1677ff' }} />
          </Col>
          <Col flex="1">
            <Title level={4} style={{ margin: 0 }}>{user?.username}</Title>
            <Text type="secondary">{user?.email}</Text>
          </Col>
          <Col>
            <Statistic
              title="Số dư token"
              value={balance?.currentToken ?? 0}
              prefix={<WalletOutlined />}
              valueStyle={{ color: '#1677ff' }}
            />
          </Col>
        </Row>
      </Card>

      <Card>
        <Tabs items={tabItems} />
      </Card>
    </div>
  );
}
