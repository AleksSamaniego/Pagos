using MediatR;
using AutoMapper;
using Pagos.Application.Query.Dto;
using Pagos.Domain.interfaces.repositories;

namespace Pagos.Application.Query;

public class GetOrderQuery : IRequest<GetOrderDto>
{
    public string Id { get; set; } = string.Empty;
    public GetOrderQuery() { }

    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderDto>
    {
        private readonly IPaymentProvider paymentProvider;
        private readonly IMapper mapper;

        public GetOrderQueryHandler(IPaymentProvider _paymentProvider, IMapper _mapper)
        {
            paymentProvider = _paymentProvider;
            mapper = _mapper;
        }

        public async Task<GetOrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var model = await paymentProvider.GetOrderById(request.Id);
            var response = mapper.Map<GetOrderDto>(model);
            return response;
        }
    }
}
