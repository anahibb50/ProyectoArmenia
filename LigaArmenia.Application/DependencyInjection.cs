using LigaArmenia.Application.Interfaces.Services;
using LigaArmenia.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LigaArmenia.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IRolPermisoService, RolPermisoService>();
        services.AddScoped<IVocalService, VocalService>();
        services.AddScoped<ICampeonatoService, CampeonatoService>();
        services.AddScoped<INominaService, NominaService>();
        return services;
    }
}
