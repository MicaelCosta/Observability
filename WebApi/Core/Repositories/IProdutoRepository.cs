using Core.Entities;

namespace Core.Repositories
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> GetAllAsync();
    }
}
