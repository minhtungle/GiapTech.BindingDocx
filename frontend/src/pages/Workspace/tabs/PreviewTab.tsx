import { Empty } from 'antd';
import type { ProfileGroup } from '../../../types';

interface Props {
  group: ProfileGroup;
}

export default function PreviewTab({ group }: Props) {
  return (
    <div style={{ padding: '16px 0', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <Empty
        description={
          <span style={{ color: '#8c8c8c' }}>
            Chọn một hồ sơ để xem preview tài liệu của {group.name}.
          </span>
        }
      />
    </div>
  );
}
