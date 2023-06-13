using Application.InputModels;
using Application.ViewModels;
using Core.Entities;
using Core.Repositories;
using MassTransit;
using MediatR;

namespace Application.Commands.CreatePedido
{
    public class CreatePedidoCommandHandler : IRequestHandler<CreatePedidoCommand, long>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IPedidoProdutoRepository _pedidoProdutoRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreatePedidoCommandHandler(IPedidoRepository pedidoRepository,
                                          IPedidoProdutoRepository pedidoProdutoRepository,
                                          IPublishEndpoint publishEndpoint)
        {
            _pedidoRepository = pedidoRepository;
            _pedidoProdutoRepository = pedidoProdutoRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<long> Handle(CreatePedidoCommand request, CancellationToken cancellationToken)
        {
            Pedido pedido = Map(request.ListProduto);

            await _pedidoRepository.AddAsync(pedido);

            await _pedidoProdutoRepository.AddAsync(Map(pedido.Id, request.ListProduto));

            await _pedidoRepository.SaveChangesAsync();

            await _publishEndpoint.Publish(Map(pedido.Id));

            return pedido.Id;
        }

        private Pedido Map(List<ProdutoInputModel> listProduto)
        {
            decimal valorTotal = listProduto.Sum(a => a.Quantidade * a.Preco);

            return new Pedido()
            {
                ValorTotal = valorTotal,
                DataInclusao = DateTime.Now
            };
        }

        private List<PedidoProduto> Map(long idPedido, List<ProdutoInputModel> listProduto)
        {
            return listProduto.Select(a => new PedidoProduto()
            {
                IdPedido = idPedido,
                IdProduto = a.Id,
                Quantidade = a.Quantidade,
                PrecoUnitario = a.Preco,
                PrecoTotal = a.Quantidade * a.Preco
            })
            .ToList();
        }

        private PedidoViewModel Map(long id)
        {
            return new PedidoViewModel()
            {
                Id = id
            };
        }
    }
}
