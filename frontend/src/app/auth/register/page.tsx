'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuthStore } from '@/lib/store/authStore';
import { authApi } from '@/lib/api/auth';

export default function RegisterPage() {
    const router = useRouter();
    const { user, setAuth, setLoading } = useAuthStore();

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

    const [formData, setFormData] = useState({
        email: '',
        password: '',
        fullName: '',
        grade: 12,
        school: '',
    });
    const [error, setError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsSubmitting(true);
        setLoading(true);

        if (formData.password.length < 6) {
            setError('Mật khẩu phải có ít nhất 6 ký tự');
            setIsSubmitting(false);
            setLoading(false);
            return;
        }

        try {
            const response = await authApi.register(formData);
            setAuth(response.user, response.accessToken, response.refreshToken);
            router.push('/courses');
        } catch (err: any) {
            setError(err.response?.data?.message || 'Đăng ký thất bại. Vui lòng thử lại.');
        } finally {
            setIsSubmitting(false);
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center p-4 relative overflow-hidden">
            {/* Background Orbs */}
            <div className="absolute inset-0 overflow-hidden pointer-events-none">
                <div className="absolute -top-40 -right-40 w-80 h-80 bg-gradient-to-br from-purple-400/30 to-pink-500/30 rounded-full blur-3xl animate-float" />
                <div className="absolute -bottom-40 -left-40 w-96 h-96 bg-gradient-to-br from-blue-400/20 to-purple-500/20 rounded-full blur-3xl animate-float" style={{ animationDelay: '-3s' }} />
            </div>

            {/* Register Card */}
            <div className="glass w-full max-w-md p-8 relative z-10 animate-fade-in-up">
                {/* Logo */}
                <div className="text-center mb-8">
                    <Link href="/" className="text-3xl font-bold text-gradient inline-block mb-2">
                        hoccungtonhe
                    </Link>
                    <h1 className="text-xl font-semibold text-[var(--text-primary)]">
                        Tạo tài khoản
                    </h1>
                    <p className="text-sm text-[var(--text-secondary)] mt-1">
                        Bắt đầu hành trình học tập với AI
                    </p>
                </div>

                {/* Error Message */}
                {error && (
                    <div className="mb-6 p-4 rounded-xl bg-red-500/10 border border-red-500/20">
                        <p className="text-red-500 text-sm text-center">{error}</p>
                    </div>
                )}

                {/* Register Form */}
                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label htmlFor="fullName" className="block text-sm font-medium text-[var(--text-primary)] mb-2">
                            Họ và tên <span className="text-red-400">*</span>
                        </label>
                        <input
                            id="fullName"
                            type="text"
                            required
                            value={formData.fullName}
                            onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                            className="input-glass"
                            placeholder="Nguyễn Văn A"
                        />
                    </div>

                    <div>
                        <label htmlFor="email" className="block text-sm font-medium text-[var(--text-primary)] mb-2">
                            Email <span className="text-red-400">*</span>
                        </label>
                        <input
                            id="email"
                            type="email"
                            required
                            value={formData.email}
                            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                            className="input-glass"
                            placeholder="email@example.com"
                        />
                    </div>

                    <div>
                        <label htmlFor="password" className="block text-sm font-medium text-[var(--text-primary)] mb-2">
                            Mật khẩu <span className="text-red-400">*</span>
                        </label>
                        <input
                            id="password"
                            type="password"
                            required
                            minLength={6}
                            value={formData.password}
                            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                            className="input-glass"
                            placeholder="Tối thiểu 6 ký tự"
                        />
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label htmlFor="grade" className="block text-sm font-medium text-[var(--text-primary)] mb-2">
                                Lớp
                            </label>
                            <select
                                id="grade"
                                value={formData.grade}
                                onChange={(e) => setFormData({ ...formData, grade: parseInt(e.target.value) })}
                                className="input-glass"
                            >
                                <option value={10}>Lớp 10</option>
                                <option value={11}>Lớp 11</option>
                                <option value={12}>Lớp 12</option>
                            </select>
                        </div>

                        <div>
                            <label htmlFor="school" className="block text-sm font-medium text-[var(--text-primary)] mb-2">
                                Trường
                            </label>
                            <input
                                id="school"
                                type="text"
                                value={formData.school}
                                onChange={(e) => setFormData({ ...formData, school: e.target.value })}
                                className="input-glass"
                                placeholder="THPT..."
                            />
                        </div>
                    </div>

                    <button
                        type="submit"
                        disabled={isSubmitting}
                        className="btn-primary w-full py-3.5 text-base disabled:opacity-50 disabled:cursor-not-allowed mt-2"
                    >
                        {isSubmitting ? (
                            <span className="flex items-center justify-center gap-2">
                                <svg className="animate-spin h-5 w-5" viewBox="0 0 24 24">
                                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                                </svg>
                                Đang đăng ký...
                            </span>
                        ) : 'Đăng ký'}
                    </button>
                </form>

                {/* Login Link */}
                <p className="text-center text-[var(--text-secondary)] mt-6">
                    Đã có tài khoản?{' '}
                    <Link href="/auth/login" className="text-[var(--primary)] hover:underline font-medium">
                        Đăng nhập ngay
                    </Link>
                </p>
            </div>
        </div>
    );
}
