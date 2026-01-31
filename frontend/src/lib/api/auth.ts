import { apiClient } from './client';
import { User } from '@/lib/store/authStore';

export interface RegisterRequest {
    email: string;
    password: string;
    fullName: string;
    grade?: number;
    school?: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    user: User;
}

export const authApi = {
    register: async (data: RegisterRequest): Promise<AuthResponse> => {
        const response = await apiClient.post<AuthResponse>('/api/auth/register', data);
        return response.data;
    },

    login: async (data: LoginRequest): Promise<AuthResponse> => {
        const response = await apiClient.post<AuthResponse>('/api/auth/login', data);
        return response.data;
    },

    googleLogin: async (idToken: string): Promise<AuthResponse> => {
        const response = await apiClient.post<AuthResponse>('/api/auth/google', { idToken });
        return response.data;
    },

    logout: async (refreshToken: string): Promise<void> => {
        await apiClient.post('/api/auth/logout', { refreshToken });
    },

    refreshToken: async (refreshToken: string): Promise<AuthResponse> => {
        const response = await apiClient.post<AuthResponse>('/api/auth/refresh', {
            refreshToken,
        });
        return response.data;
    },
};
