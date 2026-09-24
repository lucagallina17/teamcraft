using System.Text;
using System.Text.Json.Serialization;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TeamCraft.Application.Commands;
using TeamCraft.Application.Consumers;
using TeamCraft.Application.Interfaces.Repositories;
using TeamCraft.Application.Repositories;
using TeamCraft.Application.Services.Implementations;
using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Infrastructure.Auth;
using TeamCraft.Infrastructure.Persistence;
using TeamCraft.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS � origin consentiti letti da configurazione (Cors:AllowedOrigins),
// cos� in Azure si aggiungono senza ricompilare (App Service > Configuration)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Repository
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ICompetencyRepository, CompetencyRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectRoleRepository, ProjectRoleRepository>();
builder.Services.AddScoped<IEmployeeAffinityRepository, EmployeeAffinityRepository>();
builder.Services.AddScoped<ITeamAggregateRepository, TeamAggregateRepository>();
builder.Services.AddScoped<ITeamReadRepository, TeamReadRepository>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<ITeamReviewAggregateRepository, TeamReviewAggregateRepository>();

// Service
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICompetencyService, CompetencyService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITeamMatchingService, TeamMatchingService>();
builder.Services.AddScoped<IProjectRoleService, ProjectRoleService>();
builder.Services.AddScoped<IEmployeeAffinityService, EmployeeAffinityService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }); ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Incolla solo il token, senza il prefisso 'Bearer ' — lo aggiunge Swagger da solo"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ActivateTeamCommandHandler).Assembly));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TeamActivatedIntegrationConsumer>();
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

// Autenticazione JWT
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Applica automaticamente le migration EF Core mancanti all'avvio.
// Scelta accettabile per un'app a singola istanza (tier F1); da rivedere
// se in futuro si passa a pi� istanze (rischio di corse tra istanze).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
// Swagger abilitato anche in Production: progetto portfolio dimostrabile,
// nessun dato sensibile esposto tramite lo schema API.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Attiva la policy CORS � deve stare prima di UseAuthorization
app.UseCors("AllowAngularApp");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
