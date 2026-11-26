using Microsoft.AspNetCore.Authorization;

namespace LigaArmenia.Web.Security;

public enum VocaliaResourceType
{
    Partido = 1,
    Vocalia = 2
}

public sealed record VocaliaResourceTarget(VocaliaResourceType ResourceType, long ResourceId);

/// <summary>
/// Requisito reutilizable para validar que el usuario autenticado tenga asignado
/// el partido o sea propietario de la vocalía solicitada.
/// </summary>
public sealed class VocaliaAssignmentRequirement : IAuthorizationRequirement
{
    public static readonly VocaliaAssignmentRequirement Instance = new();
}
