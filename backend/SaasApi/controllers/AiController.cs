using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necessário para acessar o banco
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using SaasApi.Data; // Seu DbContext
using SaasApi.Models;

namespace SaasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;
    private readonly AppDbContext _context; // <--- A novidade: O Banco de Dados

    // Injetamos o Contexto aqui no construtor
    public AiController(Kernel kernel, IChatCompletionService chatService, AppDbContext context)
    {
        _kernel = kernel;
        _chatService = chatService;
        _context = context;
    }

    [HttpPost("classify-expenses")]
    public async Task<IActionResult> ClassifyExpenses([FromBody] ExpenseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
            return BadRequest("O texto não pode estar vazio.");

        // 1. Configurar IA (System Prompt)
        var history = new ChatHistory();
        history.AddSystemMessage(@"
            Você é um assistente financeiro (API). 
            Responda APENAS com um JSON válido seguindo esta estrutura:
            { ""Resumo"": ""..."", ""CategoriaPrincipal"": ""..."", ""ItensIdentificados"": [...], ""TagsSugeridas"": [...] }
            Responda em Português.
        ");
        history.AddUserMessage($"Analise: \"{request.Texto}\"");

        try
        {
            // 2. Chamar IA
            var result = await _chatService.GetChatMessageContentAsync(history, kernel: _kernel);
            
            // Limpeza básica do JSON (Markdown strip)
            string jsonResponse = result.Content!
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            // 3. Validar o JSON (Parse)
            var dadosEstruturados = JsonSerializer.Deserialize<ExpenseAnalysisResult>(jsonResponse);

            // --- SALVA NO BANCO ---
            var novaDespesa = new Expense
            {
                TextoOriginal = request.Texto,
                ResultadoJson = jsonResponse, // Salvam o JSON bruto da IA
                DataCriacao = DateTime.UtcNow
            };

            _context.Expenses.Add(novaDespesa);
            await _context.SaveChangesAsync();
            // --------------------------------------------------

            // Retorna o resultado da IA + o ID gerado no banco
            return Ok(new { 
                db_id = novaDespesa.Id, 
                analise = dadosEstruturados 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao processar", details = ex.Message });
        }
    }

    // Rota para ler o histórico ---
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        // Busca os dados brutos no banco
        var dadosBrutos = await _context.Expenses
            .OrderByDescending(x => x.DataCriacao)
            .Take(20)
            .ToListAsync();

        // Transforma (Mapeia) para um formato limpo
        var historicoLimpo = dadosBrutos.Select(item => new 
        {
            item.Id,
            Data = item.DataCriacao,
            TextoEnviado = item.TextoOriginal,
            // Converte a string de volta para Objeto aqui!
            Analise = JsonSerializer.Deserialize<ExpenseAnalysisResult>(item.ResultadoJson)
        });

        return Ok(historicoLimpo);
    }
}