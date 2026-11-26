namespace LigaArmenia.Application.Interfaces.Persistence;

/// <summary>
/// Ejecuta operaciones compuestas dentro de una transacción de base de datos
/// compatible con la estrategia de reintentos de Entity Framework Core.
/// </summary>
public interface ITransactionRunner
{
    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default);
}
