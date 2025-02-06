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

        public AuthController(AuthService authService)
        {
            _authService = authService;
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
                PasswordHash = usuario.PasswordHash,
                Tareas = [],
                User = usuario.User
            };

            var resultado = await _authService.Registrar(usuariodto, usuariodto.PasswordHash);
            return Ok(resultado);
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
