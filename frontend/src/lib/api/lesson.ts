import { apiClient } from './client';
import { Lesson, CreateLessonRequest, UpdateLessonRequest } from '@/lib/types/lesson';

export const lessonApi = {
    // Get all lessons for a course
    getByCourse: async (courseId: string): Promise<Lesson[]> => {
        const response = await apiClient.get<Lesson[]>(`/api/courses/${courseId}/lessons`);
        return response.data;
    },

    // Get single lesson
    getById: async (id: string): Promise<Lesson> => {
        const response = await apiClient.get<Lesson>(`/api/lessons/${id}`);
        return response.data;
    },

    // Create new lesson
    create: async (data: CreateLessonRequest): Promise<Lesson> => {
        const response = await apiClient.post<Lesson>('/api/lessons', data);
        return response.data;
    },

    // Update existing lesson
    update: async (id: string, data: UpdateLessonRequest): Promise<void> => {
        await apiClient.put(`/api/lessons/${id}`, data);
    },

    // Delete lesson
    delete: async (id: string): Promise<void> => {
        await apiClient.delete(`/api/lessons/${id}`);
    },
};
