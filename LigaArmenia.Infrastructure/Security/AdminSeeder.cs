using LigaArmenia.Application.Interfaces.Security;
using LigaArmenia.Domain.Constants;
using LigaArmenia.Domain.Entities;
using LigaArmenia.Domain.Rules;
using LigaArmenia.Infrastructure.Data;
using LigaArmenia.Infrastructure.Security.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LigaArmenia.Infrastructure.Security;

public sealed class AdminSeeder : IAdminSeeder
{
    private readonly LigaArmeniaDbContext _dbContext;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly AdminSeedOptions _options;
    private readonly ILogger<AdminSeeder> _logger;

    public AdminSeeder(
        LigaArmeniaDbContext dbContext,
        IPasswordHasherService passwordHasher,
        IOptions<AdminSeedOptions> options,
        ILogger<AdminSeeder> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("AdminSeed deshabilitado. No se ejecutará la inicialización del administrador.");
            return;
        }

        var nombres = _options.Nombres?.Trim() ?? string.Empty;
        var correoNormalizado = PasswordPolicy.NormalizeEmail(_options.Correo);
        var password = _options.Password;

        if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(correoNormalizado) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("AdminSeed habilitado pero con configuración incompleta. Se omite la inicialización.");
            return;
        }

        var (isValidPassword, _) = PasswordPolicy.Validate(password);
        if (!isValidPassword)
        {
            _logger.LogWarning("AdminSeed omitido: la contraseña configurada no cumple la política mínima de seguridad.");
            return;
        }

        var strategy = _dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var adminRole = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Nombre.ToUpper() == SystemRoles.Administrador, cancellationToken);

            if (adminRole is null)
            {
                _logger.LogWarning("AdminSeed omitido: el rol {RoleName} no existe en la base de datos.", SystemRoles.Administrador);
                await transaction.RollbackAsync(cancellationToken);
                return;
            }

            var adminExists = await _dbContext.UsuariosRoles
                .AsNoTracking()
                .AnyAsync(ur => ur.RolId == adminRole.Id, cancellationToken);

            if (adminExists)
            {
                _logger.LogInformation("AdminSeed omitido: ya existe al menos un usuario con rol {RoleName}.", SystemRoles.Administrador);
                await transaction.RollbackAsync(cancellationToken);
                return;
            }

            var existingAccount = await _dbContext.Usuarios
                .AsNoTracking()
                .AnyAsync(u => u.Correo.ToLower() == correoNormalizado, cancellationToken);

            if (existingAccount)
            {
                _logger.LogWarning(
                    "AdminSeed omitido: ya existe una cuenta registrada con el correo configurado. " +
                    "Por seguridad no se elevan privilegios ni se sobrescriben contraseñas.");
                await transaction.RollbackAsync(cancellationToken);
                return;
            }

            var now = DateTime.UtcNow;
            var adminUser = new Usuario
            {
                Nombres = nombres,
                Correo = correoNormalizado,
                Activo = true,
                CreadoEn = now,
                ActualizadoEn = now
            };

            adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, password);

            await _dbContext.Usuarios.AddAsync(adminUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var userRole = new UsuariosRole
            {
                UsuarioId = adminUser.Id,
                RolId = adminRole.Id,
                CreadoEn = now,
                ActualizadoEn = now,
                CreadoPor = adminUser.Id,
                ActualizadoPor = adminUser.Id
            };

            await _dbContext.UsuariosRoles.AddAsync(userRole, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Administrador inicial registrado correctamente (UsuarioId: {UserId}).", adminUser.Id);
        });
    }
}
