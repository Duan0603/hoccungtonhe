'use client';

import ProtectedRoute from '@/components/ProtectedRoute';
import Link from 'next/link';

export default function AdminDashboard() {
    return (
        <ProtectedRoute allowedRoles={['Admin']}>
            <div className="min-h-screen">
                {/* Glass Navbar */}
                <nav className="glass-nav sticky top-0 z-50">
                    <div className="container mx-auto px-6 py-4 flex justify-between items-center">
                        <Link href="/" className="text-2xl font-bold text-gradient">
                            hoccungtonhe <span className="text-[var(--text-muted)] text-sm font-normal ml-2">| Admin</span>
                        </Link>
                        <Link href="/courses" className="btn-glass !py-2 !px-4 text-sm">
                            Khóa học
                        </Link>
                    </div>
                </nav>

                <div className="container mx-auto px-6 py-8">
                    <div className="mb-8">
                        <h1 className="text-3xl font-bold text-[var(--text-primary)]">Admin Dashboard</h1>
                        <p className="text-[var(--text-secondary)] mt-2">Quản lý hệ thống hoccungtonhe</p>
                    </div>

                    {/* Stats Grid */}
                    <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
                        <div className="glass p-5 text-center">
                            <div className="text-3xl font-bold text-gradient">0</div>
                            <div className="text-sm text-[var(--text-secondary)] mt-1">Học sinh</div>
                        </div>
                        <div className="glass p-5 text-center">
                            <div className="text-3xl font-bold text-gradient">0</div>
                            <div className="text-sm text-[var(--text-secondary)] mt-1">Giảng viên</div>
                        </div>
                        <div className="glass p-5 text-center">
                            <div className="text-3xl font-bold text-gradient">0</div>
                            <div className="text-sm text-[var(--text-secondary)] mt-1">Khóa học</div>
                        </div>
                        <div className="glass p-5 text-center">
                            <div className="text-3xl font-bold text-gradient">0₫</div>
                            <div className="text-sm text-[var(--text-secondary)] mt-1">Doanh thu</div>
                        </div>
                    </div>

                    {/* Coming Soon Notice */}
                    <div className="glass p-8 text-center">
                        <div className="text-6xl mb-4">🚧</div>
                        <h2 className="text-xl font-bold text-[var(--text-primary)] mb-2">Đang phát triển</h2>
                        <p className="text-[var(--text-secondary)] mb-6">
                            Các chức năng Admin sẽ được triển khai ở Milestone 6
                        </p>

                        <div className="glass-light p-6 max-w-md mx-auto text-left">
                            <h3 className="font-semibold text-[var(--text-primary)] mb-3">Chức năng sắp có:</h3>
                            <ul className="space-y-2 text-sm text-[var(--text-secondary)]">
                                <li className="flex items-center gap-2">
                                    <span className="w-2 h-2 rounded-full bg-[var(--primary)]"></span>
                                    Quản lý người dùng (sinh viên, giảng viên)
                                </li>
                                <li className="flex items-center gap-2">
                                    <span className="w-2 h-2 rounded-full bg-[var(--primary)]"></span>
                                    Duyệt/từ chối giảng viên mới
                                </li>
                                <li className="flex items-center gap-2">
                                    <span className="w-2 h-2 rounded-full bg-[var(--primary)]"></span>
                                    Quản lý khóa học toàn hệ thống
                                </li>
                                <li className="flex items-center gap-2">
                                    <span className="w-2 h-2 rounded-full bg-[var(--primary)]"></span>
                                    Thống kê doanh thu PayOS
                                </li>
                            </ul>
                        </div>

                        <div className="flex justify-center gap-3 mt-6">
                            <Link href="/courses" className="btn-glass">
                                Xem khóa học
                            </Link>
                            <Link href="/instructor/dashboard" className="btn-primary">
                                Quản lý khóa học
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </ProtectedRoute>
    );
}
