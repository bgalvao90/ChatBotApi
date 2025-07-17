using System.Text;
using ChatBotApi;
using ChatBotApi.Context;
using ChatBotApi.DTOs.Mapping;
using ChatBotApi.Repositories.Implementations;
using ChatBotApi.Repositories.Interfaces;
using ChatBotApi.Services.Implementations;
using ChatBotApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiCatalogoMinimal", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header usando o esquema Bearer. " +
        "Digite 'Bearer' [espaço] e então seu token no campo abaixo. Exemplo: \"Bearer 12345abcdef\""
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddAutoMapper(typeof(MensagemDTOMappingProfile));

builder.Services.AddScoped<IAtendimentoRepository, AtendimentoRepository>();
builder.Services.AddScoped<IAtendenteRepository, AtendenteRepository>();
builder.Services.AddScoped<IMensagemRepository, MensagemRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDistribuidorService, DistribuidorService>();
builder.Services.AddScoped<IAtendenteService, AtendenteService>();
builder.Services.AddScoped<IAtendimentoService, AtendimentoService>();
builder.Services.AddScoped<IIAService, IAService>();
builder.Services.AddScoped<IMensagemService, MensagemService>();
builder.Services.AddScoped<ICanalService, TelegramService>();
builder.Services.AddScoped<IClienteService, ClienteService>();





builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("PermitirTudo");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
