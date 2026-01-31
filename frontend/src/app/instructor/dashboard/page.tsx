'use client';

import { useState, useEffect } from 'react';
import Link from 'next/link';
import ProtectedRoute from '@/components/ProtectedRoute';
import { courseApi } from '@/lib/api/course';
import { Course } from '@/lib/types/course';
import { useAuthStore } from '@/lib/store/authStore';

export default function InstructorDashboard() {
    const [courses, setCourses] = useState<Course[]>([]);
    const [loading, setLoading] = useState(true);
    const { user } = useAuthStore();

    useEffect(() => {
        const fetchMyCourses = async () => {
            try {
                const data = await courseApi.getMyCourses();
                setCourses(data);
            } catch (error) {
                console.error('Failed to fetch instructor courses:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchMyCourses();
    }, []);

    const handleDelete = async (id: string) => {
        if (confirm('Bạn có chắc chắn muốn xóa khóa học này?')) {
            try {
                await courseApi.delete(id);
                setCourses(courses.filter((c) => c.id !== id));
            } catch (error) {
                alert('Xóa thất bại');
                console.error(error);
            }
        }
    };

    return (
        <ProtectedRoute allowedRoles={['Instructor', 'Admin']}>
            <div className="min-h-screen">
                {/* Glass Navbar */}
                <nav className="glass-nav sticky top-0 z-50">
                    <div className="container mx-auto px-6 py-4 flex justify-between items-center">
                        <Link href="/courses" className="text-2xl font-bold text-gradient">
                            hoccungtonhe <span className="text-[var(--text-muted)] text-sm font-normal ml-2">| Giảng viên</span>
                        </Link>
                        <div className="flex items-center gap-3">
                            <div className="text-right hidden sm:block">
                                <div className="text-sm font-semibold text-[var(--text-primary)]">{user?.fullName}</div>
                                <div className="text-xs text-[var(--text-muted)]">Giảng viên</div>
                            </div>
                            <div className="w-10 h-10 rounded-full bg-gradient-to-br from-[var(--accent-start)] to-[var(--accent-end)] flex items-center justify-center text-white font-bold">
                                {user?.fullName?.charAt(0) || 'G'}
                            </div>
                        </div>
                    </div>
                </nav>

                <div className="container mx-auto px-6 py-8">
                    {/* Header */}
                    <div className="flex justify-between items-center mb-8">
                        <div>
                            <h1 className="text-2xl font-bold text-[var(--text-primary)]">Quản lý khóa học</h1>
                            <p className="text-[var(--text-secondary)] mt-1">Tạo và quản lý các khóa học của bạn</p>
                        </div>
                        <Link href="/instructor/courses/create" className="btn-primary flex items-center gap-2">
                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                            </svg>
                            Tạo khóa học mới
                        </Link>
                    </div>

                    {/* Stats */}
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
                        <div className="glass p-5">
                            <div className="text-3xl font-bold text-gradient">{courses.length}</div>
                            <div className="text-sm text-[var(--text-secondary)]">Tổng khóa học</div>
                        </div>
                        <div className="glass p-5">
                            <div className="text-3xl font-bold text-gradient">{courses.filter(c => c.isPublished).length}</div>
                            <div className="text-sm text-[var(--text-secondary)]">Đã xuất bản</div>
                        </div>
                        <div className="glass p-5">
                            <div className="text-3xl font-bold text-gradient">{courses.filter(c => !c.isPublished).length}</div>
                            <div className="text-sm text-[var(--text-secondary)]">Bản nháp</div>
                        </div>
                    </div>

                    {/* Content */}
                    {loading ? (
                        <div className="glass text-center py-20">
                            <div className="animate-spin w-8 h-8 border-2 border-[var(--primary)] border-t-transparent rounded-full mx-auto mb-4"></div>
                            <p className="text-[var(--text-secondary)]">Đang tải...</p>
                        </div>
                    ) : courses.length === 0 ? (
                        <div className="glass text-center py-16">
                            <div className="text-6xl mb-4">📚</div>
                            <p className="text-[var(--text-secondary)] mb-4">Bạn chưa có khóa học nào.</p>
                            <Link href="/instructor/courses/create" className="btn-primary inline-block">
                                Tạo khóa học đầu tiên
                            </Link>
                        </div>
                    ) : (
                        <div className="glass overflow-hidden">
                            <div className="overflow-x-auto">
                                <table className="w-full text-left">
                                    <thead className="border-b border-[var(--glass-border)]">
                                        <tr>
                                            <th className="px-6 py-4 font-medium text-[var(--text-secondary)] text-sm">Tên khóa học</th>
                                            <th className="px-6 py-4 font-medium text-[var(--text-secondary)] text-sm">Môn</th>
                                            <th className="px-6 py-4 font-medium text-[var(--text-secondary)] text-sm">Lớp</th>
                                            <th className="px-6 py-4 font-medium text-[var(--text-secondary)] text-sm">Giá</th>
                                            <th className="px-6 py-4 font-medium text-[var(--text-secondary)] text-sm">Trạng thái</th>
                                            <th className="px-6 py-4 font-medium text-[var(--text-secondary)] text-sm text-right">Thao tác</th>
                                        </tr>
                                    </thead>
                                    <tbody className="divide-y divide-[var(--glass-border)]">
                                        {courses.map((course) => (
                                            <tr key={course.id} className="hover:bg-[var(--glass-bg-light)] transition-all-smooth">
                                                <td className="px-6 py-4">
                                                    <div className="font-semibold text-[var(--text-primary)]">{course.title}</div>
                                                    <div className="text-xs text-[var(--text-muted)] mt-1 line-clamp-1">{course.id}</div>
                                                </td>
                                                <td className="px-6 py-4 text-[var(--text-secondary)]">{course.subject}</td>
                                                <td className="px-6 py-4 text-[var(--text-secondary)]">{course.grade}</td>
                                                <td className="px-6 py-4">
                                                    <span className="text-gradient font-semibold">
                                                        {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(course.price)}
                                                    </span>
                                                </td>
                                                <td className="px-6 py-4">
                                                    <span className={`px-3 py-1 rounded-full text-xs font-medium ${course.isPublished
                                                        ? 'bg-green-500/10 text-green-600 border border-green-500/20'
                                                        : 'bg-[var(--glass-bg-light)] text-[var(--text-muted)] border border-[var(--glass-border)]'}`}>
                                                        {course.isPublished ? '✓ Xuất bản' : 'Bản nháp'}
                                                    </span>
                                                </td>
                                                <td className="px-6 py-4 text-right space-x-2">
                                                    <Link
                                                        href={`/courses/${course.id}`}
                                                        className="btn-glass !py-1.5 !px-3 text-sm inline-block"
                                                    >
                                                        Xem
                                                    </Link>
                                                    <Link
                                                        href={`/instructor/courses/${course.id}/edit`}
                                                        className="btn-primary !py-1.5 !px-3 text-sm inline-block"
                                                    >
                                                        Sửa
                                                    </Link>
                                                    <button
                                                        onClick={() => handleDelete(course.id)}
                                                        className="px-3 py-1.5 text-sm bg-red-500/10 text-red-500 border border-red-500/20 rounded-xl hover:bg-red-500/20 transition-all-smooth"
                                                    >
                                                        Xóa
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </ProtectedRoute>
    );
}
