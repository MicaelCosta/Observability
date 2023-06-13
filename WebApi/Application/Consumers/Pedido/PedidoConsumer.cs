using Application.ViewModels;
using Core.Caching;
using Core.Entities;
using Core.Repositories;
using MassTransit;

namespace Application.Consumers
{
    public class PedidoConsumer : IConsumer<PedidoViewModel>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICachingService _cache;

        public PedidoConsumer(IPedidoRepository pedidoRepository,
                              ICachingService cache)
        {
            _pedidoRepository = pedidoRepository;
            _cache = cache;
        }

        public async Task Consume(ConsumeContext<PedidoViewModel> context)
        {
            Pedido pedido = await _pedidoRepository.GetByIdAsync(context.Message.Id);
            if (pedido == null) return;

            pedido.DataNotificacao = DateTime.Now;
            _pedidoRepository.Update(pedido);

            await _pedidoRepository.SaveChangesAsync();

            await _cache.RemoveAsync("ProdutoAll");
        }
    }
}
