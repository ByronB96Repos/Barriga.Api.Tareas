﻿namespace Barriga.Api.Tareas.ModelsDTO
{
    public class TareaRegistroDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string? Observacion { get; set; }
        public int UsuarioId { get; set; }
    }
}
