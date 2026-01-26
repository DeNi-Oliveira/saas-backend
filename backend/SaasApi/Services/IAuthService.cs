using SaasApi.DTOs;
using SaasApi.Models;

namespace SaasApi.Services;

public interface IAuthService
{
    Task<Usuario> RegistrarAsync(CreateUsuarioRequest request);
    Task<string?> LoginAsync(LoginRequest request); // Retorna o Token (string)
}