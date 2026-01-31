'use client';

import { useState, useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import ProtectedRoute from '@/components/ProtectedRoute';
import { courseApi } from '@/lib/api/course';
import Link from 'next/link';
import { UpdateCourseRequest } from '@/lib/types/course';

export default function EditCoursePage() {
    const router = useRouter();
    const params = useParams();
    const id = params.id as string;

    const [loading, setLoading] = useState(false);
    const [fetching, setFetching] = useState(true);
    const [formData, setFormData] = useState<UpdateCourseRequest>({
        title: '',
        description: '',
        subject: 'Toán',
        grade: 12,
        price: 0,
        isPublished: false
    });

    useEffect(() => {
        const fetchCourse = async () => {
            try {
                const data = await courseApi.getById(id);
                setFormData({
                    title: data.title,
                    description: data.description || '',
                    subject: data.subject,
                    grade: data.grade,
                    price: data.price,
                    isPublished: data.isPublished
                });
            } catch (error) {
                console.error('Failed to fetch course', error);
                alert('Không tìm thấy khóa học');
                router.push('/instructor/dashboard');
            } finally {
                setFetching(false);
            }
        };
        if (id) fetchCourse();
    }, [id, router]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        try {
            await courseApi.update(id, formData);
            router.push('/instructor/dashboard');
        } catch (error) {
            alert('Cập nhật khóa học thất bại');
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    if (fetching) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="glass p-8 text-center">
                    <div className="animate-spin w-10 h-10 border-3 border-[var(--primary)] border-t-transparent rounded-full mx-auto mb-4"></div>
                    <p className="text-[var(--text-secondary)]">Đang tải...</p>
                </div>
            </div>
        );
    }

    return (
        <ProtectedRoute allowedRoles={['Instructor', 'Admin']}>
            <div className="min-h-screen flex items-center justify-center p-4">
                {/* Background Orbs */}
                <div className="fixed inset-0 overflow-hidden pointer-events-none">
                    <div className="absolute top-1/4 -left-20 w-80 h-80 bg-gradient-to-br from-green-400/20 to-emerald-400/20 rounded-full blur-3xl animate-float" />
                    <div className="absolute bottom-1/4 -right-20 w-96 h-96 bg-gradient-to-br from-teal-400/20 to-cyan-400/20 rounded-full blur-3xl animate-float-delayed" />
                </div>

                <div className="glass p-8 w-full max-w-2xl relative z-10 animate-fade-in-up">
                    <div className="flex justify-between items-center mb-6 border-b border-[var(--glass-border)] pb-4">
                        <div>
                            <h1 className="text-2xl font-bold text-[var(--text-primary)]">Chỉnh sửa khóa học</h1>
                            <p className="text-sm text-[var(--text-muted)] mt-1">Cập nhật thông tin khóa học</p>
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
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-[var(--text-secondary)] mb-2">Mô tả</label>
                            <textarea
                                rows={4}
                                className="input-glass min-h-[120px]"
                                value={formData.description}
                                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
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
                        </div>

                        {/* Publish Toggle */}
                        <div className="glass-light p-4 flex items-center justify-between">
                            <div>
                                <div className="font-medium text-[var(--text-primary)]">Xuất bản khóa học</div>
                                <div className="text-sm text-[var(--text-muted)]">Khóa học sẽ hiển thị công khai</div>
                            </div>
                            <button
                                type="button"
                                onClick={() => setFormData({ ...formData, isPublished: !formData.isPublished })}
                                className={`relative w-12 h-6 rounded-full transition-all-smooth ${formData.isPublished
                                    ? 'bg-gradient-to-r from-[var(--accent-start)] to-[var(--accent-end)]'
                                    : 'bg-[var(--glass-border)]'}`}
                            >
                                <div className={`absolute top-1 w-4 h-4 rounded-full bg-white shadow transition-all-smooth ${formData.isPublished ? 'left-7' : 'left-1'}`} />
                            </button>
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
                                {loading ? 'Đang lưu...' : 'Lưu thay đổi'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </ProtectedRoute>
    );
}
