using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaasApi.Models;

// Esta classe vira uma TABELA no Postgres chamada "Expenses"
public class Expense
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string TextoOriginal { get; set; } = string.Empty; // O que o usuário digitou

    [Column(TypeName = "jsonb")] // No Postgres, isso salva como JSON consultável!
    public string ResultadoJson { get; set; } = string.Empty; // A resposta da IA completa

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}