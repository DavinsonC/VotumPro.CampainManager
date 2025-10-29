using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotumPro.CampainManeger.Application.Interface;
using VotumPro.CampainManeger.Domain.DTOs.Auth;
using VotumPro.CampainManeger.Domain.Models;
using VotumPro.CampainManeger.Infraestructure.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace VotumPro.CampainManeger.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly VotumProDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;

        public AuthController(VotumProDbContext context, ITokenService tokenService, IConfiguration config)
        {
            _context = context;
            _tokenService = tokenService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                    .ThenInclude(r => r.RolMenus)
                        .ThenInclude(rm => rm.Menu)
                            .ThenInclude(m => m.SubMenus)
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (user == null) return Unauthorized(new { message = "Credenciales inválidas" });
            if (!BCrypt.Net.BCrypt.Verify(dto.Contrasena, user.ContrasenaHash))
                return Unauthorized(new { message = "Credenciales inválidas" });

            var token = _tokenService.GenerateToken(user);

            var menu = user.Rol?.RolMenus
                .Where(rm => rm.Menu.EsActivo && rm.Menu.IdMenuPadre == null)
                .Select(rm => new
                {
                    rm.Menu.IdMenu,
                    rm.Menu.Nombre,
                    rm.Menu.Ruta,
                    rm.Menu.Icono,
                    SubMenus = rm.Menu.SubMenus
                        .Where(sm => sm.EsActivo)
                        .Select(sm => new
                        {
                            sm.IdMenu,
                            sm.Nombre,
                            sm.Ruta,
                            sm.Icono
                        })
                        .ToList()
                })
                .ToList();

            return Ok(new
            {
                token,
                user = new
                {
                    user.IdUsuario,
                    user.Nombre,
                    user.Correo,
                    Rol = user.Rol?.NombreRol
                },
                menu
            });
        }

        // Endpoint para registrarse (opcional)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest(new { message = "El correo ya está en uso" });

            var user = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Cedula = dto.Cedula,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                IdRol = dto.Rol,           // Asignas el ID del rol
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                CreatedAt = DateTime.UtcNow
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { id = user.IdUsuario }, new { user.IdUsuario, user.Correo });
        }
    }
}

