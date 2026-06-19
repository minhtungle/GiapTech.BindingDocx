import { Drawer, Form, Input, Select, Divider, Button, Space } from 'antd';
import { SettingOutlined } from '@ant-design/icons';
import { useAppStore } from '../../stores/appStore';

export default function SettingsDrawer() {
  const settingsDrawerOpen = useAppStore((s) => s.settingsDrawerOpen);
  const setSettingsDrawerOpen = useAppStore((s) => s.setSettingsDrawerOpen);

  return (
    <Drawer
      title={<span><SettingOutlined /> Cài đặt</span>}
      open={settingsDrawerOpen}
      onClose={() => setSettingsDrawerOpen(false)}
      width={420}
      placement="right"
      footer={
        <Space>
          <Button onClick={() => setSettingsDrawerOpen(false)}>Đóng</Button>
          <Button type="primary">Lưu</Button>
        </Space>
      }
    >
      <Form layout="vertical" size="small">
        <Divider style={{ fontSize: 13, fontWeight: 600 }}>Thư mục</Divider>

        <Form.Item label="Template Folder">
          <Input placeholder="E:/GIAPTECH/GiapTech.BindingDocx/storage/templates" />
        </Form.Item>

        <Form.Item label="Output Folder">
          <Input placeholder="E:/GIAPTECH/GiapTech.BindingDocx/storage/output" />
        </Form.Item>

        <Divider style={{ fontSize: 13, fontWeight: 600 }}>Storage</Divider>

        <Form.Item label="Storage Provider">
          <Select
            defaultValue="local"
            options={[
              { value: 'local', label: 'Local Storage' },
              { value: 'minio', label: 'MinIO' },
            ]}
          />
        </Form.Item>

        <Divider style={{ fontSize: 13, fontWeight: 600 }}>Giao diện</Divider>

        <Form.Item label="Theme">
          <Select
            defaultValue="light"
            options={[
              { value: 'light', label: 'Light' },
              { value: 'dark', label: 'Dark' },
            ]}
          />
        </Form.Item>

        <Form.Item label="Language">
          <Select
            defaultValue="vi"
            options={[
              { value: 'vi', label: 'Tiếng Việt' },
              { value: 'en', label: 'English' },
            ]}
          />
        </Form.Item>
      </Form>
    </Drawer>
  );
}
