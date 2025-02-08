using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Barriga.Api.Tareas.Data;
using Barriga.Api.Tareas.Models;
using Barriga.Api.Tareas.ModelsDTO;
using Microsoft.AspNetCore.Authorization;

namespace Barriga.Api.Tareas.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly ApplicactionDBContext _context;

        public TareasController(ApplicactionDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas()
        {
            return await _context.Tareas.ToListAsync();
        }

        [HttpGet("T/{id}")]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareasUser(int id)
        {
            var tareas = await _context.Tareas.Where(opt=>opt.UsuarioId == id).ToListAsync();
            if (tareas == null || !tareas.Any())
            {
                return NotFound();
            }
            return tareas;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound();
            }

            return tarea;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarea(int id, TareaEdicionDTO tareadto)
        {
            if (id != tareadto.Id)
            {
                return BadRequest();
            }

            var tareaobj = await _context.Tareas.FindAsync(id);

            if (tareaobj == null)
            {
                return NotFound();
            }

            tareaobj.Nombre = tareadto.Nombre;
            tareaobj.Descripcion = tareadto.Descripcion;
            tareaobj.Observacion = tareadto.Observacion;
            tareaobj.Estado = tareadto.Estado;
            tareaobj.FechaEdicion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Error al actualizar la tarea.");
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<Tarea>> PostTarea(TareaRegistroDTO tareadto)
        {
            var tarea = new Tarea { 
                Id = 0,
                Nombre = tareadto.Nombre,
                Descripcion = tareadto.Descripcion,
                Estado = "Incompleto",
                FechaCreacion = DateTime.Now,
                FechaEdicion = DateTime.Now,
                Observacion = tareadto.Observacion,
                UsuarioId = tareadto.UsuarioId
            };

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTarea", new { id = tarea.Id }, tarea);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch()]
        public async Task<IActionResult> MarcarTareasComoCompletadas([FromBody] List<int> tareaIds)
        {
            if (tareaIds == null || !tareaIds.Any())
            {
                return BadRequest("No se han proporcionado IDs de tareas.");
            }

            var tareas = await _context.Tareas.Where(t => tareaIds.Contains(t.Id)).ToListAsync();

            if (tareas.Count != tareaIds.Count)
            {
                return NotFound("Algunas tareas no fueron encontradas.");
            }

            foreach (var tarea in tareas)
            {
                tarea.Estado = "Completada";
                tarea.FechaEdicion = DateTime.Now;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Error al actualizar las tareas.");
            }

            return NoContent();
        }


        private bool TareaExists(int id)
        {
            return _context.Tareas.Any(e => e.Id == id);
        }
    }
}
