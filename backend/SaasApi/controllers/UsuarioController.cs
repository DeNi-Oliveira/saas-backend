using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaasApi.Data;
using SaasApi.Models;

namespace SaasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/usuario (Botão Azul - Listar)
    [HttpGet]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios = await _context.Usuarios.ToListAsync();
        return Ok(usuarios);
    }

    // POST: api/usuario (Botão Verde - Cadastrar)
    [HttpPost]
    public async Task<IActionResult> CriarUsuario([FromBody] Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Id }, usuario);
    }
}