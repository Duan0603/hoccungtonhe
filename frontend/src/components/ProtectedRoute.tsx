'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/lib/store/authStore';

interface ProtectedRouteProps {
    children: React.ReactNode;
    allowedRoles?: Array<'Student' | 'Instructor' | 'Admin'>;
}

export default function ProtectedRoute({
    children,
    allowedRoles,
}: ProtectedRouteProps) {
    const router = useRouter();
    const { user, isLoading } = useAuthStore();
    const [hasMounted, setHasMounted] = useState(false);

    useEffect(() => {
        setHasMounted(true);
    }, []);

    useEffect(() => {
        if (!hasMounted) return;

        if (!user && !isLoading) {
            router.push('/auth/login');
            return;
        }

        if (user && allowedRoles && !allowedRoles.includes(user.role)) {
            router.push('/unauthorized');
            return;
        }

        if (user && user.status !== 'Approved') {
            router.push('/pending-approval');
            return;
        }
    }, [user, isLoading, allowedRoles, router, hasMounted]);

    if (!hasMounted || isLoading || !user) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                {/* Background */}
                <div className="fixed inset-0 overflow-hidden pointer-events-none">
                    <div className="absolute top-1/3 left-1/4 w-72 h-72 bg-gradient-to-br from-blue-400/20 to-cyan-400/20 rounded-full blur-3xl animate-float" />
                    <div className="absolute bottom-1/3 right-1/4 w-80 h-80 bg-gradient-to-br from-indigo-400/20 to-purple-400/20 rounded-full blur-3xl animate-float-delayed" />
                </div>

                <div className="glass p-8 text-center relative z-10">
                    <div className="w-12 h-12 mx-auto mb-4 rounded-full bg-gradient-to-br from-[var(--accent-start)] to-[var(--accent-end)] flex items-center justify-center animate-pulse-glow">
                        <div className="animate-spin w-6 h-6 border-2 border-white border-t-transparent rounded-full"></div>
                    </div>
                    <p className="text-[var(--text-secondary)]">Đang tải...</p>
                </div>
            </div>
        );
    }

    return <>{children}</>;
}
