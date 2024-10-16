using Pagos.Domain.Entity;
using Pagos.Domain.Enums;

namespace Pagos.Domain.interfaces.repositories;

public interface IPaymentProvider
{
    Task<List<Order>> GetOrders(string? provider);
    Task<Order?> GetOrderById(string id);
    Task<Order> CreateOrder(PaymentMethod method, List<Product> products);
    Task<string> CancelOrder(string id);
    Task<string> PayOrder(string id);
}
