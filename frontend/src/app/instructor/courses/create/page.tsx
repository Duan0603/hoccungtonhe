'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import ProtectedRoute from '@/components/ProtectedRoute';
import { courseApi } from '@/lib/api/course';
import Link from 'next/link';

export default function CreateCoursePage() {
    const router = useRouter();
    const [loading, setLoading] = useState(false);
    const [formData, setFormData] = useState({
        title: '',
        description: '',
        subject: 'Toán',
        grade: 12,
        price: 0
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        try {
            await courseApi.create(formData);
            router.push('/instructor/dashboard');
        } catch (error) {
            alert('Tạo khóa học thất bại');
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <ProtectedRoute allowedRoles={['Instructor', 'Admin']}>
            <div className="min-h-screen flex items-center justify-center p-4">
                {/* Background Orbs */}
                <div className="fixed inset-0 overflow-hidden pointer-events-none">
                    <div className="absolute top-1/4 -left-20 w-80 h-80 bg-gradient-to-br from-blue-400/20 to-cyan-400/20 rounded-full blur-3xl animate-float" />
                    <div className="absolute bottom-1/4 -right-20 w-96 h-96 bg-gradient-to-br from-indigo-400/20 to-purple-400/20 rounded-full blur-3xl animate-float-delayed" />
                </div>

                <div className="glass p-8 w-full max-w-2xl relative z-10 animate-fade-in-up">
                    <div className="flex justify-between items-center mb-6 border-b border-[var(--glass-border)] pb-4">
                        <div>
                            <h1 className="text-2xl font-bold text-[var(--text-primary)]">Tạo khóa học mới</h1>
                            <p className="text-sm text-[var(--text-muted)] mt-1">Điền thông tin để bắt đầu</p>
                        </div>
                        <Link href="/instructor/dashboard" className="p-2 hover:bg-[var(--glass-bg-light)] rounded-xl transition-all-smooth">
                            <svg className="w-6 h-6 text-[var(--text-muted)]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                            </svg>
                        </Link>
                    </div>

                    <form onSubmit={handleSubmit} className="space-y-5">
                        <div>
                            <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Tên khóa học</label>
                            <input
                                type="text"
                                required
                                className="input-glass"
                                value={formData.title}
                                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                                placeholder="Ví dụ: Toán 12 - Chuyên đề Hàm số"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Mô tả</label>
                            <textarea
                                rows={4}
                                className="input-glass min-h-[120px]"
                                value={formData.description}
                                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                placeholder="Giới thiệu nội dung khóa học..."
                            />
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Môn học</label>
                                <select
                                    className="input-glass"
                                    value={formData.subject}
                                    onChange={(e) => setFormData({ ...formData, subject: e.target.value })}
                                >
                                    <option value="Toán">Toán</option>
                                    <option value="Lý">Vật Lý</option>
                                    <option value="Hóa">Hóa Học</option>
                                    <option value="Anh">Tiếng Anh</option>
                                    <option value="Văn">Ngữ Văn</option>
                                    <option value="Sinh">Sinh Học</option>
                                </select>
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Lớp</label>
                                <select
                                    className="input-glass"
                                    value={formData.grade}
                                    onChange={(e) => setFormData({ ...formData, grade: Number(e.target.value) })}
                                >
                                    <option value={12}>Lớp 12</option>
                                    <option value={11}>Lớp 11</option>
                                    <option value={10}>Lớp 10</option>
                                </select>
                            </div>
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Giá khóa học (VNĐ)</label>
                            <input
                                type="number"
                                min="0"
                                step="1000"
                                className="input-glass"
                                value={formData.price}
                                onChange={(e) => setFormData({ ...formData, price: Number(e.target.value) })}
                            />
                            <p className="text-xs text-[var(--text-muted)] mt-2">Nhập 0 để tạo khóa học miễn phí</p>
                        </div>

                        <div className="flex justify-end gap-3 pt-4 border-t border-[var(--glass-border)]">
                            <Link href="/instructor/dashboard" className="btn-glass">
                                Hủy bỏ
                            </Link>
                            <button
                                type="submit"
                                disabled={loading}
                                className="btn-primary disabled:opacity-50"
                            >
                                {loading ? 'Đang tạo...' : 'Tạo khóa học'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </ProtectedRoute>
    );
}
