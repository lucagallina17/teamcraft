using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Interfaces.Repositories;
using TeamCraft.Application.Services.Implementations;
using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Application.Repositories;
using TeamCraft.Infrastructure.Persistence;
using TeamCraft.Infrastructure.Persistence.Repositories;
using TeamCraft.Application.Commands;
using MassTransit;
using TeamCraft.Application.Consumers;

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

// Service
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICompetencyService, CompetencyService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITeamMatchingService, TeamMatchingService>();
builder.Services.AddScoped<IProjectRoleService, ProjectRoleService>();
builder.Services.AddScoped<IEmployeeAffinityService, EmployeeAffinityService>();

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }); ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ActivateTeamCommandHandler).Assembly));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TeamActivatedIntegrationConsumer>();
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

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

app.UseAuthorization();

app.MapControllers();

app.Run();
