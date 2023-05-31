using Application.ViewModels;
using Core.Caching;
using Core.Entities;
using Core.Repositories;
using MediatR;

namespace Application.Queries.GetAllProduto
{
    public class GetAllProdutoQueryHandler : IRequestHandler<GetAllProdutoQuery, List<ProdutoViewModel>>
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly ICachingService _cache;

        public GetAllProdutoQueryHandler(IProdutoRepository produtoRepository,
                                         ICachingService cache)
        {
            _produtoRepository = produtoRepository;
            _cache = cache;
        }

        public async Task<List<ProdutoViewModel>> Handle(GetAllProdutoQuery request, CancellationToken cancellationToken)
        {
            List<Produto> listProduto;

            List<Produto> listProdutoCache = await _cache.GetAsync<List<Produto>>("ProdutoAll");
            if (listProdutoCache != null)
            {
                listProduto = listProdutoCache;
            }
            else
            {
                listProduto = await _produtoRepository.GetAllAsync();
                await _cache.SetAsync("ProdutoAll", listProduto);
            }

            return listProduto.Select(Map).ToList();
        }

        private ProdutoViewModel Map(Produto produto)
        {
            return new ProdutoViewModel()
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco,
                Image = produto.Image,
            };
        }
    }
}
