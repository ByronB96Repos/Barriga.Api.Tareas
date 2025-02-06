namespace Barriga.Api.Tareas.ModelsDTO
{
    public class TareaEdicionDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string? Observacion { get; set; }
        public string Estado { get; set; }

    }
}
