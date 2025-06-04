using ProjetoLoginRegister.Domain;
using Microsoft.EntityFrameworkCore;

namespace ProjetoLoginRegister.Api.Data
{
    public class ProjetoContext : DbContext
    {
        public ProjetoContext(DbContextOptions options) : base(options) 
        { 
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Computador> Computadores { get; set; }

    }
}
