using FluentAssertions; // Deixa a leitura do teste mais natural
using FluentValidation.TestHelper; // Ferramenta específica para testar Validators
using SaasApi.Models; // Importa os modelos do projeto principal
using SaasApi.Validators; // Importa o validator que vamos testar
using Xunit; // O motor que roda os testes

namespace SaasApi.Tests.Validators;

public class ExpenseRequestValidatorTests
{
    // A variável que vai guardar o nosso Validator
    private readonly ExpenseRequestValidator _validator;

    public ExpenseRequestValidatorTests()
    {
        // ARRANGE (Preparação)
        // Antes de cada teste, a gente cria uma instância "zerada" do validator
        _validator = new ExpenseRequestValidator();
    }

    [Fact] // [Fact] diz pro .NET: "Isso aqui é um teste, execute-o!"
    public void Deve_Dar_Erro_Quando_Texto_For_Vazio()
    {
        // ARRANGE (Cenário)
        // Criamos um pedido inválido (texto vazio)
        var model = new ExpenseRequest { Texto = "" };

        // ACT (Ação)
        // Pedimos pro validator validar esse modelo
        var result = _validator.TestValidate(model);

        // ASSERT (Verificação)
        // O teste só passa se o resultado TIVER um erro na propriedade 'Texto'
        // E a mensagem de erro TEM que ser aquela que definimos ontem
        result.ShouldHaveValidationErrorFor(x => x.Texto)
              .WithErrorMessage("O texto da despesa é obrigatório.");
    }

    [Fact]
    public void Deve_Dar_Erro_Quando_Texto_For_Muito_Curto()
    {
        // ARRANGE
        var model = new ExpenseRequest { Texto = "Oi" }; // Só 2 letras (o mínimo é 5)

        // ACT
        var result = _validator.TestValidate(model);

        // ASSERT
        // Esperamos erro, pois é muito curto
        result.ShouldHaveValidationErrorFor(x => x.Texto);
    }

    [Fact]
    public void Deve_Passar_Quando_Texto_For_Valido()
    {
        // ARRANGE
        var model = new ExpenseRequest { Texto = "Comprei um mouse novo" };

        // ACT
        var result = _validator.TestValidate(model);

        // ASSERT
        // Aqui NÃO pode ter erro nenhum
        result.ShouldNotHaveValidationErrorFor(x => x.Texto);
    }
}