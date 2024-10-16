using AutoMapper;
using Pagos.Application.Command.Dto;
using Pagos.Application.Query.Dto;
using Pagos.Domain.Entity;

namespace Pagos.Application.Mapping;

public class PagosDtoMapper : Profile
{
    public PagosDtoMapper()
    {
        CreateMap<Order, GetOrderDto>();
        CreateMap<Order, CreateDto>();
    }
}
