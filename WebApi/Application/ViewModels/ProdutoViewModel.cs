namespace Application.ViewModels
{
    public class ProdutoViewModel
    {
        public long Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public string Image { get; set; } = string.Empty;
    }
}
