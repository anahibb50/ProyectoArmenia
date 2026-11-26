namespace LigaArmenia.Application.Interfaces.Security;

public interface IAdminSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
