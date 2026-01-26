using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SaasApi.Data;
using SaasApi.DTOs;
using SaasApi.Models;
using BCrypt.Net;

namespace SaasApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration; // Para ler a chave do appsettings

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<Usuario> RegistrarAsync(CreateUsuarioRequest request)
    {
        // 1. Verifica se email já existe
        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            throw new Exception("Este email já está cadastrado.");

        // 2. Criptografa a senha (Nunca salvar pura!)
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

        // 3. Cria o usuário
        var novoUsuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            Cnpj = request.Cnpj,
            PasswordHash = passwordHash // Salva o hash
        };

        _context.Usuarios.Add(novoUsuario);
        await _context.SaveChangesAsync();

        return novoUsuario;
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        // 1. Busca usuário pelo email
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        // 2. Verifica se usuário existe E se a senha bate com o Hash
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.PasswordHash))
        {
            return null; // Login inválido
        }

        // 3. Se passou, gera o Token JWT
        var token = GerarTokenJwt(usuario);
        return token;
    }

    private string GerarTokenJwt(Usuario usuario)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // O que vai dentro do Token (Claims)
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("Nome", usuario.Nome) // Da pra colocar infos extras
            }),
            Expires = DateTime.UtcNow.AddHours(8), // Token vale por 8 horas
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}