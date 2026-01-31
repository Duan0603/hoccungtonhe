'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { paymentApi } from '@/lib/api/payment';
import { useAuthStore } from '@/lib/store/authStore';

interface PaymentButtonProps {
    courseId: string;
    coursePrice: number;
    className?: string;
}

export default function PaymentButton({ courseId, coursePrice, className = '' }: PaymentButtonProps) {
    const [loading, setLoading] = useState(false);
    const router = useRouter();
    const { user } = useAuthStore();

    const handlePayment = async () => {
        if (!user) {
            router.push(`/auth/login?redirect=/courses/${courseId}`);
            return;
        }

        try {
            setLoading(true);
            // Construct absolute URLs for return/cancel
            const returnUrl = `${window.location.origin}/payment/success`;
            const cancelUrl = `${window.location.origin}/payment/cancel`;

            const data = await paymentApi.createPaymentLink(courseId, returnUrl, cancelUrl);

            // Redirect to PayOS checkout
            if (data.checkoutUrl) {
                window.location.href = data.checkoutUrl;
            } else {
                console.error('No checkout URL received');
                alert('Có lỗi xảy ra khi tạo link thanh toán. Vui lòng thử lại.');
            }
        } catch (error) {
            console.error('Payment error:', error);
            alert('Có lỗi xảy ra khi xử lý thanh toán.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <button
            onClick={handlePayment}
            disabled={loading}
            className={`px-6 py-3 bg-indigo-600 hover:bg-indigo-700 text-white font-semibold rounded-lg shadow-md transition duration-300 flex items-center justify-center ${className} ${loading ? 'opacity-75 cursor-not-allowed' : ''}`}
        >
            {loading ? (
                <>
                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Đang xử lý...
                </>
            ) : (
                <>
                    Mua khóa học ngay - {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(coursePrice)}
                </>
            )}
        </button>
    );
}
