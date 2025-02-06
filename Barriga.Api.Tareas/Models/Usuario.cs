namespace Barriga.Api.Tareas.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string User {  get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ICollection<Tarea> Tareas { get; set; }
    }
}
