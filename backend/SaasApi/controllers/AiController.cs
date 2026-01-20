using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace SaasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly Kernel _kernel;
    public AiController(Kernel kernel)
    {
        _kernel = kernel;
    }

    // Rota: GET /api/ai/perguntar?texto=Ola
    [HttpGet("perguntar")]
    public async Task<IActionResult> Perguntar([FromQuery] string texto)
    {
        try
        {
            // InvokePromptAsync manda o texto pro Gemini e traz a resposta
            var resposta = await _kernel.InvokePromptAsync(texto);
            
            return Ok(new { 
                Pergunta = texto, 
                Resposta = resposta.ToString() 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Erro = "O gemini teve um problema cerebral:", Detalhes = ex.Message });
        }
    }
}