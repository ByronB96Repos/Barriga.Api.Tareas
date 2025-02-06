using Barriga.Api.Tareas.Models;
using Microsoft.EntityFrameworkCore;

namespace Barriga.Api.Tareas.Data
{
    public class ApplicactionDBContext: DbContext
    {
        public ApplicactionDBContext(DbContextOptions<ApplicactionDBContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
    }
}
