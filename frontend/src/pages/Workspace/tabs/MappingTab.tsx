import { Empty } from 'antd';
import type { ProfileGroup } from '../../../types';

interface Props {
  group: ProfileGroup;
}

export default function MappingTab({ group }: Props) {
  return (
    <div style={{ padding: '16px 0', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <Empty
        description={
          <span style={{ color: '#8c8c8c' }}>
            Upload template để cấu hình mapping cho {group.name}.
          </span>
        }
      />
    </div>
  );
}
