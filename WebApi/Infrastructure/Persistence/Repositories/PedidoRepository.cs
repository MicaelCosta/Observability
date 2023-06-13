using Core.Entities;
using Core.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Pedido> GetByIdAsync(long id)
        {
            return await _ctx.Pedido.FirstOrDefaultAsync(a => a.Id == id);
        }

        public void Update(Pedido pedido)
        {
            _ctx.Pedido.Update(pedido);
        }
    }
}
