using MediatR;
using AutoMapper;
using Pagos.Application.Command.Dto;
using Pagos.Domain.interfaces.repositories;
using Pagos.Domain.Entity;

namespace Pagos.Application.Command;

public class PayOrderCommand : IRequest<string>
{
    public string Id { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public PayOrderCommand() { }

    public class PayOrderCommandHandler : IRequestHandler<PayOrderCommand, string>
    {
        private readonly IPaymentProvider paymentProvider;
        public PayOrderCommandHandler(IPaymentProvider _paymentProvider)
        {
            paymentProvider = _paymentProvider;
        }

        public async Task<string> Handle(PayOrderCommand command, CancellationToken cancellationToken)
        {
            var response = await paymentProvider.PayOrder(command.Id);
            return response;
        }
    }
}
