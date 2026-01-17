using Microsoft.EntityFrameworkCore;
using SaasApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddNpgSql(connectionString!);
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirBubble",
        policy =>
        {
            policy.WithOrigins("https://seu-app-bubble.bubbleapps.io") // Troque pelo link do seu Bubble
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirBubble");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
