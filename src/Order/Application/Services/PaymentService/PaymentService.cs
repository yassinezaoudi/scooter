using Order.Application.Services.PaymentService.ViewModels;
using Order.Database;

namespace Order.Application.Services.PaymentService;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public PaymentService(AppDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }
    public async Task<PaymentResultDto> HoldMoney(int sum, Guid userId)
    {
        int maxPaymentId = 0;
        if (_context.Rents.Count() > 0)
        {
            maxPaymentId = _context.Rents.Max(r => r.PaymentId);
        }
        // TODO: request to payment service
        return new PaymentResultDto
        {
            PaymentId = maxPaymentId + 1,
            IsSuccess = true
        };
    }
    public async Task<bool> UnHoldMoney(int paymentId)
    {
        // TODO: request to payment service
        return true;
    }

    public async Task<bool> WithdrawFunds(int paymentId, int finalCost)
    {
        // TODO: request to payment service
        return true;
    }
}