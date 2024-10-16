using MediatR;
using AutoMapper;
using Pagos.Application.Command.Dto;
using Pagos.Domain.interfaces.repositories;
using Pagos.Domain.Entity;
using Pagos.Domain.Enums;

namespace Pagos.Application.Command;

public class CreateOrderCommand : IRequest<CreateDto>
{
    public PaymentMethod Method { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();

    public CreateOrderCommand() { }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateDto>
    {
        private readonly IPaymentProvider paymentProvider;
        private readonly IMapper mapper;
        public CreateOrderCommandHandler(IPaymentProvider _paymentProvider, IMapper _mapper)
        {
            paymentProvider = _paymentProvider;
            mapper = _mapper;
        }

        public async Task<CreateDto> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var response = await paymentProvider.CreateOrder(command.Method, command.Products);
            var result = mapper.Map<CreateDto>(response);
            return result;
        }
    }
}
