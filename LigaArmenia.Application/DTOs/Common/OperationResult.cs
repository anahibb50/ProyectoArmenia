namespace LigaArmenia.Application.DTOs.Common;

public class OperationResult
{
    public bool Succeeded { get; protected init; }
    public string? Message { get; protected init; }
    public IReadOnlyList<string> Errors { get; protected init; } = Array.Empty<string>();

    public static OperationResult Ok(string? message = null) => new()
    {
        Succeeded = true,
        Message = message
    };

    public static OperationResult Fail(string error) => new()
    {
        Succeeded = false,
        Message = error,
        Errors = new[] { error }
    };

    public static OperationResult Fail(IEnumerable<string> errors)
    {
        var list = errors.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
        return new OperationResult
        {
            Succeeded = false,
            Message = list.FirstOrDefault() ?? "No se pudo completar la operación.",
            Errors = list
        };
    }
}

public class OperationResult<T> : OperationResult
{
    public T? Data { get; private init; }

    public static OperationResult<T> Ok(T data, string? message = null) => new()
    {
        Succeeded = true,
        Data = data,
        Message = message
    };

    public new static OperationResult<T> Fail(string error) => new()
    {
        Succeeded = false,
        Message = error,
        Errors = new[] { error }
    };

    public new static OperationResult<T> Fail(IEnumerable<string> errors)
    {
        var list = errors.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
        return new OperationResult<T>
        {
            Succeeded = false,
            Message = list.FirstOrDefault() ?? "No se pudo completar la operación.",
            Errors = list
        };
    }
}
