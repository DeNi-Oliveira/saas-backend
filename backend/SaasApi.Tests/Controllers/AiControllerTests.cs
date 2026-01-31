using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq; // <--- O Mágico dos Dublês
using SaasApi.Controllers;
using SaasApi.Models;
using SaasApi.Services;
using Xunit;

namespace SaasApi.Tests.Controllers;

public class AiControllerTests
{
    // O Mock que vai fingir ser o serviço
    private readonly Mock<IExpenseService> _mockService;
    // O Controller real que vamos testar
    private readonly AiController _controller;

    public AiControllerTests()
    {
        // 1. Criamos o Dublê (Mock)
        _mockService = new Mock<IExpenseService>();

        // 2. Injetamos o Dublê no Controller
        // Repare no ".Object": é assim que transformamos o Mock na Interface real
        _controller = new AiController(_mockService.Object);
    }

    [Fact]
    public void Deve_Retornar_Ok_Quando_Texto_Valido()
    {
        // ARRANGE (Cenário)
        var request = new ExpenseRequest { Texto = "Gastei 100 reais" };
        var respostaEsperada = "Categoria: Transporte"; // Resposta fake

        // O Roteiro do Dublê (Setup):
        // "Quando chamarem ProcessarDespesaAsync com qualquer string...
        // ...retorne 'Categoria: Transporte' imediatamente."
        _mockService.Setup(service => service.ProcessarDespesaAsync(It.IsAny<string>()))
                    .ReturnsAsync(respostaEsperada);

        // ACT (Ação)
        // Chamamos o controller (que vai usar o dublê sem saber)
        var result = _controller.ClassifyExpenses(request).Result;

        // ASSERT (Verificação)
        // Verificamos se o Controller devolveu um status 200 (OK)
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        
        // Verificamos se o conteúdo é o que o dublê entregou
        okResult.Value.Should().Be(respostaEsperada);
    }

    [Fact]
    public void Deve_Chamar_O_Servico_Uma_Vez()
    {
        // Teste de Comportamento: Queremos saber se o Controller realmente chamou o Service
        
        // ARRANGE
        var request = new ExpenseRequest { Texto = "Teste" };

        // ACT
        _controller.ClassifyExpenses(request).Wait();

        // ASSERT
        // "Verifique se o método ProcessarDespesaAsync foi chamado exatamente 1 vez"
        _mockService.Verify(s => s.ProcessarDespesaAsync(request.Texto), Times.Once);
    }
}