'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuthStore } from '@/lib/store/authStore';
import { authApi } from '@/lib/api/auth';
import GoogleLoginButton from '@/components/GoogleLoginButton';

export default function LoginPage() {
    const router = useRouter();
    const { user, setAuth, setLoading } = useAuthStore();

    const [formData, setFormData] = useState({
        email: '',
        password: '',
    });
    const [error, setError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsSubmitting(true);
        setLoading(true);

        try {
            const response = await authApi.login(formData);
            setAuth(response.user, response.accessToken, response.refreshToken);

            if (response.user.role === 'Admin') {
                router.push('/admin');
            } else if (response.user.role === 'Instructor') {
                router.push('/instructor/dashboard');
            } else {
                router.push('/courses');
            }
        } catch (err: any) {
            setError(err.response?.data?.message || 'Đăng nhập thất bại. Vui lòng thử lại.');
        } finally {
            setIsSubmitting(false);
            setLoading(false);
        }
    };

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
        <div className="min-h-screen flex items-center justify-center p-4 relative overflow-hidden">
            {/* Background Orbs */}
            <div className="absolute inset-0 overflow-hidden pointer-events-none">
                <div className="absolute -top-40 -left-40 w-80 h-80 bg-gradient-to-br from-blue-400/30 to-purple-500/30 rounded-full blur-3xl animate-float" />
                <div className="absolute -bottom-40 -right-40 w-96 h-96 bg-gradient-to-br from-pink-400/20 to-blue-500/20 rounded-full blur-3xl animate-float" style={{ animationDelay: '-3s' }} />
            </div>

            {/* Login Card */}
            <div className="glass w-full max-w-md p-8 relative z-10 animate-fade-in-up">
                {/* Logo */}
                <div className="text-center mb-8">
                    <Link href="/" className="text-3xl font-bold text-gradient inline-block mb-2">
                        hoccungtonhe
                    </Link>
                    <h1 className="text-xl font-semibold text-[var(--text-primary)]">
                        Đăng nhập
                    </h1>
                    <p className="text-sm text-[var(--text-secondary)] mt-1">
                        Chào mừng bạn trở lại!
                    </p>
                </div>

                {/* Error Message */}
                {error && (
                    <div className="mb-6 p-4 rounded-xl bg-red-500/10 border border-red-500/20">
                        <p className="text-red-500 text-sm text-center">{error}</p>
                    </div>
                )}

                {/* Login Form */}
                <form onSubmit={handleSubmit} className="space-y-5">
                    <div>
                        <label htmlFor="email" className="block text-sm font-medium text-[var(--text-primary)] mb-2">
                            Email
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
                            Mật khẩu
                        </label>
                        <input
                            id="password"
                            type="password"
                            required
                            value={formData.password}
                            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                            className="input-glass"
                            placeholder="••••••••"
                        />
                    </div>

                    <button
                        type="submit"
                        disabled={isSubmitting}
                        className="btn-primary w-full py-3.5 text-base disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {isSubmitting ? (
                            <span className="flex items-center justify-center gap-2">
                                <svg className="animate-spin h-5 w-5" viewBox="0 0 24 24">
                                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                                </svg>
                                Đang đăng nhập...
                            </span>
                        ) : 'Đăng nhập'}
                    </button>
                </form>

                {/* Google Login */}
                <GoogleLoginButton onError={(msg) => setError(msg)} />

                {/* Register Link */}
                <p className="text-center text-[var(--text-secondary)] mt-6">
                    Chưa có tài khoản?{' '}
                    <Link href="/auth/register" className="text-[var(--primary)] hover:underline font-medium">
                        Đăng ký ngay
                    </Link>
                </p>

                {/* Demo Accounts */}
                <div className="mt-8 pt-6 border-t border-[var(--glass-border)]">
                    <p className="text-xs text-[var(--text-muted)] text-center mb-3">Tài khoản demo:</p>
                    <div className="grid grid-cols-3 gap-2 text-xs">
                        <div className="glass-light p-2 rounded-lg text-center">
                            <div className="font-medium text-[var(--text-primary)]">Admin</div>
                            <div className="text-[var(--text-muted)] truncate">admin@eduvn.com</div>
                        </div>
                        <div className="glass-light p-2 rounded-lg text-center">
                            <div className="font-medium text-[var(--text-primary)]">Giảng viên</div>
                            <div className="text-[var(--text-muted)] truncate">instructor@eduvn.com</div>
                        </div>
                        <div className="glass-light p-2 rounded-lg text-center">
                            <div className="font-medium text-[var(--text-primary)]">Học sinh</div>
                            <div className="text-[var(--text-muted)] truncate">student@eduvn.com</div>
                        </div>
                    </div>
                    <p className="text-xs text-[var(--text-muted)] text-center mt-2">Mật khẩu: [Role]@123</p>
                </div>
            </div>
        </div>
    );
}
