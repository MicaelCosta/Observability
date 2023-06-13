using Core.Entities;

namespace Core.Repositories
{
    public interface IPedidoRepository
    {
        Task AddAsync(Pedido pedido);

        Task SaveChangesAsync();

        Task<Pedido> GetByIdAsync(long id);

        void Update(Pedido pedido);
    }
}
