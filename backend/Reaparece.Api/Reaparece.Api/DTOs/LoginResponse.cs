
namespace Reaparece.Api.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEnUtc { get; set; }

    public int ComunidadPrincipalId { get; set; }
}

