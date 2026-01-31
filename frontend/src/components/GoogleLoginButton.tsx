'use client';

import { useEffect, useRef } from 'react';
import { authApi } from '@/lib/api/auth';
import { useAuthStore } from '@/lib/store/authStore';
import { useRouter } from 'next/navigation';

declare global {
    interface Window {
        google?: {
            accounts: {
                id: {
                    initialize: (config: any) => void;
                    renderButton: (element: HTMLElement, config: any) => void;
                };
            };
        };
    }
}

interface GoogleLoginButtonProps {
    onError?: (error: string) => void;
}

export default function GoogleLoginButton({ onError }: GoogleLoginButtonProps) {
    const buttonRef = useRef<HTMLDivElement>(null);
    const router = useRouter();
    const { setAuth, setLoading } = useAuthStore();

    useEffect(() => {
        const clientId = process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID;

        if (!clientId || clientId === 'your_google_client_id.apps.googleusercontent.com') {
            console.warn('Google Client ID not configured');
            return;
        }

        const script = document.createElement('script');
        script.src = 'https://accounts.google.com/gsi/client';
        script.async = true;
        script.defer = true;
        script.onload = () => {
            if (window.google && buttonRef.current) {
                window.google.accounts.id.initialize({
                    client_id: clientId,
                    callback: handleCredentialResponse,
                });

                window.google.accounts.id.renderButton(buttonRef.current, {
                    theme: 'outline',
                    size: 'large',
                    width: '100%',
                    text: 'continue_with',
                    shape: 'rectangular',
                });
            }
        };
        document.body.appendChild(script);

        return () => {
            document.body.removeChild(script);
        };
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const handleCredentialResponse = async (response: { credential: string }) => {
        setLoading(true);
        try {
            const authResponse = await authApi.googleLogin(response.credential);
            setAuth(authResponse.user, authResponse.accessToken, authResponse.refreshToken);

            if (authResponse.user.role === 'Admin') {
                router.push('/admin');
            } else if (authResponse.user.role === 'Instructor') {
                router.push('/instructor/dashboard');
            } else {
                router.push('/courses');
            }
        } catch (err: any) {
            const message = err.response?.data?.message || 'Đăng nhập Google thất bại';
            onError?.(message);
        } finally {
            setLoading(false);
        }
    };

    const clientId = process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID;

    if (!clientId || clientId === 'your_google_client_id.apps.googleusercontent.com') {
        return null;
    }

    return (
        <div className="w-full">
            <div className="relative my-6">
                <div className="absolute inset-0 flex items-center">
                    <div className="w-full border-t border-[var(--glass-border)]"></div>
                </div>
                <div className="relative flex justify-center text-sm">
                    <span className="px-3 glass-light text-[var(--text-muted)]">hoặc</span>
                </div>
            </div>
            <div ref={buttonRef} className="flex justify-center [&>div]:!rounded-xl [&>div]:!transition-all-smooth"></div>
        </div>
    );
}
