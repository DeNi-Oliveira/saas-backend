using Microsoft.EntityFrameworkCore;
using SaasApi.Models;

namespace SaasApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Expense> Expenses { get; set; }
}
