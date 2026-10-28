using Exo.WebApi.Contexts;
using Exo.WebApi.Repositories;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExoContext, ExoContext>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Habilita o CORS: permite que aplicações de outras origens (ex.: front-end)
// consumam a API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Forma de autenticação.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
// Parâmetros de validação do token.
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Valida quem está solicitando.
        ValidateIssuer = true,
        // Valida quem está recebendo.
        ValidateAudience = true,
        // Define se o tempo de expiração será validado.
        ValidateLifetime = true,
        // Criptografia e validação da chave de autenticação.
        // ATENÇÃO: esta chave precisa ser IDÊNTICA à usada no UsuariosController.
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("exoapi-chaveautenticacao")),
        // Tolerância no tempo de expiração do token.
        ClockSkew = TimeSpan.FromMinutes(30),
        // Nome do issuer, da origem.
        ValidIssuer = "exoapi.webapi",
        // Nome do audience, para o destino.
        ValidAudience = "exoapi.webapi"
    };
});

builder.Services.AddTransient<ProjetoRepository, ProjetoRepository>();
builder.Services.AddTransient<UsuarioRepository, UsuarioRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Aplica o CORS (deve vir depois do UseRouting e antes da autenticação).
app.UseCors("CorsPolicy");

// Habilita a autenticação.
app.UseAuthentication();

// Habilita a autorização.
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
