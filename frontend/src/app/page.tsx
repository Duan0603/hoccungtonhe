'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/lib/store/authStore';
import Link from 'next/link';

export default function HomePage() {
    const router = useRouter();
    const { user } = useAuthStore();

    useEffect(() => {
        if (user) {
            if (user.role === 'Admin') {
                router.push('/admin');
            } else if (user.role === 'Instructor') {
                router.push('/instructor/dashboard');
            } else {
                router.push('/courses');
            }
        }
    }, [user, router]);

    return (
        <div className="min-h-screen relative overflow-hidden">
            {/* Animated Background Orbs */}
            <div className="absolute inset-0 overflow-hidden pointer-events-none">
                <div className="absolute -top-40 -right-40 w-80 h-80 bg-gradient-to-br from-blue-400/30 to-purple-500/30 rounded-full blur-3xl animate-float" />
                <div className="absolute top-1/2 -left-40 w-96 h-96 bg-gradient-to-br from-pink-400/20 to-blue-500/20 rounded-full blur-3xl animate-float" style={{ animationDelay: '-2s' }} />
                <div className="absolute -bottom-40 right-1/4 w-72 h-72 bg-gradient-to-br from-purple-400/25 to-pink-500/25 rounded-full blur-3xl animate-float" style={{ animationDelay: '-4s' }} />
            </div>

            {/* Navigation */}
            <nav className="glass-nav sticky top-0 z-50">
                <div className="container mx-auto px-6 py-4 flex items-center justify-between">
                    <Link href="/" className="text-2xl font-bold text-gradient">
                        hoccungtonhe
                    </Link>
                    <div className="flex items-center gap-4">
                        <Link
                            href="/auth/login"
                            className="px-5 py-2.5 text-[var(--text-primary)] hover:text-[var(--primary)] transition-all-smooth font-medium"
                        >
                            Đăng nhập
                        </Link>
                        <Link
                            href="/auth/register"
                            className="btn-primary"
                        >
                            Đăng ký miễn phí
                        </Link>
                    </div>
                </div>
            </nav>

            {/* Hero Section */}
            <section className="container mx-auto px-6 pt-20 pb-32 relative z-10">
                <div className="max-w-4xl mx-auto text-center">
                    {/* Badge */}
                    <div className="inline-flex items-center gap-2 glass-light px-4 py-2 mb-8 animate-fade-in-up">
                        <span className="w-2 h-2 bg-green-500 rounded-full animate-pulse" />
                        <span className="text-sm font-medium text-[var(--text-secondary)]">
                            Nền tảng #1 ôn thi THPT Quốc Gia
                        </span>
                    </div>

                    {/* Main Heading */}
                    <h1 className="text-5xl md:text-7xl font-bold mb-6 animate-fade-in-up" style={{ animationDelay: '0.1s' }}>
                        <span className="text-[var(--text-primary)]">Ôn thi thông minh</span>
                        <br />
                        <span className="text-gradient animate-gradient">với AI hiện đại</span>
                    </h1>

                    {/* Subtitle */}
                    <p className="text-xl text-[var(--text-secondary)] mb-10 max-w-2xl mx-auto animate-fade-in-up" style={{ animationDelay: '0.2s' }}>
                        Học tập hiệu quả với AI chấm bài tự động, video bài giảng chất lượng,
                        và đề thi thử chuẩn format THPT Quốc Gia
                    </p>

                    {/* CTA Buttons */}
                    <div className="flex flex-col sm:flex-row gap-4 justify-center animate-fade-in-up" style={{ animationDelay: '0.3s' }}>
                        <Link href="/auth/register" className="btn-primary text-lg px-8 py-4">
                            Bắt đầu học miễn phí →
                        </Link>
                        <Link href="/courses" className="btn-glass text-lg px-8 py-4">
                            Khám phá khóa học
                        </Link>
                    </div>

                    {/* Stats */}
                    <div className="flex flex-wrap justify-center gap-8 mt-16 animate-fade-in-up" style={{ animationDelay: '0.4s' }}>
                        {[
                            { value: '10K+', label: 'Học sinh' },
                            { value: '500+', label: 'Bài giảng' },
                            { value: '98%', label: 'Đỗ đại học' },
                        ].map((stat) => (
                            <div key={stat.label} className="text-center">
                                <div className="text-3xl font-bold text-gradient">{stat.value}</div>
                                <div className="text-sm text-[var(--text-muted)]">{stat.label}</div>
                            </div>
                        ))}
                    </div>
                </div>
            </section>

            {/* Features Section */}
            <section className="container mx-auto px-6 pb-32 relative z-10">
                <div className="grid md:grid-cols-3 gap-6 max-w-6xl mx-auto">
                    {/* Feature 1 */}
                    <div className="glass-card p-8 animate-fade-in-up" style={{ animationDelay: '0.5s' }}>
                        <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center mb-6 shadow-glow">
                            <svg className="w-7 h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                            </svg>
                        </div>
                        <h3 className="text-xl font-bold text-[var(--text-primary)] mb-3">
                            AI Chấm bài thông minh
                        </h3>
                        <p className="text-[var(--text-secondary)]">
                            Nhận phản hồi chi tiết từ AI, chỉ ra lỗi sai và hướng dẫn cách giải đúng trong vài giây
                        </p>
                    </div>

                    {/* Feature 2 */}
                    <div className="glass-card p-8 animate-fade-in-up" style={{ animationDelay: '0.6s' }}>
                        <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-purple-500 to-pink-500 flex items-center justify-center mb-6 shadow-glow">
                            <svg className="w-7 h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
                            </svg>
                        </div>
                        <h3 className="text-xl font-bold text-[var(--text-primary)] mb-3">
                            Video bài giảng HD
                        </h3>
                        <p className="text-[var(--text-secondary)]">
                            Học từ giảng viên giỏi với video chất lượng cao, tài liệu đầy đủ cho lớp 10, 11, 12
                        </p>
                    </div>

                    {/* Feature 3 */}
                    <div className="glass-card p-8 animate-fade-in-up" style={{ animationDelay: '0.7s' }}>
                        <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-green-500 to-teal-500 flex items-center justify-center mb-6 shadow-glow">
                            <svg className="w-7 h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                            </svg>
                        </div>
                        <h3 className="text-xl font-bold text-[var(--text-primary)] mb-3">
                            Đề thi thử THPT QG
                        </h3>
                        <p className="text-[var(--text-secondary)]">
                            Luyện tập với đề thi chuẩn format, theo dõi tiến độ và xếp hạng với bạn bè
                        </p>
                    </div>
                </div>
            </section>

            {/* Footer */}
            <footer className="glass-nav py-8 relative z-10">
                <div className="container mx-auto px-6 text-center">
                    <p className="text-[var(--text-muted)]">
                        © 2026 hoccungtonhe. Nền tảng ôn thi THPT Quốc Gia với AI.
                    </p>
                </div>
            </footer>
        </div>
    );
}
