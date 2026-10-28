using System.Security.Cryptography;
using Reaparece.Api.Models;

namespace Reaparece.Api.Services;

/// <summary>
/// Almacén de usuarios y sesiones en memoria. Se registra como singleton y se reemplazará por PostgreSQL.
/// </summary>
public class UsuarioStore
{
    public static readonly TimeSpan DuracionSesion = TimeSpan.FromHours(2);

    private readonly List<Usuario> _usuarios = [];
    private readonly Dictionary<string, (int UsuarioId, DateTime ExpiraEn)> _sesiones = [];
    private readonly Lock _lock = new();
    private readonly TimeProvider _reloj;
    private int _siguienteId = 1;

    public UsuarioStore(TimeProvider reloj)
    {
        _reloj = reloj;

        // Usuario de prueba, útil para comprobar que un correo repetido no se duplica.
        Crear("Jerónimo Tavera", "jeronimo@eia.edu.co", "Reaparece123", comunidadPrincipalId: 1);
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

    public (string Token, DateTime ExpiraEn) GenerarToken(Usuario usuario)
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var expiraEn = _reloj.GetUtcNow().UtcDateTime + DuracionSesion;
        lock (_lock)
        {
            _sesiones[token] = (usuario.Id, expiraEn);
        }
        return (token, expiraEn);
    }

    /// <summary>Devuelve el usuario dueño del token, o null si no existe o ya expiró.</summary>
    public Usuario? ValidarToken(string token)
    {
        lock (_lock)
        {
            if (!_sesiones.TryGetValue(token, out var sesion))
                return null;

            if (sesion.ExpiraEn <= _reloj.GetUtcNow().UtcDateTime)
            {
                _sesiones.Remove(token);
                return null;
            }

            return _usuarios.FirstOrDefault(u => u.Id == sesion.UsuarioId);
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
