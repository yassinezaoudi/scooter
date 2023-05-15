using Gateway.Endpoints.Order.ViewModels;

namespace Gateway.Application.Services.OrderService;

public interface IOrderService
{
    Task<IResult> StartRent(OrderViewModel viewModel);
    Task<IResult> StopRent(OrderViewModel viewModel);
}