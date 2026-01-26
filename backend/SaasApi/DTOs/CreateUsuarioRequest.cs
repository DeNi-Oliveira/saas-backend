namespace SaasApi.DTOs;

public record CreateUsuarioRequest(string Nome, string Email, string Cnpj, string Senha);