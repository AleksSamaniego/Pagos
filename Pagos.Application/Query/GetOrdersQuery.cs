using MediatR;
using AutoMapper;
using Pagos.Application.Query.Dto;
using Pagos.Domain.interfaces.repositories;

namespace Pagos.Application.Query;

public class GetOrdersQuery : IRequest<List<GetOrderDto>>
{
    public string? Provider { get; set; }
    public GetOrdersQuery() { }

    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<GetOrderDto>>
    {
        private readonly IPaymentProvider paymentProvider;
        private readonly IMapper mapper;
        public GetOrdersQueryHandler(IPaymentProvider _paymentProvider, IMapper _mapper)
        {
            paymentProvider = _paymentProvider;
            mapper = _mapper;
        }
        public async Task<List<GetOrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var model = await paymentProvider.GetOrders(request.Provider);
            var response = mapper.Map<List<GetOrderDto>>(model);
            return response;
        }
    }
}
