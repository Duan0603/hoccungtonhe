import { create } from 'zustand';
import { persist } from 'zustand/middleware';

export interface User {
    id: string;
    email: string;
    fullName: string;
    role: 'Student' | 'Instructor' | 'Admin';
    status: 'Pending' | 'Approved' | 'Rejected' | 'Blocked';
    grade?: number;
    school?: string;
}

interface AuthState {
    user: User | null;
    accessToken: string | null;
    refreshToken: string | null;
    isLoading: boolean;

    // Actions
    setAuth: (user: User, accessToken: string, refreshToken: string) => void;
    logout: () => void;
    setLoading: (loading: boolean) => void;
    updateTokens: (accessToken: string, refreshToken: string) => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            user: null,
            accessToken: null,
            refreshToken: null,
            isLoading: false,

            setAuth: (user, accessToken, refreshToken) =>
                set({ user, accessToken, refreshToken, isLoading: false }),

            logout: () =>
                set({ user: null, accessToken: null, refreshToken: null }),

            setLoading: (loading) => set({ isLoading: loading }),

            updateTokens: (accessToken, refreshToken) =>
                set({ accessToken, refreshToken }),
        }),
        {
            name: 'eduvn-auth',
            partialize: (state) => ({
                user: state.user,
                accessToken: state.accessToken,
                refreshToken: state.refreshToken,
            }),
        }
    )
);
