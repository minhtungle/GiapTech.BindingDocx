import { Input, Empty } from 'antd';
import { SearchOutlined } from '@ant-design/icons';
import { useState } from 'react';
import type { ProfileGroup } from '../../../types';

interface Props {
  group: ProfileGroup;
}

export default function DataTab({ group }: Props) {
  const [search, setSearch] = useState('');

  return (
    <div style={{ padding: '16px 0' }}>
      <div style={{ marginBottom: 12, display: 'flex', gap: 8 }}>
        <Input
          prefix={<SearchOutlined />}
          placeholder="Tìm kiếm hồ sơ..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          style={{ width: 300 }}
          allowClear
        />
      </div>
      <Empty
        description={
          <span style={{ color: '#8c8c8c' }}>
            Chưa có dữ liệu. Click <strong>Import</strong> để nhập dữ liệu cho {group.name}.
          </span>
        }
      />
    </div>
  );
}
