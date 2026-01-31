'use client';

import { useState } from 'react';
import { lessonApi } from '@/lib/api/lesson';
import { apiClient } from '@/lib/api/client';
import { CreateLessonRequest } from '@/lib/types/lesson';

interface AddLessonFormProps {
    courseId: string;
    onSuccess: () => void;
    onCancel: () => void;
}

export default function AddLessonForm({ courseId, onSuccess, onCancel }: AddLessonFormProps) {
    const [loading, setLoading] = useState(false);
    const [uploading, setUploading] = useState(false);
    const [formData, setFormData] = useState<CreateLessonRequest>({
        courseId,
        title: '',
        videoUrl: '',
        documentUrl: '',
        duration: 0,
        orderIndex: 0
    });

    const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>, type: 'video' | 'doc') => {
        const file = e.target.files?.[0];
        if (!file) return;

        setUploading(true);
        const uploadFormData = new FormData();
        uploadFormData.append('file', file);

        try {
            const response = await apiClient.post<{ url: string; relativeUrl: string }>('/api/upload', uploadFormData, {
                headers: { 'Content-Type': 'multipart/form-data' },
            });

            const relativePath = response.data.relativeUrl;

            if (type === 'video') {
                setFormData(prev => ({ ...prev, videoUrl: relativePath }));
            } else {
                setFormData(prev => ({ ...prev, documentUrl: relativePath }));
            }
        } catch (error) {
            console.error('Upload failed:', error);
            alert('Upload thất bại. Vui lòng thử lại.');
        } finally {
            setUploading(false);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        try {
            await lessonApi.create(formData);
            onSuccess();
        } catch (error) {
            console.error('Create lesson failed:', error);
            alert('Tạo bài học thất bại.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="glass p-6">
            <h3 className="text-lg font-bold text-[var(--text-primary)] mb-4 flex items-center gap-2">
                <span className="w-8 h-8 rounded-lg bg-gradient-to-br from-[var(--accent-start)] to-[var(--accent-end)] flex items-center justify-center text-white text-sm">+</span>
                Thêm bài học mới
            </h3>
            <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                    <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Tiêu đề bài học</label>
                    <input
                        type="text"
                        required
                        className="input-glass"
                        value={formData.title}
                        onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                        placeholder="Nhập tên bài học..."
                    />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Video</label>
                        <div className="flex gap-2">
                            <input
                                type="text"
                                className="input-glass flex-1 text-sm"
                                value={formData.videoUrl || ''}
                                onChange={(e) => setFormData({ ...formData, videoUrl: e.target.value })}
                                placeholder="URL hoặc upload..."
                            />
                            <label className="btn-glass !px-3 cursor-pointer flex items-center">
                                📂
                                <input type="file" className="hidden" accept="video/*" onChange={(e) => handleFileUpload(e, 'video')} disabled={uploading} />
                            </label>
                        </div>
                        {formData.videoUrl && (
                            <p className="text-xs text-green-500 mt-1">✓ Video đã được thêm</p>
                        )}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Tài liệu</label>
                        <div className="flex gap-2">
                            <input
                                type="text"
                                className="input-glass flex-1 text-sm"
                                value={formData.documentUrl || ''}
                                onChange={(e) => setFormData({ ...formData, documentUrl: e.target.value })}
                                placeholder="URL hoặc upload..."
                            />
                            <label className="btn-glass !px-3 cursor-pointer flex items-center">
                                📎
                                <input type="file" className="hidden" accept=".pdf,.doc,.docx" onChange={(e) => handleFileUpload(e, 'doc')} disabled={uploading} />
                            </label>
                        </div>
                        {formData.documentUrl && (
                            <p className="text-xs text-green-500 mt-1">✓ Tài liệu đã được thêm</p>
                        )}
                    </div>
                </div>

                {uploading && (
                    <div className="glass-light p-3 flex items-center gap-2">
                        <div className="animate-spin w-4 h-4 border-2 border-[var(--primary)] border-t-transparent rounded-full"></div>
                        <span className="text-sm text-[var(--primary)]">Đang upload...</span>
                    </div>
                )}

                <div className="flex justify-end gap-3 pt-4 border-t border-[var(--glass-border)]">
                    <button
                        type="button"
                        onClick={onCancel}
                        className="btn-glass"
                    >
                        Hủy
                    </button>
                    <button
                        type="submit"
                        disabled={loading || uploading}
                        className="btn-primary disabled:opacity-50"
                    >
                        {loading ? 'Đang lưu...' : 'Lưu bài học'}
                    </button>
                </div>
            </form>
        </div>
    );
}
