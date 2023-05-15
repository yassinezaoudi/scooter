using Order.Application.Services.PaymentService.ViewModels;

namespace Order.Application.Services.PaymentService;

public interface IPaymentService
{
    Task<PaymentResultDto> HoldMoney(int sum, Guid userId);
    Task<bool> UnHoldMoney(int paymentId);
    Task<bool> WithdrawFunds(int paymentId, int finalCost);
}