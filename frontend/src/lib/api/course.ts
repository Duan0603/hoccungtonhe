import { apiClient } from './client';
import {
    Course,
    CourseFilter,
    CourseResponse,
    CreateCourseRequest,
    UpdateCourseRequest
} from '@/lib/types/course';

export const courseApi = {
    // Public Endpoint: Get all courses with filter
    getAll: async (filter?: CourseFilter): Promise<CourseResponse> => {
        const params = new URLSearchParams();

        if (filter) {
            if (filter.search) params.append('search', filter.search);
            if (filter.subject) params.append('subject', filter.subject);
            if (filter.grade) params.append('grade', filter.grade.toString());
            if (filter.minPrice !== undefined) params.append('minPrice', filter.minPrice.toString());
            if (filter.maxPrice !== undefined) params.append('maxPrice', filter.maxPrice.toString());
            if (filter.page) params.append('page', filter.page.toString());
            if (filter.pageSize) params.append('pageSize', filter.pageSize.toString());
        }

        const response = await apiClient.get<CourseResponse>(`/api/courses?${params.toString()}`);
        return response.data;
    },

    // Public Endpoint: Get course details
    getById: async (id: string): Promise<Course> => {
        const response = await apiClient.get<Course>(`/api/courses/${id}`);
        return response.data;
    },

    // Instructor Endpoint: Get my courses
    getMyCourses: async (): Promise<Course[]> => {
        const response = await apiClient.get<Course[]>('/api/courses/my-courses');
        return response.data;
    },

    // Instructor/Admin: Create course
    create: async (data: CreateCourseRequest): Promise<Course> => {
        const response = await apiClient.post<Course>('/api/courses', data);
        return response.data;
    },

    // Instructor/Admin: Update course
    update: async (id: string, data: UpdateCourseRequest): Promise<void> => {
        await apiClient.put(`/api/courses/${id}`, data);
    },

    // Instructor/Admin: Delete course
    delete: async (id: string): Promise<void> => {
        await apiClient.delete(`/api/courses/${id}`);
    },
};
