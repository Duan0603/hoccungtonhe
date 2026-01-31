'use client';

import Link from 'next/link';
import { useEffect } from 'react';

export default function PaymentSuccessPage() {
    return (
        <div className="min-h-screen flex items-center justify-center bg-[var(--bg-primary)]">
            <div className="glass p-8 max-w-md w-full text-center">
                <div className="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-6">
                    <svg className="w-10 h-10 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                    </svg>
                </div>

                <h1 className="text-2xl font-bold text-[var(--text-primary)] mb-4">
                    Thanh toán thành công!
                </h1>

                <p className="text-[var(--text-secondary)] mb-8">
                    Cảm ơn bạn đã đăng ký khóa học. Bạn có thể bắt đầu học ngay bây giờ.
                </p>

                <div className="space-y-3">
                    <Link
                        href="/courses"
                        className="btn-primary w-full block py-3"
                    >
                        Vào học ngay
                    </Link>
                    <Link
                        href="/"
                        className="btn-glass w-full block py-3"
                    >
                        Về trang chủ
                    </Link>
                </div>
            </div>
        </div>
    );
}
