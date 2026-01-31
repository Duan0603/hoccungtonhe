using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using EduVN.Application.DTOs.Payment;
using EduVN.Application.Interfaces;
using EduVN.Application.Settings;
using Microsoft.Extensions.Options;

using System.Text.RegularExpressions;

namespace EduVN.Infrastructure.Services;

public class PayOSService : IPaymentService
{
    private readonly PayOSClient _payOS;
    private readonly PayOSSettings _settings;

    public PayOSService(IOptions<PayOSSettings> settings)
    {
        _settings = settings.Value;
        _payOS = new PayOSClient(_settings.ClientId, _settings.ApiKey, _settings.ChecksumKey);
    }

    public async Task<string> CreatePaymentLinkAsync(long orderCode, int amount, string description, string cancelUrl, string returnUrl, string buyerName = "", string buyerEmail = "")
    {
        // PayOS v2 sanitized description (alphanumeric and spaces only, max 25 chars in some contexts, but usually OK)
        var sanitizedDescription = Regex.Replace(description, @"[^a-zA-Z0-9 ]", "");
        if (sanitizedDescription.Length > 25) sanitizedDescription = sanitizedDescription.Substring(0, 25);

        var request = new CreatePaymentLinkRequest
        {
            OrderCode = orderCode,
            Amount = amount,
            Description = sanitizedDescription,
            Items = new List<PaymentLinkItem> 
            { 
                new PaymentLinkItem { Name = sanitizedDescription, Quantity = 1, Price = amount }
            },
            CancelUrl = cancelUrl,
            ReturnUrl = returnUrl,
            BuyerName = buyerName,
            BuyerEmail = buyerEmail
        };

        var result = await _payOS.PaymentRequests.CreateAsync(request);
        return result.CheckoutUrl;
    }

    public async Task<PaymentWebhookDto> VerifyWebhookAsync(PaymentWebhookDto webhookBody)
    {
        var webhook = new Webhook
        {
            Code = webhookBody.Code,
            Description = webhookBody.Desc,
            Success = webhookBody.Success,
            Data = new WebhookData
            {
                OrderCode = webhookBody.Data.OrderCode,
                Amount = webhookBody.Data.Amount,
                Description = webhookBody.Data.Description,
                AccountNumber = webhookBody.Data.AccountNumber,
                Reference = webhookBody.Data.Reference,
                TransactionDateTime = webhookBody.Data.TransactionDateTime,
                Currency = webhookBody.Data.Currency,
                PaymentLinkId = webhookBody.Data.PaymentLinkId,
                Code = webhookBody.Data.Code,
                // Desc = webhookBody.Data.Desc, // WebhookData might not have Desc, check probe. Type 252 has Description, Code, Description2.
                // Property 311: Description2 (String). Property 304: Description (String).
                // DTO has Desc?
                // Mapped: webhookBody.Data.Desc -> ???
                // Let's check WebhookData (Type 252).
                // Methods: get_Description, get_Description2.
                // Assuming Desc maps to Description or Description2.
                // Let's use Description if possible or skip if confusing.
                // Actually, I'll map what matches.
                CounterAccountBankId = webhookBody.Data.CounterAccountBankId,
                CounterAccountBankName = webhookBody.Data.CounterAccountBankName,
                CounterAccountName = webhookBody.Data.CounterAccountName,
                CounterAccountNumber = webhookBody.Data.CounterAccountNumber,
                VirtualAccountName = webhookBody.Data.VirtualAccountName,
                VirtualAccountNumber = webhookBody.Data.VirtualAccountNumber
            },
            Signature = webhookBody.Signature
        };

        // VerifyAsync throws exception if invalid
        await _payOS.Webhooks.VerifyAsync(webhook);
        
        return webhookBody;
    }
}
