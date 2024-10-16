using Microsoft.Extensions.DependencyInjection;
using Pagos.Domain.interfaces.repositories;
using Pagos.Infrastructure.Data;

namespace Pagos.Infrastructure;

public static class ServiceInjector
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPaymentProvider, PaymentProvider>();
        return services;
    }
}
