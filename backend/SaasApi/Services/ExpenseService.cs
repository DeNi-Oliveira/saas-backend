using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using SaasApi.Data;
using SaasApi.Models;

namespace SaasApi.Services;

public class ExpenseService(Kernel kernel, IChatCompletionService chatService, AppDbContext context) : IExpenseService
{
    private readonly Kernel _kernel = kernel;
    private readonly IChatCompletionService _chatService = chatService;
    private readonly AppDbContext _context = context;

    public async Task<object> ProcessarDespesaAsync(string texto)
    {
        // 1. Configurar Prompt
        var history = new ChatHistory();
        history.AddSystemMessage(@"
            Você é um assistente financeiro (API). 
            Responda APENAS com um JSON válido seguindo esta estrutura:
            { ""Resumo"": ""..."", ""CategoriaPrincipal"": ""..."", ""ItensIdentificados"": [...], ""TagsSugeridas"": [...] }
            Responda em Português.
        ");
        history.AddUserMessage($"Analise: \"{texto}\"");

        // 2. Chamar IA
        var result = await _chatService.GetChatMessageContentAsync(history, kernel: _kernel);
        
        string jsonResponse = result.Content!
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        var dadosEstruturados = JsonSerializer.Deserialize<ExpenseAnalysisResult>(jsonResponse);

        // 3. Salvar no Banco
        var novaDespesa = new Expense
        {
            TextoOriginal = texto,
            ResultadoJson = jsonResponse,
            DataCriacao = DateTime.UtcNow
        };

        _context.Expenses.Add(novaDespesa);
        await _context.SaveChangesAsync();

        return new { db_id = novaDespesa.Id, analise = dadosEstruturados };
    }

    public async Task<List<object>> ObterHistoricoAsync()
    {
        var dadosBrutos = await _context.Expenses
            .OrderByDescending(x => x.DataCriacao)
            .Take(20)
            .ToListAsync();

        // Faz o mapeamento para um formato mais limpo
        var historicoLimpo = dadosBrutos.Select(item => new 
        {
            Id = item.Id,
            Data = item.DataCriacao,
            TextoEnviado = item.TextoOriginal,
            Analise = JsonSerializer.Deserialize<ExpenseAnalysisResult>(item.ResultadoJson)
        }).ToList<object>();

        return historicoLimpo;
    }
}