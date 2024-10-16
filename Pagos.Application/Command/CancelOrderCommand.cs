using MediatR;
using AutoMapper;
using Pagos.Application.Command.Dto;
using Pagos.Domain.interfaces.repositories;
using Pagos.Domain.Entity;

namespace Pagos.Application.Command;

public class CancelOrderCommand : IRequest<string?>
{
    public string Id { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public CancelOrderCommand() { }

    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, string?>
    {
        private readonly IPaymentProvider paymentProvider;

        public CancelOrderCommandHandler(IPaymentProvider _paymentProvider)
        {
            paymentProvider = _paymentProvider;
        }

        public async Task<string?> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
        {
            var response = await paymentProvider.CancelOrder(command.Id);
            return response;
        }
    }
}
