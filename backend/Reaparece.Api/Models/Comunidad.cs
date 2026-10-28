namespace Reaparece.Api.Models;

public class Comunidad
{
    public int Id { get; init; }
    public required string Nombre { get; init; }
    public required string Categoria { get; init; }
    public bool Verificada { get; init; }

    // Comunidades precargadas mientras no hay base de datos.
    public static readonly IReadOnlyList<Comunidad> Todas =
    [
        new() { Id = 1, Nombre = "Universidad EIA", Categoria = "Universidad", Verificada = true },
        new() { Id = 2, Nombre = "Metro de Medellín", Categoria = "Transporte", Verificada = true },
        new() { Id = 3, Nombre = "Transmilenio Bogotá", Categoria = "Transporte", Verificada = true },
        new() { Id = 4, Nombre = "Barrio El Poblado", Categoria = "Barrio", Verificada = false },
    ];

    public static Comunidad? BuscarPorNombre(string nombre) =>
        Todas.FirstOrDefault(c => c.Nombre == nombre);
}
