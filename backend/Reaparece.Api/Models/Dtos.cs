namespace Reaparece.Api.Models;

public record RegistroRequest(
    string? NombreCompleto,
    string? Correo,
    string? Contrasena,
    string? ConfirmarContrasena,
    string? ComunidadPrincipal,
    bool AceptaHabeasData);

public record UsuarioResponse(int Id, string NombreCompleto, string Correo)
{
    public static UsuarioResponse Desde(Usuario u) => new(u.Id, u.NombreCompleto, u.Correo);
}

public record SesionResponse(string Token, DateTime ExpiraEn, UsuarioResponse Usuario);

public record ErrorResponse(string Mensaje);
