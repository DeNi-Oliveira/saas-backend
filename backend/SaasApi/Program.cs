using Microsoft.EntityFrameworkCore;
using SaasApi.Data;
using Microsoft.SemanticKernel;
using Serilog;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddNpgSql(connectionString, name: "database", tags: ["db", "data"]);
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

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Em vez de um limitador simples, criamos uma POLÍTICA
    options.AddPolicy("fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            // o IP de quem está chamando
            // Se não tiver IP (localhost as vezes esconde), usa "anonimo"
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonimo", 
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(10),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirBubble");
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("fixed"); // Ou colocar [EnableRateLimiting("fixed")] em cima de cada Controller
app.MapHealthChecks("/health", new HealthCheckOptions
{
    // Isso faz o health check responder um JSON bonito em vez de só texto
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                component = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.ToString()
            }),
            totalDuration = report.TotalDuration
        };
        await JsonSerializer.SerializeAsync(context.Response.Body, response);
    }
})
.RequireRateLimiting("fixed");
app.Run();
