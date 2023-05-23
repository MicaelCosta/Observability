using Application.ViewModels;
using MediatR;

namespace Application.Queries.GetAllProduto
{
    public class GetAllProdutoQuery : IRequest<List<ProdutoViewModel>>
    {

    }
}
