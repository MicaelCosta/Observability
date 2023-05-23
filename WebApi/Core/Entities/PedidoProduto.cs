using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    [Table("PedidoProduto")]
    public class PedidoProduto
    {
        [Key]
        public long Id { get; set; }

        public long IdPedido { get; set; }

        public long IdProduto { get; set; }

        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }

        public decimal PrecoTotal { get; set; }

        [ForeignKey("IdPedido")]
        public virtual Pedido Pedido { get; set; } = new();

        [ForeignKey("IdProduto")]
        public virtual Produto Produto { get; set; } = new();
    }
}
