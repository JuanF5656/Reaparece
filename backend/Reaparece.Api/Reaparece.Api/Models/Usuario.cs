
using System.ComponentModel.DataAnnotations;

namespace Reaparece.Api.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    [MaxLength(254)]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public int IntentosFallidos { get; set; } = 0;

    public DateTime? BloqueadoHastaUtc { get; set; }

    public int? ComunidadPrincipalId { get; set; }
}

