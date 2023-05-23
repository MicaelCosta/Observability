using Core.Entities;

namespace Core.Repositories
{
    public interface IPedidoProdutoRepository
    {
        Task AddAsync(List<PedidoProduto> listPedidoProduto);
    }
}
