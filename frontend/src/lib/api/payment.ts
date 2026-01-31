import { apiClient } from './client';

export interface CreatePaymentResponse {
    checkoutUrl: string;
}

export const paymentApi = {
    createPaymentLink: async (courseId: string, returnUrl?: string, cancelUrl?: string) => {
        const response = await apiClient.post<CreatePaymentResponse>('/api/payment/create-link', {
            courseId,
            returnUrl,
            cancelUrl
        });
        return response.data;
    }
};
