using Barriga.Api.Tareas.Data;
using Barriga.Api.Tareas.Models;
using Barriga.Api.Tareas.ModelsDTO;
using Barriga.Api.Tareas.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Barriga.Api.Tareas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ApplicactionDBContext _context;

        public AuthController(AuthService authService, ApplicactionDBContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDTO usuario)
        {
            var usuariodto = new Usuario
            {
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                FechaCreacion = DateTime.Now,
                Id = 0,
                Nombre = usuario.Nombre,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash),
                Tareas = [],
                User = usuario.User
            };
            _context.Add(usuariodto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuariodto.Id }, usuariodto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDTO usuario)
        {
            var token = await _authService.Login(usuario.Email, usuario.PasswordHash);
            if (token == null)
                return Unauthorized("Credenciales incorrectas");

            return Ok(new { token });
        }
    }
}
