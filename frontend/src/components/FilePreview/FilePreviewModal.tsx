import { Modal, Spin, Alert } from 'antd';
import { FileWordOutlined, FileExcelOutlined } from '@ant-design/icons';
import { useState, useEffect } from 'react';
import { workspaceService } from '../../services/workspaceService';
import './FilePreviewModal.css';

interface Props {
  groupId: string;
  fileName: string;
  onClose: () => void;
}

export default function FilePreviewModal({ groupId, fileName, onClose }: Props) {
  const [loading, setLoading] = useState(true);
  const [htmlContent, setHtmlContent] = useState('');
  const [error, setError] = useState('');

  const ext = fileName.split('.').pop()?.toLowerCase();
  const isDocx = ext === 'docx';
  const isXlsx = ext === 'xlsx';

  const displayName = fileName.replace(/_/g, ' ').replace(/\.[^.]+$/, '');

  useEffect(() => {
    setLoading(true);
    setHtmlContent('');
    setError('');

    (async () => {
      try {
        const blob = await workspaceService.getFilePreview(groupId, fileName);
        const arrayBuffer = await blob.arrayBuffer();

        if (isDocx) {
          const mammoth = await import('mammoth');
          const result = await mammoth.convertToHtml({ arrayBuffer });
          setHtmlContent(result.value);
        } else if (isXlsx) {
          const XLSX = await import('xlsx');
          const wb = XLSX.read(new Uint8Array(arrayBuffer), { type: 'array' });
          let html = '';
          for (const sheetName of wb.SheetNames) {
            const ws = wb.Sheets[sheetName];
            html += `<div class="sheet-label">${sheetName}</div>`;
            html += `<div class="sheet-table-wrap">${XLSX.utils.sheet_to_html(ws)}</div>`;
          }
          setHtmlContent(html);
        }
      } catch {
        setError('Không thể tải nội dung file. Vui lòng thử lại.');
      } finally {
        setLoading(false);
      }
    })();
  }, [groupId, fileName, isDocx, isXlsx]);

  const titleIcon = isDocx
    ? <FileWordOutlined style={{ color: '#2b579a', marginRight: 8 }} />
    : <FileExcelOutlined style={{ color: '#217346', marginRight: 8 }} />;

  return (
    <Modal
      title={<span>{titleIcon}{displayName}</span>}
      open
      onCancel={onClose}
      footer={null}
      width="80vw"
      style={{ top: 20, maxWidth: 1100 }}
      styles={{
        body: {
          maxHeight: 'calc(90vh - 110px)',
          overflow: 'auto',
          padding: '16px 20px',
        },
      }}
      destroyOnHidden
    >
      {loading && (
        <div style={{ textAlign: 'center', padding: 80, minHeight: 300, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
          <Spin size="large" />
          <div style={{ marginTop: 16, color: '#8c8c8c', fontSize: 13 }}>Đang tải nội dung file...</div>
        </div>
      )}
      {!loading && error && (
        <Alert type="error" message={error} showIcon />
      )}
      {!loading && !error && htmlContent && (
        <div
          className={isDocx ? 'docx-preview' : 'xlsx-preview'}
          dangerouslySetInnerHTML={{ __html: htmlContent }}
        />
      )}
    </Modal>
  );
}
