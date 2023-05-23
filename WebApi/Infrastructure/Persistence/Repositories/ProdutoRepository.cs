using Core.Entities;
using Core.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ContextCore _ctx;

        public ProdutoRepository(ContextCore ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<Produto>> GetAllAsync()
        {
            return await _ctx.Produto.ToListAsync();
        }
    }
}
