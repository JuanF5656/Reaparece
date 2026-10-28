
using System.ComponentModel.DataAnnotations;

namespace Reaparece.Api.DTOs;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Contrasena { get; set; } = string.Empty;
}

