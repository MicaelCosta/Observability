using Application.Queries.GetAllProduto;
using Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProdutoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            GetAllProdutoQuery getAllProdutoQuery = new();

            List<ProdutoViewModel> listProduto = await _mediator.Send(getAllProdutoQuery);

            return Ok(listProduto);
        }
    }
}
