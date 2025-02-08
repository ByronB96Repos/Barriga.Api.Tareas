using Barriga.Api.Tareas.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Barriga.Api.Tareas.Data;
using Microsoft.EntityFrameworkCore;

namespace Barriga.Api.Tareas.Services
{
    public class AuthService
    {
        private readonly ApplicactionDBContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicactionDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string?> Login(string email, string password)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
                return null;

            return GenerarToken(usuario);
        }

        private string GenerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                 new Claim(ClaimTypes.Email, usuario.Email)
            };

            // Fecha actual en UTC (de creación)
            var creationDate = DateTime.UtcNow;
            // Fecha de expiración (2 horas después)
            var expirationDate = creationDate.AddHours(2);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: expirationDate,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return $"{tokenString}";
        }

    }
}
