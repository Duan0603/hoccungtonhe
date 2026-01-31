import Link from 'next/link';

export default function UnauthorizedPage() {
    return (
        <div className="min-h-screen flex items-center justify-center p-4">
            {/* Background */}
            <div className="fixed inset-0 overflow-hidden pointer-events-none">
                <div className="absolute top-1/3 left-1/4 w-72 h-72 bg-gradient-to-br from-red-400/20 to-rose-400/20 rounded-full blur-3xl animate-float" />
                <div className="absolute bottom-1/3 right-1/4 w-80 h-80 bg-gradient-to-br from-pink-400/20 to-red-400/20 rounded-full blur-3xl animate-float-delayed" />
            </div>

            <div className="glass p-10 max-w-md w-full text-center relative z-10 animate-fade-in-up">
                <div className="w-20 h-20 mx-auto mb-6 rounded-2xl bg-gradient-to-br from-red-400 to-rose-500 flex items-center justify-center text-4xl shadow-glow">
                    🚫
                </div>
                <h1 className="text-2xl font-bold text-[var(--text-primary)] mb-3">Truy cập bị từ chối</h1>
                <p className="text-[var(--text-secondary)] mb-8 leading-relaxed">
                    Bạn không có quyền truy cập trang này.
                    Vui lòng đăng nhập với tài khoản phù hợp.
                </p>

                <div className="flex justify-center gap-3">
                    <Link href="/" className="btn-glass">
                        Về trang chủ
                    </Link>
                    <Link href="/auth/login" className="btn-primary">
                        Đăng nhập
                    </Link>
                </div>
            </div>
        </div>
    );
}
