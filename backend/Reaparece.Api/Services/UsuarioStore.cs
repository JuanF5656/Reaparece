using Reaparece.Api.Models;

namespace Reaparece.Api.Services;

public enum ResultadoLogin { Exitoso, CredencialesInvalidas, Bloqueado }

/// <summary>
/// Almacén de usuarios en memoria. Se registra como singleton y se reemplazará por PostgreSQL.
/// </summary>
public class UsuarioStore
{
    public const int MaxIntentosFallidos = 3;
    public static readonly TimeSpan DuracionBloqueo = TimeSpan.FromMinutes(15);

    private readonly List<Usuario> _usuarios = [];
    // Los intentos se cuentan por correo aunque el correo no exista, para no revelar qué cuentas están registradas.
    private readonly Dictionary<string, (int Fallidos, DateTime? BloqueadoHasta)> _intentos = [];
    private readonly Lock _lock = new();
    private readonly TimeProvider _reloj;
    private int _siguienteId = 1;

    public UsuarioStore(TimeProvider reloj)
    {
        _reloj = reloj;

        var prueba = Crear("Jerónimo Tavera", "jeronimo@eia.edu.co", "Reaparece123", comunidadPrincipalId: 1);
        prueba.ComunidadIds.Add(2);
        prueba.Reputacion = 4.6;
        prueba.ObjetosPublicados = 7;
        prueba.ObjetosRecuperados = 4;
        prueba.ObjetosReclamados = 3;
    }

    public static string NormalizarCorreo(string correo) => correo.Trim().ToLowerInvariant();

    public Usuario? BuscarPorCorreo(string correo)
    {
        var normalizado = NormalizarCorreo(correo);
        lock (_lock)
        {
            return _usuarios.FirstOrDefault(u => u.Correo == normalizado);
        }
    }

    public Usuario? BuscarPorId(int id)
    {
        lock (_lock)
        {
            return _usuarios.FirstOrDefault(u => u.Id == id);
        }
    }

    /// <summary>Crea el usuario o devuelve null si el correo ya tiene una cuenta.</summary>
    public Usuario? TryCrear(string nombreCompleto, string correo, string contrasena, int comunidadPrincipalId)
    {
        lock (_lock)
        {
            if (_usuarios.Any(u => u.Correo == NormalizarCorreo(correo)))
                return null;

            return Crear(nombreCompleto, correo, contrasena, comunidadPrincipalId);
        }
    }

    public (ResultadoLogin Resultado, Usuario? Usuario, DateTime? BloqueadoHasta) ValidarCredenciales(string correo, string contrasena)
    {
        var normalizado = NormalizarCorreo(correo);
        var ahora = _reloj.GetUtcNow().UtcDateTime;

        lock (_lock)
        {
            _intentos.TryGetValue(normalizado, out var intento);

            if (intento.BloqueadoHasta is { } hasta)
            {
                if (hasta > ahora)
                    return (ResultadoLogin.Bloqueado, null, hasta);

                // El bloqueo expiró: se empieza de cero.
                intento = (0, null);
            }

            var usuario = _usuarios.FirstOrDefault(u => u.Correo == normalizado);
            if (usuario is not null && BCrypt.Net.BCrypt.Verify(contrasena, usuario.ContrasenaHash))
            {
                _intentos.Remove(normalizado);
                return (ResultadoLogin.Exitoso, usuario, null);
            }

            var fallidos = intento.Fallidos + 1;
            DateTime? bloqueadoHasta = fallidos >= MaxIntentosFallidos ? ahora + DuracionBloqueo : null;
            _intentos[normalizado] = (fallidos, bloqueadoHasta);
            return (ResultadoLogin.CredencialesInvalidas, null, null);
        }
    }

    private Usuario Crear(string nombreCompleto, string correo, string contrasena, int comunidadPrincipalId)
    {
        var usuario = new Usuario
        {
            Id = _siguienteId++,
            NombreCompleto = nombreCompleto.Trim(),
            Correo = NormalizarCorreo(correo),
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(contrasena),
            ComunidadPrincipalId = comunidadPrincipalId,
            ComunidadIds = [comunidadPrincipalId],
        };
        _usuarios.Add(usuario);
        return usuario;
    }
}
