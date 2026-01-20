using Microsoft.EntityFrameworkCore;
using SaasApi.Data;
using Microsoft.SemanticKernel;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .WriteTo.Console());

var geminiKey = builder.Configuration["Gemini:ApiKey"];
var geminiModel = builder.Configuration["Gemini:ModelId"];

if (!string.IsNullOrEmpty(geminiKey))
{
    builder.Services.AddKernel()
        .AddGoogleAIGeminiChatCompletion(
            modelId: geminiModel!,
            apiKey: geminiKey);
}

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
            policy.WithOrigins("https://seu-app-bubble.bubbleapps.io") // Trocar pelo link do bubble
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseSerilogRequestLogging();

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
