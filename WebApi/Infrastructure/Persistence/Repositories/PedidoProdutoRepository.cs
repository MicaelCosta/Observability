using Core.Entities;
using Core.Repositories;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class PedidoProdutoRepository : IPedidoProdutoRepository
    {
        private readonly ContextCore _ctx;

        public PedidoProdutoRepository(ContextCore ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(List<PedidoProduto> listPedidoProduto)
        {
            await _ctx.PedidoProduto.AddRangeAsync(listPedidoProduto);
        }
    }
}
