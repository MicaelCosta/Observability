using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence.Context
{
    public class ContextCore : DbContext
    {
        public ContextCore(DbContextOptions<ContextCore> options) : base(options)
        {

        }

        public DbSet<Produto> Produto { get; set; }

        public DbSet<Pedido> Pedido { get; set; }

        public DbSet<PedidoProduto> PedidoProduto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IEnumerable<Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
                    .SelectMany(t => t.GetForeignKeys()).Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
