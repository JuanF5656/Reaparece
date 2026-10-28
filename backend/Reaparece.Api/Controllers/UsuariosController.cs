using Microsoft.AspNetCore.Mvc;
using Reaparece.Api.Models;
using Reaparece.Api.Services;

namespace Reaparece.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController(UsuarioStore store) : ControllerBase
{
    [HttpGet("{id:int}/perfil")]
    public ActionResult<PerfilResponse> Perfil(int id)
    {
        var usuario = store.BuscarPorId(id);
        if (usuario is null)
            return NotFound(new ErrorResponse("Usuario no encontrado"));

        var comunidades = Comunidad.Todas
            .Where(c => usuario.ComunidadIds.Contains(c.Id))
            .Select(c => new ComunidadResponse(c.Id, c.Nombre, c.Categoria, c.Verificada, c.Id == usuario.ComunidadPrincipalId))
            .ToList();

        return Ok(new PerfilResponse(
            usuario.Id,
            usuario.NombreCompleto,
            usuario.Correo,
            usuario.Reputacion,
            usuario.ObjetosPublicados,
            usuario.ObjetosRecuperados,
            usuario.ObjetosReclamados,
            comunidades));
    }
}
