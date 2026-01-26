using System.Text.Json.Serialization;

namespace SaasApi.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;

    [JsonIgnore] // O JsonIgnore impede que o hash da senha apareça no Retorno da API
    public string PasswordHash { get; set; } = string.Empty;
}
