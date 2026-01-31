using EduVN.Application.DTOs.Payment;
using EduVN.Application.Interfaces;
using EduVN.Domain.Entities;
using EduVN.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduVN.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IOrderRepository _orderRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public PaymentController(
        IPaymentService paymentService,
        IOrderRepository orderRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _paymentService = paymentService;
        _orderRepository = orderRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    [HttpPost("create-link")]
    [Authorize]
    public async Task<IActionResult> CreatePaymentLink(CreatePaymentRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var user = await _userRepository.GetByIdAsync(userId);
        var course = await _courseRepository.GetByIdAsync(request.CourseId);

        if (course == null) return NotFound(new { message = "Course not found" });

        // Create Order
        // Generate a simplified OrderCode (must be unique integer <= 15 digits)
        // Using timestamp (seconds) * 1000 + random (0-999) = 13 digits
        long orderCode = (long)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000) + new Random().Next(0, 999);

        var order = new Order
        {
            OrderCode = orderCode,
            StudentId = userId,
            CourseId = course.Id,
            Amount = (int)course.Price,
            Status = "PENDING",
            CreatedAt = DateTime.UtcNow
        };

        await _orderRepository.AddAsync(order);

        var returnUrl = request.ReturnUrl ?? "http://localhost:3000/payment/success";
        var cancelUrl = request.CancelUrl ?? "http://localhost:3000/payment/cancel";

        try
        {
            var checkoutUrl = await _paymentService.CreatePaymentLinkAsync(
                order.OrderCode,
                order.Amount,
                course.Title, // Will be sanitized in service
                cancelUrl,
                returnUrl,
                user?.FullName ?? "Khach hang",
                user?.Email ?? "customer@example.com"
            );

            return Ok(new { checkoutUrl });
        }
        catch (Exception ex)
        {
            // Log full exception details to console for debugging
            Console.WriteLine($"PayOS Error: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] PaymentWebhookDto webhookBody)
    {
        try
        {
            var verifiedData = await _paymentService.VerifyWebhookAsync(webhookBody);
            
            // Check order status
            if (verifiedData.Success)
            {
                var order = await _orderRepository.GetByOrderCodeAsync(verifiedData.Data.OrderCode);
                if (order != null)
                {
                    if (order.Status == "PENDING")
                    {
                        order.Status = "PAID";
                        order.PaidAt = DateTime.UtcNow;
                        // TODO: Create Enrollment access for user
                        // We will add Enrollment logic soon or here directly if EnrollmentRepository exists.
                        // Ideally trigger a service method: _enrollmentService.EnrollUser(order.UserId, order.CourseId);
                        
                        await _orderRepository.UpdateAsync(order);
                    }
                }
            }
            
            return Ok(new { message = "Webhook received" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class CreatePaymentRequest
{
    public Guid CourseId { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}
