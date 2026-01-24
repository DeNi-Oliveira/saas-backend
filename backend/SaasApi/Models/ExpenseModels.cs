namespace SaasApi.Models;

// O que a IA vai devolver
public class ExpenseAnalysisResult
{
    public string Resumo { get; set; } = string.Empty;
    public string CategoriaPrincipal { get; set; } = string.Empty; // Ex: Viagem, Alimentação, Escritório
    public List<ExpenseItem> ItensIdentificados { get; set; } = new();
    public List<string> TagsSugeridas { get; set; } = new();
}

// Detalhe de cada gasto encontrado no texto
public class ExpenseItem
{
    public string Descricao { get; set; } = string.Empty;
    public string ValorEstimado { get; set; } = string.Empty; // String para aceitar "R$ 50" ou "50 reais"
    public string CategoriaItem { get; set; } = string.Empty;
}

// O que o usuário manda no Post (Input)
public class ExpenseRequest
{
    public string Texto { get; set; } = string.Empty;
}