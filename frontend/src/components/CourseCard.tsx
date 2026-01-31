import Link from 'next/link';
import { Course } from '@/lib/types/course';

interface CourseCardProps {
    course: Course;
}

export default function CourseCard({ course }: CourseCardProps) {
    const formattedPrice = new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND',
    }).format(course.price);

    const thumbnail = course.thumbnailUrl
        ? `${process.env.NEXT_PUBLIC_API_URL}${course.thumbnailUrl}`
        : 'https://placehold.co/600x400/667eea/ffffff?text=Course';

    return (
        <div className="glass-card overflow-hidden flex flex-col h-full group">
            <Link href={`/courses/${course.id}`} className="block relative aspect-video overflow-hidden">
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img
                    src={thumbnail}
                    alt={course.title}
                    className="object-cover w-full h-full group-hover:scale-110 transition-all duration-500"
                />
                {/* Overlay gradient */}
                <div className="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />

                {/* Tags */}
                <div className="absolute top-3 right-3 glass-light !rounded-lg px-3 py-1.5 text-xs font-bold text-[var(--primary)]">
                    Lớp {course.grade}
                </div>
                <div className="absolute top-3 left-3 bg-gradient-to-r from-[var(--accent-start)] to-[var(--accent-end)] px-3 py-1.5 rounded-lg text-xs font-bold text-white shadow-lg">
                    {course.subject}
                </div>
            </Link>

            <div className="p-5 flex-grow flex flex-col">
                <Link href={`/courses/${course.id}`}>
                    <h3 className="text-lg font-bold text-[var(--text-primary)] mb-2 line-clamp-2 group-hover:text-[var(--primary)] transition-colors">
                        {course.title}
                    </h3>
                </Link>

                <p className="text-[var(--text-secondary)] text-sm mb-4 line-clamp-2 flex-grow">
                    {course.description || 'Chưa có mô tả'}
                </p>

                <div className="flex items-center justify-between mt-auto pt-4 border-t border-[var(--glass-border)]">
                    <div className="flex items-center gap-2">
                        <div className="w-8 h-8 rounded-full bg-gradient-to-br from-[var(--accent-start)] to-[var(--accent-end)] flex items-center justify-center text-xs font-bold text-white shadow-sm">
                            {course.instructorName.charAt(0)}
                        </div>
                        <span className="text-sm text-[var(--text-secondary)] truncate max-w-[100px]" title={course.instructorName}>
                            {course.instructorName}
                        </span>
                    </div>
                    <span className="text-lg font-bold text-gradient">
                        {course.price === 0 ? 'Miễn phí' : formattedPrice}
                    </span>
                </div>
            </div>
        </div>
    );
}
