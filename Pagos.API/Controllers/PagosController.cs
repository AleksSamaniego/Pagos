using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pagos.Application.Command;
using Pagos.Application.Query;
using System.ComponentModel.DataAnnotations;

namespace Pagos.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly IMediator mediator;
        public PagosController(IMediator _mediator)
        {
            mediator = _mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var response = await mediator.Send(new GetOrderQuery() { Id = id.ToString() });
            return response != null ? Ok(response) : NotFound();
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(string? provider)
        {
            var response = await mediator.Send(new GetOrdersQuery() { Provider = provider });
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand order)
        {
            var response = await mediator.Send(order);
            return Ok(response);
        }

        [HttpPut("pay/{id}")]
        public async Task<IActionResult> UpdateOrder(string id)
        {
            var response = await mediator.Send(new PayOrderCommand() { Id = id });
            return Ok(response);
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(string id)
        {
            var response = await mediator.Send(new CancelOrderCommand() { Id = id });
            return Ok(response);
        }
    }
}
