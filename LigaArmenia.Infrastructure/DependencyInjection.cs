using LigaArmenia.Application.Interfaces.Persistence;
using LigaArmenia.Application.Interfaces.Repositories;
using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Infrastructure.Data;
using LigaArmenia.Infrastructure.Repositories;
using LigaArmenia.Infrastructure.Security;
using LigaArmenia.Infrastructure.Security.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LigaArmenia.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'ConnectionStrings:DefaultConnection'. " +
                "Configúrala mediante User Secrets en desarrollo o variables de entorno en producción.");
        }

        services.AddDbContext<LigaArmeniaDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
            }));

        services.AddMemoryCache();

        // Repositorios y transacciones
        services.AddScoped<ITransactionRunner, EfTransactionRunner>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IPermisoRepository, PermisoRepository>();
        services.AddScoped<IVocalRepository, VocalRepository>();
        services.AddScoped<ICampeonatoRepository, CampeonatoRepository>();
        services.AddScoped<INominaRepository, NominaRepository>();

        // Opciones de configuración
        services.Configure<AdminSeedOptions>(configuration.GetSection(AdminSeedOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // Servicios técnicos de seguridad
        services.AddSingleton<IPasswordHasherService, AspNetPasswordHasherService>();
        services.AddSingleton<ILoginAttemptTracker, MemoryLoginAttemptTracker>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUserSecurityStateService, UserSecurityStateService>();
        services.AddScoped<IVocaliaOwnershipService, VocaliaOwnershipService>();
        services.AddScoped<IAdminSeeder, AdminSeeder>();

        return services;
    }
}
