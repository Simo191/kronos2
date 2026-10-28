using GestionPrestataires.Domain.Repositories;
using GestionPrestataires.Domain.Security;
using GestionPrestataires.Infrastructure.Security;
using GestionPrestataires.Infrastructure.Data;
using GestionPrestataires.Infrastructure.Persistence.Repositories;
using GestionPrestataires.Infrastructure.Repositories;
using GestionRoles.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
//    typeof(GestionPrestataires.Application.Prestataires.Commands.CreatePrestataire.CreatePrestataireCommand).Assembly));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(GestionPrestataires.Application.Prestataires.Commands.CreatePrestataire.CreatePrestataireCommand).Assembly
    );
    // ✅ Fallback : scanner aussi l'assembly du handler Update si dans un projet séparé
    cfg.RegisterServicesFromAssembly(
        typeof(GestionPrestataires.Application.JoursTravail.Commands.UpdateJourTravail.UpdateJourTravailCommandHandler).Assembly
    );
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPrestataireRepository, PrestataireRepository>();
builder.Services.AddScoped<ITypeContratRepository, TypeContratRepository>();
builder.Services.AddScoped<IContratRepository, ContratRepository>();
builder.Services.AddScoped<IJourTravailRepository, JourTravailRepository>();
builder.Services.AddScoped<ICagnotteRepository, CagnotteRepository>();
builder.Services.AddScoped<ISourceBudgetRepository, SourceBudgetRepository>();
builder.Services.AddScoped<ITypeShiftRepository, TypeShiftRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IMoisVerrouillRepository, MoisVerrouillRepository>();
builder.Services.AddScoped<IEstimationRepository, EstimationRepository>();
builder.Services.AddScoped<ILigneEstimationRepository, LigneEstimationRepository>();
builder.Services.AddScoped<IApplicationPlanningRepository, ApplicationPlanningRepository>();
builder.Services.AddScoped<IApplicationPrestataireAssignmentRepository, ApplicationPrestataireAssignmentRepository>();
builder.Services.AddScoped<IApplicationServerRepository, ApplicationServerRepository>();
builder.Services.AddScoped<IPrestataireCredentialRepository, PrestataireCredentialRepository>();
builder.Services.AddScoped<IEntrepriseRepository, EntrepriseRepository>();

//   Chiffrement AES-256-GCM pour les mots de passe des comptes prestataires
builder.Services.AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>();


// ✅ JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key non configuré"))
        )
    };
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ✅ CORS corrigé — autorise toutes les origines connues + preflight OPTIONS pour les uploads
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://localhost:56378",
                "http://x72sdeigfze:8036"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Content-Disposition"); // ✅ utile pour téléchargements
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ✅ CORS DOIT être avant tout le reste (StaticFiles, Auth, etc.)
app.UseCors("AllowAngular");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Gérer explicitement le preflight OPTIONS (nécessaire pour les uploads multipart)
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

try
{
    Log.Information("Starting LeaStudio API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}