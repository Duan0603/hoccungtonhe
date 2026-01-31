export interface Lesson {
    id: string;
    courseId: string;
    title: string;
    videoUrl: string | null;
    documentUrl: string | null;
    orderIndex: number;
    duration: number; // in minutes
    isPublished: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface CreateLessonRequest {
    courseId: string;
    title: string;
    videoUrl?: string;
    documentUrl?: string;
    orderIndex?: number;
    duration?: number;
}

export interface UpdateLessonRequest {
    title?: string;
    videoUrl?: string;
    documentUrl?: string;
    orderIndex?: number;
    duration?: number;
    isPublished?: boolean;
}
