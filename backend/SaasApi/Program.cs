using Microsoft.EntityFrameworkCore;
using SaasApi.Data;
using Microsoft.SemanticKernel;
using Serilog;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using SaasApi.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

// Adiciona os serviços de autenticação
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("Jwt:Key não encontrada no appsettings.");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; // Em produção (HTTPS) deve ser true
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,   // Simplificando para facilitar o teste
        ValidateAudience = false  // Simplificando para facilitar o teste
    };
});

// Registra o Serviço de Auth
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SaasApi", Version = "v1" });

    // Define que usa JWT Bearer
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Cole o token assim: Bearer 12345abcdef",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
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

    // Em vez de um limitador simples, cria uma POLÍTICA
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

builder.Services.AddScoped<SaasApi.Services.IExpenseService, SaasApi.Services.ExpenseService>();
builder.Services.AddFluentValidationAutoValidation(); // Habilita validação automática
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // Registra todos os validators do projeto

var app = builder.Build();
app.UseMiddleware<SaasApi.Middlewares.GlobalErrorHandlingMiddleware>();
app.UseSerilogRequestLogging();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseCors("PermitirBubble");
app.UseRateLimiter();
app.UseAuthentication();
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
