using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion; // Necessário para o ChatHistory
using System.Text.Json;
using SaasApi.Models; // Importando os modelos que criamos

namespace SaasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;

    public AiController(Kernel kernel, IChatCompletionService chatService)
    {
        _kernel = kernel;
        _chatService = chatService;
    }

    [HttpGet("perguntar")]
    public async Task<IActionResult> Perguntar([FromQuery] string texto)
    {
        try
        {
            var resposta = await _kernel.InvokePromptAsync(texto);
            return Ok(new { Pergunta = texto, Resposta = resposta.ToString() });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = "Erro no Gemini", Detalhes = ex.Message });
        }
    }

    // --- MÉTODO DE ENGENHARIA DE PROMPT para teste de despesas ---
    [HttpPost("classify-expenses")]
    public async Task<IActionResult> ClassifyExpenses([FromBody] ExpenseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
            return BadRequest("O texto não pode estar vazio.");

        // 1. Definição do SYSTEM PROMPT (O Cérebro)
        // Aqui "hipnotizo" a IA para ela esquecer que é um chatbot e virar uma API JSON
        var history = new ChatHistory();
        history.AddSystemMessage(@"
            Você é um assistente financeiro (API) especializado em estruturar dados de despesas.
            Sua tarefa é ler relatos informais e extrair um JSON estrito.

            REGRAS OBRIGATÓRIAS:
            1. Responda APENAS com o JSON. Sem markdown (```json), sem explicações.
            2. Siga estritamente esta estrutura:
            {
                ""Resumo"": ""Texto curto resumindo o gasto"",
                ""CategoriaPrincipal"": ""Categoria macro (ex: Alimentação, Transporte, Viagem)"",
                ""ItensIdentificados"": [
                    { ""Descricao"": ""O que foi comprado"", ""ValorEstimado"": ""Valor (ou 'Não informado')"", ""CategoriaItem"": ""Subcategoria"" }
                ],
                ""TagsSugeridas"": [""Tag1"", ""Tag2""]
            }
            3. Se o valor não for claro, use 'Não informado'.
            4. Responda em Português do Brasil.
        ");

        // 2. Adiciona o texto do usuário
        history.AddUserMessage($"Analise este relato: \"{request.Texto}\"");

        try
        {
            // 3. Chama a IA usando o serviço de Chat (mais avançado que o InvokePrompt)
            var result = await _chatService.GetChatMessageContentAsync(
                history,
                kernel: _kernel
            );

            // 4. Limpeza da resposta (O Gemini adora colocar ```json no começo)
            string jsonResponse = result.Content!
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            // 5. Converte o texto da IA em Objeto C#
            var dadosEstruturados = JsonSerializer.Deserialize<ExpenseAnalysisResult>(jsonResponse);

            return Ok(dadosEstruturados);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                error = "Falha ao processar IA ou Deserializar JSON", 
                details = ex.Message 
            });
        }
    }
}