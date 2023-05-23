using Application.Commands.CreatePedido;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/pedido")]
    public class PedidoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePedidoCommand command)
        {
            long id = await _mediator.Send(command);

            if(id <= 0)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
