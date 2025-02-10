using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Barriga.Api.Tareas.Controllers;
using Barriga.Api.Tareas.Data;
using Barriga.Api.Tareas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barriga.Api.Tareas.ModelsDTO;

public class TareasControllerTests
{
    private readonly ApplicactionDBContext _context;
    private readonly TareasController _controller;

    public TareasControllerTests()
    {
        // Usar la base de datos en memoria para las pruebas
        var options = new DbContextOptionsBuilder<ApplicactionDBContext>()
                      .UseInMemoryDatabase(databaseName: "TestTareasDb")
                      .Options;

        _context = new ApplicactionDBContext(options);
        _controller = new TareasController(_context);

        // Agregar datos de prueba a la base de datos en memoria
        if (!_context.Tareas.Any())
        {
            _context.Tareas.AddRange(new List<Tarea>
            {
                new Tarea { Id = 1, Nombre = "Tarea 1", Descripcion = "Descripcion 1", Estado = "Incompleto", FechaCreacion = DateTime.Now, FechaEdicion = DateTime.Now ,Observacion="" ,UsuarioId = 1},
                new Tarea { Id = 2, Nombre = "Tarea 2", Descripcion = "Descripcion 2", Estado = "Incompleto", FechaCreacion = DateTime.Now, FechaEdicion = DateTime.Now ,Observacion="" , UsuarioId = 1}
            });
            _context.SaveChanges();
        }
    }

    [Fact]
    public async Task GetTareas_ReturnsOkResult_WithTareas()
    {
        var result = await _controller.GetTareas();
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Tarea>>>(result); 
        var tareas = Assert.IsType<List<Tarea>>(actionResult.Value); 
        Assert.NotEmpty(tareas); 
    }



    // Prueba para obtener una tarea por ID (cuando no existe)
    [Fact]
    public async Task GetTarea_ReturnsNotFound_WhenTareaDoesNotExist()
    {
        var result = await _controller.GetTarea(99); 
        var actionResult = Assert.IsType<ActionResult<Tarea>>(result); 
        Assert.IsType<NotFoundResult>(actionResult.Result); 
    }



    // Prueba para crear una tarea
    [Fact]
    public async Task PostTarea_ReturnsCreatedAtAction_WhenTareaIsCreated()
    {
        var tareaDto = new TareaRegistroDTO
        {
            Nombre = "Tarea 3",
            Descripcion = "Descripcion 3",
            Observacion = "Incompleto",
            UsuarioId = 1
        };

        var result = await _controller.PostTarea(tareaDto); 
        var actionResult = Assert.IsType<ActionResult<Tarea>>(result); 
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result); 
        var createdTarea = Assert.IsType<Tarea>(createdAtActionResult.Value); 
        Assert.Equal("Tarea 3", createdTarea.Nombre); 
    }



    // Prueba para actualizar una tarea
    [Fact]
    public async Task PutTarea_ReturnsNoContent_WhenTareaIsUpdated()
    {
        var tareaExistente = await _context.Tareas.FindAsync(1);
        if (tareaExistente == null)
        {
            _context.Tareas.Add(new Tarea
            {
                Id = 1,
                Nombre = "Tarea 1",
                Descripcion = "Descripcion 1",
                Estado = "Incompleto",
                FechaCreacion = DateTime.Now,
                FechaEdicion = DateTime.Now,
                Observacion = "",
                UsuarioId = 1
            });
            await _context.SaveChangesAsync();
        }

        var tareadto = new TareaEdicionDTO
        {
            Id = 1,
            Nombre = "Tarea 1 Actualizada",
            Descripcion = "Descripcion Actualizada",
            Estado = "Completa",
            Observacion = "Actualizada"
        };

        var result = await _controller.PutTarea(1, tareadto); 
        Assert.IsType<NoContentResult>(result); 
    }


    // Prueba para eliminar una tarea
    [Fact]
    public async Task DeleteTarea_ReturnsNoContent_WhenTareaIsDeleted()
    {
        var result = await _controller.DeleteTarea(1); 
        Assert.IsType<NoContentResult>(result); 
    }
}
