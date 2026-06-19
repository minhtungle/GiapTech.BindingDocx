import { useState } from 'react';
import {
  Table, Button, Space, Modal, Form, Input, InputNumber, Switch,
  message, Tooltip, Typography, Popconfirm,
} from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminTokenService } from '../../../services/adminTokenService';
import type { TokenPackage } from '../../../types';

export default function AdminTokenPackagesPage() {
  const qc = useQueryClient();
  const [modalOpen, setModalOpen] = useState(false);
  const [editPkg, setEditPkg] = useState<TokenPackage | null>(null);
  const [form] = Form.useForm();

  const { data: packages = [], isLoading } = useQuery({
    queryKey: ['admin-token-packages'],
    queryFn: adminTokenService.getAllPackages,
  });

  const createMut = useMutation({
    mutationFn: adminTokenService.createPackage,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['admin-token-packages'] }); message.success('Tạo gói thành công'); closeModal(); },
    onError: () => message.error('Tạo thất bại'),
  });

  const updateMut = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: Parameters<typeof adminTokenService.updatePackage>[1] }) =>
      adminTokenService.updatePackage(id, payload),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['admin-token-packages'] }); message.success('Cập nhật thành công'); closeModal(); },
    onError: () => message.error('Cập nhật thất bại'),
  });

  const deleteMut = useMutation({
    mutationFn: adminTokenService.deletePackage,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['admin-token-packages'] }); message.success('Xóa thành công'); },
    onError: () => message.error('Xóa thất bại'),
  });

  const toggleMut = useMutation({
    mutationFn: adminTokenService.togglePackageActive,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-token-packages'] }),
  });

  const openCreate = () => { setEditPkg(null); form.resetFields(); form.setFieldsValue({ isActive: true, sortOrder: (packages.length + 1) }); setModalOpen(true); };
  const openEdit = (p: TokenPackage) => { setEditPkg(p); form.setFieldsValue(p); setModalOpen(true); };
  const closeModal = () => { setModalOpen(false); setEditPkg(null); form.resetFields(); };

  const onSubmit = (values: Parameters<typeof adminTokenService.createPackage>[0]) => {
    if (editPkg) {
      updateMut.mutate({ id: editPkg.id, payload: values });
    } else {
      createMut.mutate(values);
    }
  };

  const columns = [
    { title: 'Thứ tự', dataIndex: 'sortOrder', key: 'sortOrder', width: 80 },
    { title: 'Tên gói', dataIndex: 'name', key: 'name' },
    {
      title: 'Token', dataIndex: 'tokenAmount', key: 'tokenAmount',
      render: (v: number) => <Typography.Text strong>{v.toLocaleString()}</Typography.Text>,
    },
    {
      title: 'Giá/Token', dataIndex: 'pricePerToken', key: 'pricePerToken',
      render: (v: number) => `${v.toLocaleString('vi-VN')}đ`,
    },
    {
      title: 'Tổng giá', dataIndex: 'totalPrice', key: 'totalPrice',
      render: (v: number) => <Typography.Text type="success">{v.toLocaleString('vi-VN')}đ</Typography.Text>,
    },
    {
      title: 'Trạng thái', dataIndex: 'isActive', key: 'isActive',
      render: (v: boolean, r: TokenPackage) => (
        <Switch checked={v} size="small" onChange={() => toggleMut.mutate(r.id)} />
      ),
    },
    {
      title: 'Hành động', key: 'actions',
      render: (_: unknown, r: TokenPackage) => (
        <Space>
          <Tooltip title="Chỉnh sửa">
            <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(r)} />
          </Tooltip>
          <Popconfirm title="Xóa gói này?" okText="Xóa" cancelText="Hủy" onConfirm={() => deleteMut.mutate(r.id)}>
            <Tooltip title="Xóa">
              <Button size="small" danger icon={<DeleteOutlined />} />
            </Tooltip>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <Typography.Title level={4} style={{ margin: 0 }}>Gói Token</Typography.Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>Thêm gói</Button>
      </div>

      <Table columns={columns} dataSource={packages} rowKey="id" loading={isLoading} pagination={false} size="middle" />

      <Modal
        title={editPkg ? 'Chỉnh sửa gói token' : 'Thêm gói token mới'}
        open={modalOpen}
        onCancel={closeModal}
        onOk={() => form.submit()}
        confirmLoading={createMut.isPending || updateMut.isPending}
        destroyOnClose
      >
        <Form form={form} layout="vertical" onFinish={onSubmit}>
          <Form.Item name="name" label="Tên gói" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="tokenAmount" label="Số lượng token" rules={[{ required: true }]}>
            <InputNumber min={1} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="pricePerToken" label="Giá mỗi token (đ)" rules={[{ required: true }]}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="totalPrice" label="Tổng giá (đ)" rules={[{ required: true }]}>
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="sortOrder" label="Thứ tự" rules={[{ required: true }]}>
            <InputNumber min={1} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="isActive" label="Kích hoạt" valuePropName="checked">
            <Switch />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
