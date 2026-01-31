export interface Course {
    id: string;
    title: string;
    description: string | null;
    price: number;
    subject: string;
    grade: number;
    thumbnailUrl: string | null;
    isPublished: boolean;
    createdAt: string;
    updatedAt: string;
    instructorId: string;
    instructorName: string;
}

export interface CourseFilter {
    search?: string;
    subject?: string;
    grade?: number;
    minPrice?: number;
    maxPrice?: number;
    page?: number;
    pageSize?: number;
}

export interface CourseResponse {
    data: Course[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

export interface CreateCourseRequest {
    title: string;
    description?: string;
    price: number;
    subject: string;
    grade: number;
}

export interface UpdateCourseRequest {
    title?: string;
    description?: string;
    price?: number;
    subject?: string;
    grade?: number;
    isPublished?: boolean;
}
