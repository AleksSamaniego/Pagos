using Pagos.Domain.Entity;
using Pagos.Domain.Enums;

namespace Pagos.Application.Query.Dto;

public class GetOrderDto
{
    public string orderId { get; set; }
    public decimal amount { get; set; }
    public string status { get; set; }
    public string method { get; set; }
    public List<Fee> fees { get; set; }
    public List<Product> products { get; set; }
}
