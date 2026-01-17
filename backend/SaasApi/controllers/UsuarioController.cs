using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaasApi.Data;
using SaasApi.Models;
using SaasApi.DTOs; // <--- 1. IMPORTANTE: Adicione isso para usar os DTOs

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

    // GET: api/usuario (Listar)
    [HttpGet]
    public async Task<IActionResult> GetUsuarios()
    {
        // 1. Busca os usuários do banco (Models)
        var usuariosDoBanco = await _context.Usuarios.ToListAsync();

        // 2. Transforma (Mapeia) cada Model em um UsuarioResponse (DTO)
        // Usamos o .Select para converter a lista inteira de uma vez
        var listaDeResposta = usuariosDoBanco
            .Select(u => new UsuarioResponse(u.Id, u.Nome, u.Email, u.Cnpj));

        // 3. Retorna a lista limpa (sem senhas, sem dados ocultos)
        return Ok(listaDeResposta);
    }

    // POST: api/usuario (Cadastrar)
    [HttpPost]
    public async Task<IActionResult> CriarUsuario([FromBody] CreateUsuarioRequest request) // <--- Recebe o DTO
    {
        // 1. Mapeamento Manual: Passar dados do DTO para o Model
        // (Aqui a gente ignora campos perigosos como IsAdmin se existissem)
        var novoUsuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            Cnpj = request.Cnpj
        };

        // 2. Salva no Banco
        _context.Usuarios.Add(novoUsuario);
        await _context.SaveChangesAsync();

        // 3. Cria a resposta segura
        var resposta = new UsuarioResponse(novoUsuario.Id, novoUsuario.Nome, novoUsuario.Email, novoUsuario.Cnpj);

        // 4. Retorna 201 Created com o DTO de resposta
        return CreatedAtAction(nameof(GetUsuarios), new { id = resposta.Id }, resposta);
    }
}