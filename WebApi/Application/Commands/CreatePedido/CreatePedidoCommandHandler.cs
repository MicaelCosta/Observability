﻿using Application.InputModels;
using Core.Entities;
using Core.Repositories;
using MediatR;

namespace Application.Commands.CreatePedido
{
    public class CreatePedidoCommandHandler : IRequestHandler<CreatePedidoCommand, long>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IPedidoProdutoRepository _pedidoProdutoRepository;

        public CreatePedidoCommandHandler(IPedidoRepository pedidoRepository,
                                          IPedidoProdutoRepository pedidoProdutoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _pedidoProdutoRepository = pedidoProdutoRepository;
        }

        public async Task<long> Handle(CreatePedidoCommand request, CancellationToken cancellationToken)
        {
            Pedido pedido = Map(request.ListProduto);

            await _pedidoRepository.AddAsync(pedido);

            await _pedidoProdutoRepository.AddAsync(Map(pedido.Id, request.ListProduto));

            await _pedidoRepository.SaveChangesAsync();

            return pedido.Id;
        }

        private Pedido Map(List<ProdutoInputModel> listProduto)
        {
            decimal valorTotal = listProduto.Sum(a => a.Quantidade * a.Preco);

            return new Pedido()
            {
                ValorTotal = valorTotal,
                DataInclusao = new DateTime()
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
    }
}