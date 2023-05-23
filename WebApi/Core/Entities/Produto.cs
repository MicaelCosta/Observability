using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    [Table("Produto")]
    public class Produto
    {
        [Key]
        public long Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public string Image { get; set; } = string.Empty;
    }
}
