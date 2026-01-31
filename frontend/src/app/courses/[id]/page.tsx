'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { Course } from '@/lib/types/course';
import { Lesson } from '@/lib/types/lesson';
import { courseApi } from '@/lib/api/course';
import { lessonApi } from '@/lib/api/lesson';
import ProtectedRoute from '@/components/ProtectedRoute';
import { useAuthStore } from '@/lib/store/authStore';
import VideoPlayer from '@/components/VideoPlayer';
import AddLessonForm from '@/components/AddLessonForm';

export default function CourseDetailPage() {
    const params = useParams();
    const router = useRouter();
    const id = params.id as string;
    const { user } = useAuthStore();

    const [course, setCourse] = useState<Course | null>(null);
    const [lessons, setLessons] = useState<Lesson[]>([]);
    const [currentLesson, setCurrentLesson] = useState<Lesson | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showAddLesson, setShowAddLesson] = useState(false);
    const [hasMounted, setHasMounted] = useState(false);

    useEffect(() => {
        setHasMounted(true);
    }, []);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [courseData, lessonsData] = await Promise.all([
                    courseApi.getById(id),
                    lessonApi.getByCourse(id)
                ]);
                setCourse(courseData);
                setLessons(lessonsData);
                if (lessonsData.length > 0) {
                    setCurrentLesson(lessonsData[0]);
                }
            } catch (err: any) {
                console.error('Error fetching data:', err);
                setError('Không tìm thấy khóa học hoặc có lỗi xảy ra.');
            } finally {
                setLoading(false);
            }
        };
        if (id) fetchData();
    }, [id]);

    const handleLessonAdded = async () => {
        setShowAddLesson(false);
        const lessonsData = await lessonApi.getByCourse(id);
        setLessons(lessonsData);
    };

    const handleDeleteLesson = async (lessonId: string, lessonTitle: string) => {
        if (!confirm(`Bạn có chắc muốn xóa bài học "${lessonTitle}"?`)) return;
        try {
            await lessonApi.delete(lessonId);
            const lessonsData = await lessonApi.getByCourse(id);
            setLessons(lessonsData);
            if (currentLesson?.id === lessonId) {
                setCurrentLesson(lessonsData.length > 0 ? lessonsData[0] : null);
            }
        } catch (err) {
            console.error('Error deleting lesson:', err);
            alert('Không thể xóa bài học.');
        }
    };

    // Loading State
    if (loading || !hasMounted) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="glass p-8 text-center">
                    <div className="animate-spin w-10 h-10 border-3 border-[var(--primary)] border-t-transparent rounded-full mx-auto mb-4"></div>
                    <p className="text-[var(--text-secondary)]">Đang tải...</p>
                </div>
            </div>
        );
    }

    // Error State
    if (error || !course) {
        return (
            <ProtectedRoute>
                <div className="min-h-screen flex items-center justify-center p-4">
                    <div className="glass p-8 text-center max-w-md">
                        <div className="text-5xl mb-4">⚠️</div>
                        <h2 className="text-xl font-bold text-[var(--text-primary)] mb-2">Có lỗi xảy ra</h2>
                        <p className="text-[var(--text-secondary)] mb-6">{error}</p>
                        <Link href="/courses" className="btn-primary inline-block">
                            Quay lại danh sách
                        </Link>
                    </div>
                </div>
            </ProtectedRoute>
        );
    }

    // Role-based permissions
    const isOwner = user?.role === 'Instructor' && user.id === course.instructorId;
    const isAdmin = user?.role === 'Admin';
    const canManage = isOwner || isAdmin; // Can edit, add lessons, delete
    const isStudent = user?.role === 'Student';

    const formattedPrice = new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND',
    }).format(course.price);

    const thumbnail = course.thumbnailUrl
        ? `${process.env.NEXT_PUBLIC_API_URL}${course.thumbnailUrl}`
        : 'https://placehold.co/800x400/667eea/ffffff?text=Course';

    return (
        <ProtectedRoute>
            <div className="min-h-screen pb-20">
                {/* Glass Navbar */}
                <nav className="glass-nav sticky top-0 z-50">
                    <div className="container mx-auto px-6 py-4 flex items-center gap-4">
                        <Link
                            href={canManage ? "/instructor/dashboard" : "/courses"}
                            className="p-2 hover:bg-[var(--glass-bg-light)] rounded-xl transition-all-smooth"
                        >
                            <svg className="w-6 h-6 text-[var(--text-secondary)]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                            </svg>
                        </Link>
                        <h1 className="text-xl font-bold text-[var(--text-primary)] truncate flex-1">
                            {course.title}
                        </h1>

                        {/* Instructor/Admin Actions */}
                        {canManage && (
                            <div className="flex items-center gap-2">
                                <Link
                                    href={`/instructor/courses/${course.id}/edit`}
                                    className="btn-glass !py-2 !px-4 text-sm"
                                >
                                    ✏️ Sửa khóa học
                                </Link>
                                <button
                                    onClick={() => setShowAddLesson(true)}
                                    className="btn-primary !py-2 !px-4 text-sm"
                                >
                                    + Thêm bài học
                                </button>
                            </div>
                        )}
                    </div>
                </nav>

                <div className="container mx-auto px-6 py-8">
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                        {/* Main Content */}
                        <div className="lg:col-span-2 space-y-6">

                            {/* Video Player */}
                            <div className="glass overflow-hidden !rounded-2xl">
                                {currentLesson?.videoUrl ? (
                                    <VideoPlayer url={currentLesson.videoUrl} />
                                ) : (
                                    <div className="aspect-video relative bg-gradient-to-br from-[var(--accent-start)] to-[var(--accent-end)]">
                                        {/* eslint-disable-next-line @next/next/no-img-element */}
                                        <img
                                            src={thumbnail}
                                            alt={course.title}
                                            className="w-full h-full object-cover opacity-60"
                                        />
                                        <div className="absolute inset-0 flex items-center justify-center">
                                            <div className="glass text-center p-6">
                                                <div className="text-4xl mb-2">📚</div>
                                                <p className="text-[var(--text-primary)] font-semibold">
                                                    {currentLesson ? 'Bài học không có video' : 'Chọn bài học để bắt đầu'}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                )}
                            </div>

                            {/* Current Lesson Info */}
                            {currentLesson && (
                                <div className="glass p-6">
                                    <h2 className="text-xl font-bold text-[var(--text-primary)] mb-3">{currentLesson.title}</h2>
                                    <div className="flex flex-wrap gap-4 text-sm">
                                        <span className="glass-light px-3 py-1.5 text-[var(--text-secondary)]">
                                            ⏱ {currentLesson.duration} phút
                                        </span>
                                        {currentLesson.documentUrl && (
                                            <a
                                                href={`${process.env.NEXT_PUBLIC_API_URL}${currentLesson.documentUrl}`}
                                                target="_blank"
                                                rel="noopener noreferrer"
                                                className="glass-light px-3 py-1.5 text-[var(--primary)] hover:bg-[var(--primary-light)] transition-all-smooth"
                                            >
                                                📄 Tải tài liệu
                                            </a>
                                        )}
                                    </div>
                                </div>
                            )}

                            {/* Add Lesson Form (Instructor Only) */}
                            {showAddLesson && canManage && (
                                <div className="animate-fade-in-up">
                                    <AddLessonForm
                                        courseId={course.id}
                                        onSuccess={handleLessonAdded}
                                        onCancel={() => setShowAddLesson(false)}
                                    />
                                </div>
                            )}

                            {/* Description */}
                            <div className="glass p-6">
                                <h2 className="text-lg font-bold text-[var(--text-primary)] mb-4">Giới thiệu khóa học</h2>
                                <p className="text-[var(--text-secondary)] whitespace-pre-line leading-relaxed">
                                    {course.description || 'Chưa có mô tả chi tiết.'}
                                </p>
                            </div>

                            {/* Curriculum */}
                            <div className="glass overflow-hidden">
                                <div className="px-6 py-4 border-b border-[var(--glass-border)] flex justify-between items-center">
                                    <h2 className="text-lg font-bold text-[var(--text-primary)]">Nội dung khóa học</h2>
                                    <span className="glass-light px-3 py-1 text-sm text-[var(--text-secondary)]">
                                        {lessons.length} bài
                                    </span>
                                </div>
                                <div className="divide-y divide-[var(--glass-border)]">
                                    {lessons.length === 0 ? (
                                        <div className="p-8 text-center text-[var(--text-muted)]">
                                            {canManage ? 'Chưa có bài học. Nhấn "Thêm bài học" để bắt đầu.' : 'Chưa có bài học nào.'}
                                        </div>
                                    ) : (
                                        lessons.map((lesson, index) => (
                                            <div
                                                key={lesson.id}
                                                className={`px-6 py-4 flex items-center gap-4 hover:bg-[var(--glass-bg-light)] transition-all-smooth ${currentLesson?.id === lesson.id ? 'bg-[var(--primary-light)]' : ''}`}
                                            >
                                                <button
                                                    onClick={() => setCurrentLesson(lesson)}
                                                    className="flex items-center gap-4 flex-1 text-left"
                                                >
                                                    <div className={`w-10 h-10 rounded-xl flex items-center justify-center text-sm font-bold transition-all-smooth ${currentLesson?.id === lesson.id
                                                        ? 'bg-gradient-to-br from-[var(--accent-start)] to-[var(--accent-end)] text-white shadow-glow'
                                                        : 'bg-[var(--glass-bg-light)] text-[var(--text-secondary)]'
                                                        }`}>
                                                        {index + 1}
                                                    </div>
                                                    <div className="flex-1">
                                                        <div className={`font-medium ${currentLesson?.id === lesson.id ? 'text-[var(--primary)]' : 'text-[var(--text-primary)]'}`}>
                                                            {lesson.title}
                                                        </div>
                                                        <div className="text-xs text-[var(--text-muted)] mt-0.5 flex gap-3">
                                                            <span>{lesson.duration > 0 ? `${lesson.duration} phút` : 'Video'}</span>
                                                            {lesson.documentUrl && <span>+ Tài liệu</span>}
                                                        </div>
                                                    </div>
                                                    <div className={`text-sm ${currentLesson?.id === lesson.id ? 'text-[var(--primary)]' : 'text-[var(--text-muted)]'}`}>
                                                        {currentLesson?.id === lesson.id ? '▶ Đang phát' : '▶'}
                                                    </div>
                                                </button>

                                                {/* Delete button (Instructor Only) */}
                                                {canManage && (
                                                    <button
                                                        onClick={() => handleDeleteLesson(lesson.id, lesson.title)}
                                                        className="p-2 text-[var(--text-muted)] hover:text-red-500 hover:bg-red-500/10 rounded-lg transition-all-smooth"
                                                        title="Xóa bài học"
                                                    >
                                                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                                        </svg>
                                                    </button>
                                                )}
                                            </div>
                                        ))
                                    )}
                                </div>
                            </div>
                        </div>

                        {/* Sidebar */}
                        <div className="lg:col-span-1">
                            <div className="glass p-6 sticky top-24">
                                {/* Price */}
                                <div className="text-3xl font-bold text-gradient mb-1">
                                    {course.price === 0 ? 'Miễn phí' : formattedPrice}
                                </div>
                                <div className="text-sm text-[var(--text-muted)] mb-6">Giá trọn gói khóa học</div>

                                {/* Student: Buy/Enroll Button */}
                                {isStudent && (
                                    <button className="btn-primary w-full py-4 text-lg mb-4">
                                        {course.price === 0 ? 'Đăng ký miễn phí' : 'Mua khóa học'}
                                    </button>
                                )}

                                {/* Instructor: Edit Button */}
                                {canManage && (
                                    <Link
                                        href={`/instructor/courses/${course.id}/edit`}
                                        className="btn-primary w-full py-4 text-lg mb-4 block text-center"
                                    >
                                        ✏️ Chỉnh sửa khóa học
                                    </Link>
                                )}

                                {/* Course Info */}
                                <div className="space-y-4 border-t border-[var(--glass-border)] pt-6">
                                    <div className="flex items-center gap-3">
                                        <div className="w-12 h-12 rounded-full bg-gradient-to-br from-[var(--accent-start)] to-[var(--accent-end)] flex items-center justify-center text-white font-bold text-lg">
                                            {course.instructorName.charAt(0)}
                                        </div>
                                        <div>
                                            <div className="text-xs text-[var(--text-muted)]">Giảng viên</div>
                                            <div className="font-semibold text-[var(--text-primary)]">{course.instructorName}</div>
                                        </div>
                                    </div>

                                    <div className="space-y-2 pt-4">
                                        <div className="flex justify-between text-sm py-2 border-b border-[var(--glass-border)]">
                                            <span className="text-[var(--text-muted)]">Môn học</span>
                                            <span className="font-medium text-[var(--text-primary)]">{course.subject}</span>
                                        </div>
                                        <div className="flex justify-between text-sm py-2 border-b border-[var(--glass-border)]">
                                            <span className="text-[var(--text-muted)]">Lớp</span>
                                            <span className="font-medium text-[var(--text-primary)]">{course.grade}</span>
                                        </div>
                                        <div className="flex justify-between text-sm py-2 border-b border-[var(--glass-border)]">
                                            <span className="text-[var(--text-muted)]">Số bài học</span>
                                            <span className="font-medium text-[var(--text-primary)]">{lessons.length} bài</span>
                                        </div>
                                        <div className="flex justify-between text-sm py-2">
                                            <span className="text-[var(--text-muted)]">Trạng thái</span>
                                            <span className={`font-medium ${course.isPublished ? 'text-green-500' : 'text-[var(--text-muted)]'}`}>
                                                {course.isPublished ? '✓ Đã xuất bản' : 'Bản nháp'}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ProtectedRoute>
    );
}
