using Application.ViewModels;
using Core.Entities;
using Core.Repositories;
using MediatR;

namespace Application.Queries.GetAllProduto
{
    public class GetAllProdutoQueryHandler : IRequestHandler<GetAllProdutoQuery, List<ProdutoViewModel>>
    {
        private readonly IProdutoRepository _produtoRepository;

        public GetAllProdutoQueryHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<List<ProdutoViewModel>> Handle(GetAllProdutoQuery request, CancellationToken cancellationToken)
        {
            var listProduto = await _produtoRepository.GetAllAsync();

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
