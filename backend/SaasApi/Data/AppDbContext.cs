using Microsoft.EntityFrameworkCore;
using SaasApi.Models;

namespace SaasApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
}
