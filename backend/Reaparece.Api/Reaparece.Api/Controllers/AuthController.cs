
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reaparece.Api.Data;
using Reaparece.Api.DTOs;
using Reaparece.Api.Services;

namespace Reaparece.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const int MaxIntentos = 3;
    private const int MinutosBloqueo = 15;

    private const string ErrorCredenciales =
        "Correo o contraseña incorrectos";

    private readonly ReapareceDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(
        ReapareceDbContext db,
        TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var correo = request.Correo.Trim().ToLowerInvariant();

        var usuario = await _db.Usuarios
            .SingleOrDefaultAsync(u => u.Correo == correo);

        // No revelar si el correo existe.
        if (usuario is null)
        {
            return Unauthorized(new
            {
                mensaje = ErrorCredenciales
            });
        }

        var ahora = DateTime.UtcNow;

        // Si sigue bloqueado, no aceptar intentos.
        if (usuario.BloqueadoHastaUtc.HasValue)
        {
            if (usuario.BloqueadoHastaUtc.Value > ahora)
            {
                return StatusCode(423, new
                {
                    mensaje =
                        "Cuenta bloqueada temporalmente. Intenta más tarde."
                });
            }

            // El bloqueo expiró: reiniciar el contador.
            usuario.BloqueadoHastaUtc = null;
            usuario.IntentosFallidos = 0;
        }

        bool contrasenaCorrecta;

        try
        {
            contrasenaCorrecta = BCrypt.Net.BCrypt.Verify(
                request.Contrasena,
                usuario.PasswordHash);
        }
        catch (ArgumentException)
        {
            // Un hash almacenado con formato inválido
            // es un problema de datos, no una contraseña válida.
            return StatusCode(500, new
            {
                mensaje = "No fue posible validar la cuenta."
            });
        }

        if (!contrasenaCorrecta)
        {
            usuario.IntentosFallidos++;

            if (usuario.IntentosFallidos >= MaxIntentos)
            {
                usuario.BloqueadoHastaUtc =
                    ahora.AddMinutes(MinutosBloqueo);

                await _db.SaveChangesAsync();

                return StatusCode(423, new
                {
                    mensaje =
                        "Cuenta bloqueada temporalmente. Intenta más tarde."
                });
            }

            await _db.SaveChangesAsync();

            return Unauthorized(new
            {
                mensaje = ErrorCredenciales
            });
        }

        // Un inicio de sesión correcto reinicia los fallos.
        usuario.IntentosFallidos = 0;
        usuario.BloqueadoHastaUtc = null;

        if (!usuario.ComunidadPrincipalId.HasValue)
        {
            await _db.SaveChangesAsync();

            return Conflict(new
            {
                mensaje =
                    "La cuenta no tiene una comunidad principal configurada."
            });
        }

        await _db.SaveChangesAsync();

        var resultado = _tokenService.CrearToken(usuario);

        return Ok(new LoginResponse
        {
            Token = resultado.Token,
            ExpiraEnUtc = resultado.ExpiraEnUtc,
            ComunidadPrincipalId = usuario.ComunidadPrincipalId.Value
        });
    }
}

