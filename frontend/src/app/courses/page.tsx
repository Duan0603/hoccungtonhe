'use client';

import { useState, useEffect } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import Link from 'next/link';
import ProtectedRoute from '@/components/ProtectedRoute';
import { useAuthStore } from '@/lib/store/authStore';
import CourseCard from '@/components/CourseCard';
import CourseFilter from '@/components/CourseFilter';
import { courseApi } from '@/lib/api/course';
import { Course } from '@/lib/types/course';

export default function CoursesPage() {
    const { user, logout } = useAuthStore();
    const router = useRouter();
    const searchParams = useSearchParams();

    const filter = {
        search: searchParams.get('search') || undefined,
        subject: searchParams.get('subject') || undefined,
        grade: searchParams.get('grade') ? Number(searchParams.get('grade')) : undefined,
        minPrice: searchParams.get('minPrice') ? Number(searchParams.get('minPrice')) : undefined,
        maxPrice: searchParams.get('maxPrice') ? Number(searchParams.get('maxPrice')) : undefined,
        page: searchParams.get('page') ? Number(searchParams.get('page')) : 1,
        pageSize: 9
    };

    const [courses, setCourses] = useState<Course[]>([]);
    const [totalCount, setTotalCount] = useState(0);
    const [totalPages, setTotalPages] = useState(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCourses = async () => {
            setLoading(true);
            try {
                const data = await courseApi.getAll(filter);
                setCourses(data.data);
                setTotalCount(data.totalCount);
                setTotalPages(data.totalPages);
            } catch (error) {
                console.error('Failed to fetch courses:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchCourses();
    }, [searchParams]);

    const handleLogout = async () => {
        const { refreshToken } = useAuthStore.getState();
        if (refreshToken) {
            try {
                await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/logout`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ refreshToken }),
                });
            } catch (error) {
                console.error('Logout error:', error);
            }
        }
        logout();
        router.push('/auth/login');
    };

    const handlePageChange = (newPage: number) => {
        if (newPage > 0 && newPage <= totalPages) {
            const params = new URLSearchParams(searchParams.toString());
            params.set('page', newPage.toString());
            router.push(`/courses?${params.toString()}`);
        }
    };

    return (
        <ProtectedRoute allowedRoles={['Student', 'Instructor', 'Admin']}>
            <div className="min-h-screen flex flex-col">
                {/* Glass Navbar */}
                <nav className="glass-nav sticky top-0 z-50">
                    <div className="container mx-auto px-6 py-4 flex justify-between items-center">
                        <Link href="/" className="text-2xl font-bold text-gradient">
                            hoccungtonhe
                        </Link>

                        <div className="flex items-center gap-6">
                            {user?.role === 'Instructor' && (
                                <Link href="/instructor/dashboard" className="text-[var(--text-secondary)] hover:text-[var(--primary)] font-medium transition-all-smooth">
                                    Quản lý khóa học
                                </Link>
                            )}

                            <div className="flex items-center gap-3 pl-6 border-l border-[var(--glass-border)]">
                                <div className="text-right hidden sm:block">
                                    <div className="text-sm font-semibold text-[var(--text-primary)]">{user?.fullName}</div>
                                    <div className="text-xs text-[var(--text-muted)]">{user?.role}</div>
                                </div>
                                <button
                                    onClick={handleLogout}
                                    className="btn-glass !py-2 !px-4 text-sm"
                                >
                                    Đăng xuất
                                </button>
                            </div>
                        </div>
                    </div>
                </nav>

                {/* Main Content */}
                <main className="container mx-auto px-6 py-8 flex-grow">
                    <div className="flex flex-col lg:flex-row gap-8">
                        {/* Sidebar Filter */}
                        <aside className="w-full lg:w-72 flex-shrink-0">
                            <div className="glass p-6 sticky top-24">
                                <CourseFilter />
                            </div>
                        </aside>

                        {/* Course Grid */}
                        <div className="flex-1">
                            {/* Header */}
                            <div className="flex justify-between items-center mb-6">
                                <h2 className="text-2xl font-bold text-[var(--text-primary)]">
                                    Khóa học
                                </h2>
                                <span className="glass-light px-4 py-2 text-sm text-[var(--text-secondary)]">
                                    <strong className="text-[var(--primary)]">{totalCount}</strong> kết quả
                                </span>
                            </div>

                            {/* Loading State */}
                            {loading ? (
                                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                                    {[1, 2, 3, 4, 5, 6].map((n) => (
                                        <div key={n} className="glass p-4 h-80 animate-pulse">
                                            <div className="bg-[var(--glass-border)] h-40 rounded-xl mb-4"></div>
                                            <div className="bg-[var(--glass-border)] h-6 w-3/4 rounded-lg mb-2"></div>
                                            <div className="bg-[var(--glass-border)] h-4 w-full rounded-lg mb-4"></div>
                                            <div className="bg-[var(--glass-border)] h-8 w-20 rounded-lg"></div>
                                        </div>
                                    ))}
                                </div>
                            ) : courses.length === 0 ? (
                                /* Empty State */
                                <div className="glass text-center py-20">
                                    <div className="text-6xl mb-4">📚</div>
                                    <h3 className="text-xl font-bold text-[var(--text-primary)] mb-2">Không tìm thấy khóa học</h3>
                                    <p className="text-[var(--text-secondary)] mb-6">Thử thay đổi bộ lọc hoặc tìm kiếm từ khóa khác.</p>
                                    <button
                                        onClick={() => router.push('/courses')}
                                        className="btn-primary"
                                    >
                                        Xóa bộ lọc
                                    </button>
                                </div>
                            ) : (
                                /* Grid */
                                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                                    {courses.map((course) => (
                                        <CourseCard key={course.id} course={course} />
                                    ))}
                                </div>
                            )}

                            {/* Pagination */}
                            {!loading && totalPages > 1 && (
                                <div className="flex justify-center mt-10 gap-2">
                                    <button
                                        onClick={() => handlePageChange(filter.page - 1)}
                                        disabled={filter.page === 1}
                                        className="btn-glass !py-2 disabled:opacity-50 disabled:cursor-not-allowed"
                                    >
                                        ← Trước
                                    </button>
                                    <span className="glass-light px-6 py-2 text-[var(--text-secondary)] font-medium">
                                        {filter.page} / {totalPages}
                                    </span>
                                    <button
                                        onClick={() => handlePageChange(filter.page + 1)}
                                        disabled={filter.page === totalPages}
                                        className="btn-glass !py-2 disabled:opacity-50 disabled:cursor-not-allowed"
                                    >
                                        Sau →
                                    </button>
                                </div>
                            )}
                        </div>
                    </div>
                </main>
            </div>
        </ProtectedRoute>
    );
}
