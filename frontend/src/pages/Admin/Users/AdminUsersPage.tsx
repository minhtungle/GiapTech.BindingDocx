import { useState } from 'react';
import {
  Table, Button, Space, Tag, Input, Modal, Form, Select, Switch,
  message, Tooltip, InputNumber, Typography, Popconfirm,
} from 'antd';
import {
  PlusOutlined, EditOutlined, DeleteOutlined, WalletOutlined,
} from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminUserService } from '../../../services/adminUserService';
import type { AdminUser } from '../../../types';

const { Search } = Input;
const { Text } = Typography;

export default function AdminUsersPage() {
  const qc = useQueryClient();
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [modalOpen, setModalOpen] = useState(false);
  const [tokenModalOpen, setTokenModalOpen] = useState(false);
  const [editUser, setEditUser] = useState<AdminUser | null>(null);
  const [tokenTarget, setTokenTarget] = useState<AdminUser | null>(null);
  const [form] = Form.useForm();
  const [tokenForm] = Form.useForm();

  const { data, isLoading } = useQuery({
    queryKey: ['admin-users', search, page],
    queryFn: () => adminUserService.getAll({ search, page, pageSize: 20 }),
  });

  const createMut = useMutation({
    mutationFn: adminUserService.create,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['admin-users'] }); message.success('Tạo tài khoản thành công'); closeModal(); },
    onError: () => message.error('Tạo thất bại'),
  });

  const updateMut = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: Parameters<typeof adminUserService.update>[1] }) =>
      adminUserService.update(id, payload),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['admin-users'] }); message.success('Cập nhật thành công'); closeModal(); },
    onError: () => message.error('Cập nhật thất bại'),
  });

  const deleteMut = useMutation({
    mutationFn: adminUserService.delete,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['admin-users'] }); message.success('Xóa thành công'); },
    onError: () => message.error('Xóa thất bại'),
  });

  const toggleMut = useMutation({
    mutationFn: adminUserService.toggleActive,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['admin-users'] }); },
  });

  const tokenMut = useMutation({
    mutationFn: ({ id, amount, description }: { id: string; amount: number; description: string }) =>
      adminUserService.adjustTokens(id, amount, description),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['admin-users'] }); message.success('Điều chỉnh token thành công'); setTokenModalOpen(false); tokenForm.resetFields(); },
    onError: () => message.error('Điều chỉnh thất bại'),
  });

  const openCreate = () => { setEditUser(null); form.resetFields(); form.setFieldsValue({ role: 'user', isActive: true }); setModalOpen(true); };
  const openEdit = (u: AdminUser) => { setEditUser(u); form.setFieldsValue({ username: u.username, email: u.email, role: u.role, isActive: u.isActive }); setModalOpen(true); };
  const closeModal = () => { setModalOpen(false); setEditUser(null); form.resetFields(); };

  const onSubmit = (values: { username: string; email: string; password?: string; role: string; isActive: boolean }) => {
    if (editUser) {
      updateMut.mutate({ id: editUser.id, payload: values });
    } else {
      createMut.mutate({ ...values, password: values.password! });
    }
  };

  const columns = [
    {
      title: 'Tên đăng nhập', dataIndex: 'username', key: 'username',
      render: (v: string, r: AdminUser) => (
        <Space direction="vertical" size={0}>
          <Text strong>{v}</Text>
          <Text type="secondary" style={{ fontSize: 12 }}>{r.email}</Text>
        </Space>
      ),
    },
    {
      title: 'Vai trò', dataIndex: 'role', key: 'role',
      render: (r: string) => <Tag color={r === 'admin' ? 'red' : 'blue'}>{r === 'admin' ? 'Admin' : 'User'}</Tag>,
    },
    {
      title: 'Trạng thái', dataIndex: 'isActive', key: 'isActive',
      render: (v: boolean, r: AdminUser) => (
        <Switch checked={v} size="small" onChange={() => toggleMut.mutate(r.id)} />
      ),
    },
    {
      title: 'Token', dataIndex: 'currentToken', key: 'currentToken',
      render: (v: number) => <Text style={{ color: '#1677ff', fontWeight: 600 }}>{v.toLocaleString()}</Text>,
    },
    {
      title: 'Ngày tạo', dataIndex: 'createdAt', key: 'createdAt',
      render: (v: string) => new Date(v).toLocaleDateString('vi-VN'),
    },
    {
      title: 'Hành động', key: 'actions',
      render: (_: unknown, r: AdminUser) => (
        <Space>
          <Tooltip title="Chỉnh sửa">
            <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(r)} />
          </Tooltip>
          <Tooltip title="Điều chỉnh token">
            <Button size="small" icon={<WalletOutlined />} onClick={() => { setTokenTarget(r); tokenForm.resetFields(); setTokenModalOpen(true); }} />
          </Tooltip>
          {r.role !== 'admin' && (
            <Popconfirm title="Xóa tài khoản này?" okText="Xóa" cancelText="Hủy" onConfirm={() => deleteMut.mutate(r.id)}>
              <Tooltip title="Xóa">
                <Button size="small" danger icon={<DeleteOutlined />} />
              </Tooltip>
            </Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <Typography.Title level={4} style={{ margin: 0 }}>Quản lý người dùng</Typography.Title>
        <Space>
          <Search placeholder="Tìm kiếm..." onSearch={v => { setSearch(v); setPage(1); }} allowClear style={{ width: 240 }} />
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>Thêm người dùng</Button>
        </Space>
      </div>

      <Table
        columns={columns}
        dataSource={data?.items}
        rowKey="id"
        loading={isLoading}
        pagination={{
          current: page, total: data?.totalCount, pageSize: 20,
          onChange: setPage, showTotal: t => `Tổng ${t} người dùng`,
        }}
        size="middle"
      />

      {/* Create/Edit Modal */}
      <Modal
        title={editUser ? 'Chỉnh sửa người dùng' : 'Thêm người dùng mới'}
        open={modalOpen}
        onCancel={closeModal}
        onOk={() => form.submit()}
        confirmLoading={createMut.isPending || updateMut.isPending}
        destroyOnClose
      >
        <Form form={form} layout="vertical" onFinish={onSubmit}>
          <Form.Item name="username" label="Tên đăng nhập" rules={[{ required: true }, { min: 3 }]}>
            <Input />
          </Form.Item>
          <Form.Item name="email" label="Email" rules={[{ required: true }, { type: 'email' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="password" label={editUser ? 'Mật khẩu mới (để trống nếu không đổi)' : 'Mật khẩu'}
            rules={editUser ? [] : [{ required: true }, { min: 6 }]}>
            <Input.Password />
          </Form.Item>
          <Form.Item name="role" label="Vai trò" rules={[{ required: true }]}>
            <Select options={[{ value: 'user', label: 'User' }, { value: 'admin', label: 'Admin' }]} />
          </Form.Item>
          <Form.Item name="isActive" label="Kích hoạt" valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </Modal>

      {/* Adjust Tokens Modal */}
      <Modal
        title={`Điều chỉnh token — ${tokenTarget?.username}`}
        open={tokenModalOpen}
        onCancel={() => setTokenModalOpen(false)}
        onOk={() => tokenForm.submit()}
        confirmLoading={tokenMut.isPending}
        destroyOnClose
      >
        <Form
          form={tokenForm}
          layout="vertical"
          onFinish={v => tokenMut.mutate({ id: tokenTarget!.id, amount: v.amount, description: v.description })}
        >
          <Form.Item name="amount" label="Số lượng (dương = nạp, âm = trừ)" rules={[{ required: true }]}>
            <InputNumber style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="description" label="Ghi chú" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
