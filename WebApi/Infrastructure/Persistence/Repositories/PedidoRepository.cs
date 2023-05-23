using Core.Entities;
using Core.Repositories;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ContextCore _ctx;

        public PedidoRepository(ContextCore ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Pedido pedido)
        {
            await _ctx.Pedido.AddAsync(pedido);
        }

        public async Task SaveChangesAsync()
        {
            await _ctx.SaveChangesAsync();
        }
    }
}
