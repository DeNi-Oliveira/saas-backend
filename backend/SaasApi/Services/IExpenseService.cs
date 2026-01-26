using SaasApi.Models;

namespace SaasApi.Services;

public interface IExpenseService
{
    // Método para processar o texto e salvar
    Task<object> ProcessarDespesaAsync(string texto);

    // Método para buscar o histórico
    Task<List<object>> ObterHistoricoAsync();
}