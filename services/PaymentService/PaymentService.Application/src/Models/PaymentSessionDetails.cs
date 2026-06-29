namespace PaymentService.Application.src.Models;

public record PaymentSessionDetails(string OrderId, string UserId, long Amount, bool IsPaid);