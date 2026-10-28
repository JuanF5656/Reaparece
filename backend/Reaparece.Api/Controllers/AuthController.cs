using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Reaparece.Api.Models;
using Reaparece.Api.Services;

namespace Reaparece.Api.Controllers;

[ApiController]
[Route("api/auth")]
public partial class AuthController(UsuarioStore store) : ControllerBase
{
    private const int LongitudMinimaContrasena = 8;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex CorreoValido();

    [HttpPost("registro")]
    public ActionResult<SesionResponse> Registro(RegistroRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.NombreCompleto) || string.IsNullOrWhiteSpace(req.Correo) ||
            string.IsNullOrEmpty(req.Contrasena) || string.IsNullOrEmpty(req.ConfirmarContrasena) ||
            string.IsNullOrWhiteSpace(req.ComunidadPrincipal))
            return Error(StatusCodes.Status400BadRequest, "Completa todos los campos");

        if (!CorreoValido().IsMatch(req.Correo.Trim()))
            return Error(StatusCodes.Status400BadRequest, "Ingresa un correo válido");

        if (req.Contrasena.Length < LongitudMinimaContrasena)
            return Error(StatusCodes.Status400BadRequest, "La contraseña debe tener mínimo 8 caracteres");

        if (req.Contrasena != req.ConfirmarContrasena)
            return Error(StatusCodes.Status400BadRequest, "Las contraseñas no coinciden");

        var comunidad = Comunidad.BuscarPorNombre(req.ComunidadPrincipal);
        if (comunidad is null)
            return Error(StatusCodes.Status400BadRequest, "Selecciona una comunidad válida");

        if (!req.AceptaHabeasData)
            return Error(StatusCodes.Status400BadRequest, "Debes aceptar el tratamiento de datos personales para continuar");

        var usuario = store.TryCrear(req.NombreCompleto, req.Correo, req.Contrasena, comunidad.Id);
        if (usuario is null)
            return Error(StatusCodes.Status409Conflict, "Este correo ya tiene una cuenta");

        var (token, expiraEn) = store.GenerarToken(usuario);
        return Ok(new SesionResponse(token, expiraEn, UsuarioResponse.Desde(usuario)));
    }

    private ObjectResult Error(int status, string mensaje) => StatusCode(status, new ErrorResponse(mensaje));
}
