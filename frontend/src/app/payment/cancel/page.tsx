'use client';

import Link from 'next/link';

export default function PaymentCancelPage() {
    return (
        <div className="min-h-screen flex items-center justify-center bg-[var(--bg-primary)]">
            <div className="glass p-8 max-w-md w-full text-center">
                <div className="w-20 h-20 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-6">
                    <svg className="w-10 h-10 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </div>

                <h1 className="text-2xl font-bold text-[var(--text-primary)] mb-4">
                    Thanh toán đã bị hủy
                </h1>

                <p className="text-[var(--text-secondary)] mb-8">
                    Giao dịch của bạn chưa được hoàn tất. Nếu gặp vấn đề, vui lòng liên hệ bộ phận hỗ trợ.
                </p>

                <div className="space-y-3">
                    <Link
                        href="/courses"
                        className="btn-primary w-full block py-3"
                    >
                        Xem khóa học khác
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
