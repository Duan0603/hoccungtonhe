import Link from 'next/link';

export default function PendingApprovalPage() {
    return (
        <div className="min-h-screen flex items-center justify-center p-4">
            {/* Background */}
            <div className="fixed inset-0 overflow-hidden pointer-events-none">
                <div className="absolute top-1/3 left-1/4 w-72 h-72 bg-gradient-to-br from-amber-400/20 to-orange-400/20 rounded-full blur-3xl animate-float" />
                <div className="absolute bottom-1/3 right-1/4 w-80 h-80 bg-gradient-to-br from-yellow-400/20 to-amber-400/20 rounded-full blur-3xl animate-float-delayed" />
            </div>

            <div className="glass p-10 max-w-md w-full text-center relative z-10 animate-fade-in-up">
                <div className="w-20 h-20 mx-auto mb-6 rounded-2xl bg-gradient-to-br from-amber-400 to-orange-500 flex items-center justify-center text-4xl shadow-glow">
                    ⏳
                </div>
                <h1 className="text-2xl font-bold text-[var(--text-primary)] mb-3">Đang chờ duyệt</h1>
                <p className="text-[var(--text-secondary)] mb-8 leading-relaxed">
                    Tài khoản giảng viên của bạn đang được Admin xem xét.
                    Vui lòng quay lại sau.
                </p>

                <div className="glass-light p-4 mb-6 text-left">
                    <div className="flex items-center gap-3 text-sm text-[var(--text-secondary)]">
                        <div className="w-2 h-2 rounded-full bg-amber-500 animate-pulse"></div>
                        Trạng thái: <span className="font-medium text-amber-600">Pending</span>
                    </div>
                </div>

                <Link href="/" className="btn-primary inline-block">
                    Về trang chủ
                </Link>
            </div>
        </div>
    );
}
