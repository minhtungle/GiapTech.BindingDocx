import { useState } from 'react';
import { Select, Button, Spin, Empty, Typography, Space, Tag, Alert } from 'antd';
import { EyeOutlined, FileWordOutlined, FileExcelOutlined, ReloadOutlined } from '@ant-design/icons';
import { useAppStore } from '../../../stores/appStore';
import { workspaceService } from '../../../services/workspaceService';

const { Text } = Typography;

interface RenderedContent {
  html: string;
  type: 'docx' | 'xlsx';
}

export default function PreviewTab() {
  const { groupKeys, formData, syncEnabled, selectedGroupId, getMergedSingleFields } = useAppStore();
  const [selectedFile, setSelectedFile] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [rendered, setRendered] = useState<RenderedContent | null>(null);
  const [error, setError] = useState<string | null>(null);

  if (!groupKeys || !selectedGroupId) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
        <Empty description="Chọn nhóm và nhập dữ liệu để xem preview" />
      </div>
    );
  }

  const fileOptions = groupKeys.files.map((f) => ({
    value: f.fileName,
    label: (
      <Space size={6}>
        {f.fileType === 'docx'
          ? <FileWordOutlined style={{ color: '#2b579a' }} />
          : <FileExcelOutlined style={{ color: '#217346' }} />}
        <span>{f.displayName}</span>
        <Tag color={f.fileType === 'docx' ? 'blue' : 'green'} style={{ fontSize: 10, padding: '0 4px' }}>
          {f.fileType.toUpperCase()}
        </Tag>
      </Space>
    ),
  }));

  const handlePreview = async (fileName?: string) => {
    const target = fileName ?? selectedFile;
    if (!target) return;

    setLoading(true);
    setRendered(null);
    setError(null);

    try {
      const fileInfo = groupKeys.files.find((f) => f.fileName === target);
      if (!fileInfo) throw new Error('File không tồn tại');

      // Build singleFields for this file
      let singleFields: Record<string, string>;
      if (syncEnabled) {
        singleFields = getMergedSingleFields();
      } else {
        singleFields = formData.singleFieldsByFile[target] ?? {};
      }

      // Build tableData using tableKey → rows
      const tableData: Record<string, Record<string, string>[]> = {};
      const fileTables = groupKeys.tableFiles.filter((t) => t.fileName === target);
      for (const t of fileTables) {
        if (formData.tableData[t.tableKey]) {
          tableData[t.tableKey] = formData.tableData[t.tableKey];
        }
      }

      const blob = await workspaceService.previewRendered(selectedGroupId, target, {
        singleFields,
        tableData,
      });

      const ext = target.split('.').pop()?.toLowerCase();
      const arrayBuffer = await blob.arrayBuffer();

      if (ext === 'docx') {
        const mammoth = (await import('mammoth')).default;
        const result = await mammoth.convertToHtml({ arrayBuffer });
        setRendered({ html: result.value, type: 'docx' });
      } else if (ext === 'xlsx') {
        const XLSX = await import('xlsx');
        const wb = XLSX.read(new Uint8Array(arrayBuffer), { type: 'array' });
        let html = '';
        for (const sheetName of wb.SheetNames) {
          const ws = wb.Sheets[sheetName];
          const sheetHtml = XLSX.utils.sheet_to_html(ws, { id: `sheet-${sheetName}` });
          html += `<div class="xlsx-sheet-label">${sheetName}</div>${sheetHtml}`;
        }
        setRendered({ html, type: 'xlsx' });
      }
    } catch {
      setError('Không thể render file. Vui lòng kiểm tra dữ liệu.');
    } finally {
      setLoading(false);
    }
  };

  const handleSelectFile = (value: string) => {
    setSelectedFile(value);
    setRendered(null);
    setError(null);
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%', overflow: 'hidden' }}>
      {/* Toolbar */}
      <div style={{
        padding: '10px 16px',
        borderBottom: '1px solid #f0f0f0',
        display: 'flex',
        alignItems: 'center',
        gap: 12,
        flexShrink: 0,
      }}>
        <Text style={{ fontSize: 13, whiteSpace: 'nowrap' }}>Chọn file:</Text>
        <Select
          style={{ flex: 1, maxWidth: 360 }}
          placeholder="Chọn file để xem preview..."
          value={selectedFile ?? undefined}
          onChange={handleSelectFile}
          options={fileOptions}
          optionRender={(opt) => opt.label}
        />
        <Button
          type="primary"
          icon={loading ? <Spin size="small" /> : <EyeOutlined />}
          onClick={() => handlePreview()}
          disabled={!selectedFile || loading}
          loading={loading}
        >
          Xem preview
        </Button>
        {rendered && (
          <Button
            icon={<ReloadOutlined />}
            onClick={() => handlePreview()}
            disabled={loading}
          >
            Làm mới
          </Button>
        )}
      </div>

      {/* Preview area */}
      <div style={{ flex: 1, overflow: 'auto', padding: '16px' }}>
        {loading && (
          <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
            <Spin size="large" tip="Đang render file..." />
          </div>
        )}

        {!loading && error && (
          <Alert type="error" message={error} showIcon />
        )}

        {!loading && !error && !rendered && !selectedFile && (
          <Empty description="Chọn file bên trên rồi nhấn Xem preview" />
        )}

        {!loading && !error && !rendered && selectedFile && (
          <Empty
            description={
              <span>
                File <Text code>{selectedFile}</Text> đã chọn — nhấn <b>Xem preview</b> để render
              </span>
            }
          />
        )}

        {!loading && !error && rendered && (
          rendered.type === 'docx' ? (
            <div
              className="docx-preview"
              style={{
                background: '#fff',
                padding: '32px 48px',
                maxWidth: 900,
                margin: '0 auto',
                boxShadow: '0 1px 4px rgba(0,0,0,0.12)',
                borderRadius: 4,
                fontSize: 14,
                lineHeight: 1.7,
              }}
              dangerouslySetInnerHTML={{ __html: rendered.html }}
            />
          ) : (
            <div
              className="xlsx-preview"
              style={{ overflowX: 'auto' }}
              dangerouslySetInnerHTML={{ __html: rendered.html }}
            />
          )
        )}
      </div>

      <style>{`
        .xlsx-sheet-label {
          font-weight: 600;
          font-size: 12px;
          color: #217346;
          padding: 8px 4px 4px;
          border-top: 2px solid #217346;
          margin-top: 16px;
        }
        .xlsx-sheet-label:first-child { margin-top: 0; border-top: none; }
        table#sheet-\\3 { border-collapse: collapse; font-size: 12px; }
        [id^="sheet-"] { border-collapse: collapse; font-size: 12px; }
        [id^="sheet-"] td, [id^="sheet-"] th {
          border: 1px solid #d9d9d9;
          padding: 4px 8px;
          white-space: pre-wrap;
        }
        [id^="sheet-"] tr:first-child td {
          background: #f5f5f5;
          font-weight: 600;
        }
        .docx-preview p { margin: 0.3em 0; }
        .docx-preview table { border-collapse: collapse; width: 100%; }
        .docx-preview td, .docx-preview th { border: 1px solid #d9d9d9; padding: 4px 8px; }
      `}</style>
    </div>
  );
}
