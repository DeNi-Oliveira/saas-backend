using Microsoft.AspNetCore.Mvc;
using SaasApi.Models;
using SaasApi.Services; // Importar o Service

namespace SaasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    // Agora o Controller só conhece o SERVIÇO, não sabe nada de IA ou Banco
    public AiController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpPost("classify-expenses")]
    public async Task<IActionResult> ClassifyExpenses([FromBody] ExpenseRequest request)
    { 
        var resultado = await _expenseService.ProcessarDespesaAsync(request.Texto);
        return Ok(resultado);  
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var historico = await _expenseService.ObterHistoricoAsync();
        return Ok(historico);
    }
}