using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    [Table("Pedido")]
    public class Pedido
    {
        [Key]
        public long Id { get; private set; }

        public decimal ValorTotal  { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime? DataNotificacao { get; set; }
    }
}
