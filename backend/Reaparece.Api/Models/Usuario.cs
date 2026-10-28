namespace Reaparece.Api.Models;

public class Usuario
{
    public int Id { get; init; }
    public required string NombreCompleto { get; init; }
    public required string Correo { get; init; }
    public required string ContrasenaHash { get; init; }
    public int ComunidadPrincipalId { get; init; }
    public List<int> ComunidadIds { get; init; } = [];
}
