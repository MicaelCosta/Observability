using Application.InputModels;
using MediatR;

namespace Application.Commands.CreatePedido
{
    public class CreatePedidoCommand : IRequest<long>
    {
        public CreatePedidoCommand(List<ProdutoInputModel> listProduto)
        {
            ListProduto = listProduto;
        }

        public List<ProdutoInputModel> ListProduto { get; set; }
    }
}
