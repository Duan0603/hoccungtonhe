using EduVN.Application.DTOs.Payment;

namespace EduVN.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreatePaymentLinkAsync(long orderCode, int amount, string description, string cancelUrl, string returnUrl, string buyerName = "", string buyerEmail = "");
    Task<PaymentWebhookDto> VerifyWebhookAsync(PaymentWebhookDto webhookBody);
}
