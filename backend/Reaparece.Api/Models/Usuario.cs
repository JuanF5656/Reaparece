namespace Reaparece.Api.Models;

public class Usuario
{
    public int Id { get; init; }
    public required string NombreCompleto { get; init; }
    public required string Correo { get; init; }
    public required string ContrasenaHash { get; init; }
    public int ComunidadPrincipalId { get; init; }
    public List<int> ComunidadIds { get; init; } = [];

    // Actividad del usuario (por ahora fija, más adelante saldrá de los objetos publicados).
    public double Reputacion { get; set; }
    public int ObjetosPublicados { get; set; }
    public int ObjetosRecuperados { get; set; }
    public int ObjetosReclamados { get; set; }
}
