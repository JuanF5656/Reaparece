namespace Reaparece.Api.Models;

public record RegistroRequest(
    string? NombreCompleto,
    string? Correo,
    string? Contrasena,
    string? ConfirmarContrasena,
    string? ComunidadPrincipal,
    bool AceptaHabeasData);

public record LoginRequest(string? Correo, string? Contrasena);

public record UsuarioResponse(int Id, string NombreCompleto, string Correo)
{
    public static UsuarioResponse Desde(Usuario u) => new(u.Id, u.NombreCompleto, u.Correo);
}

public record ComunidadResponse(int Id, string Nombre, string Categoria, bool Verificada, bool EsPrincipal);

public record PerfilResponse(
    int Id,
    string NombreCompleto,
    string Correo,
    double Reputacion,
    int ObjetosPublicados,
    int ObjetosRecuperados,
    int ObjetosReclamados,
    IReadOnlyList<ComunidadResponse> Comunidades);

public record ErrorResponse(string Mensaje);
