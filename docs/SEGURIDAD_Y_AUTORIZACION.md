# Guía de Seguridad y Autorización — Liga Barrial La Armenia

Esta guía explica cómo proteger controladores MVC y API REST utilizando el sistema de autorización basado en permisos (`RBAC`), cómo validar la propiedad/asignación en vocalías y cómo configurar los secretos de ejecución de forma segura.

---

## 1. Autorización basada en permisos (MVC)

El proveedor `PermissionPolicyProvider` resuelve automáticamente cualquier política con el formato `modulo.accion`.
Utiliza las constantes centralizadas en `LigaArmenia.Domain.Constants.SystemPermissions` para evitar errores tipográficos:

```csharp
using LigaArmenia.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public sealed class EquiposController : Controller
{
    [HttpGet]
    [Authorize(Policy = SystemPermissions.EquiposConsultar)]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = SystemPermissions.EquiposGestionar)]
    public IActionResult Create(/* ... */)
    {
        return RedirectToAction(nameof(Index));
    }
}
```

### ¿Cómo funciona internamente?
- `PermissionAuthorizationHandler` consulta el estado vigente del usuario en `IUserSecurityStateService` (con caché de 60 s e invalidación inmediata al editar/desactivar usuarios o modificar permisos de un rol).
- Siempre valida en el servidor que la cuenta esté **activa** y tenga el permiso vigente; ocultar botones en la vista es solo un apoyo visual.
- Si un usuario autenticado sin permiso intenta acceder por URL directa, ASP.NET Core redirige automáticamente a `/Account/AccessDenied` (`HTTP 403`).

---

## 2. Autorización en API REST (Android / JWT Bearer)

Los endpoints consumidos por la aplicación móvil Android deben especificar explícitamente el esquema `JwtBearerDefaults.AuthenticationScheme` para mantenerse separados de las cookies MVC:

```csharp
using LigaArmenia.Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/vocalias")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class VocaliasApiController : ControllerBase
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = SystemPermissions.VocaliasRegistrar)]
    public IActionResult Registrar(/* ... */)
    {
        return Ok();
    }
}
```

### Endpoints de autenticación disponibles
- `POST /api/auth/login` — Recibe `{ "correo": "...", "password": "..." }`. Valida estado activo, credenciales, intentos fallidos y acceso móvil (`VOCAL` / `ADMINISTRADOR`). Retorna JWT Bearer + perfil, roles y permisos (nunca `password_hash`).
- `GET /api/auth/me` — Requiere header `Authorization: Bearer <token>`. Valida firma, emisor, audiencia, expiración y estado activo vigente en servidor.

---

## 3. Validación de asignación y propiedad para Vocalías

Para los módulos de vocalías, existe un requisito reutilizable (`VocaliaAssignmentRequirement`) respaldado por `IVocaliaOwnershipService`:

```csharp
using LigaArmenia.Web.Security;
using Microsoft.AspNetCore.Authorization;

public sealed class VocaliasApiController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public VocaliasApiController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HttpPost("partidos/{partidoId:long}")]
    public async Task<IActionResult> RegistrarVocalia(long partidoId)
    {
        var auth = await _authorizationService.AuthorizeAsync(
            User,
            new VocaliaResourceTarget(VocaliaResourceType.Partido, partidoId),
            VocaliaAssignmentRequirement.Instance);

        if (!auth.Succeeded)
            return Forbid();

        // Continuar con el registro...
        return Ok();
    }
}
```

También se puede validar sobre una vocalía existente usando `VocaliaResourceType.Vocalia` y el `vocaliaId`.

---

## 4. Consulta de vocales disponibles para designación de partidos

El módulo de partidos puede obtener los vocales elegibles inyectando `IVocalService`:

```csharp
using LigaArmenia.Application.Interfaces.Services;

public sealed class PartidosController : Controller
{
    private readonly IVocalService _vocalService;

    public PartidosController(IVocalService vocalService)
    {
        _vocalService = vocalService;
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        // Devuelve únicamente Id y Nombres de vocales activos,
        // con usuario activo y rol VOCAL vigente.
        var vocalesDisponibles = await _vocalService.GetAvailableForDesignationAsync(cancellationToken);
        return View(vocalesDisponibles);
    }
}
```

### Script SQL para registrar permisos de gestión de vocales en Supabase
Ejecutar manualmente en Supabase si deseas asignar estos permisos a roles adicionales desde **Roles y Permisos**:

```sql
INSERT INTO ldbla.permisos (codigo, descripcion)
VALUES
    ('vocales.consultar', 'Consultar listado y detalle de vocales'),
    ('vocales.editar', 'Editar nombres y teléfono de vocales'),
    ('vocales.activar', 'Activar perfiles de vocales'),
    ('vocales.desactivar', 'Desactivar perfiles de vocales')
ON CONFLICT (codigo) DO NOTHING;
```

---

## 5. Configuración segura de secretos (User Secrets y Producción)

Nunca almacenes credenciales en `appsettings.json`, `Dockerfile` ni código fuente.

### Desarrollo local (`dotnet user-secrets` desde la raíz de la solución)

```powershell
# 1. Cadena de conexión PostgreSQL (Supabase)
dotnet user-secrets --project LigaArmenia.Web set "ConnectionStrings:DefaultConnection" "Host=...;Port=5432;Database=postgres;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true"

# 2. Inicialización del primer administrador (desactivar tras crearlo)
dotnet user-secrets --project LigaArmenia.Web set "AdminSeed:Enabled" "true"
dotnet user-secrets --project LigaArmenia.Web set "AdminSeed:Nombres" "Administrador Liga"
dotnet user-secrets --project LigaArmenia.Web set "AdminSeed:Correo" "admin@ligalaarmenia.org"
dotnet user-secrets --project LigaArmenia.Web set "AdminSeed:Password" "ClaveSegura#2026"

# 3. Configuración JWT para API móvil Android (clave >= 32 caracteres)
dotnet user-secrets --project LigaArmenia.Web set "Jwt:Key" "REEMPLAZAR_CON_CLAVE_SECRETA_ALEATORIA_DE_AL_MENOS_32_CARACTERES"
dotnet user-secrets --project LigaArmenia.Web set "Jwt:Issuer" "LigaArmenia.Api"
dotnet user-secrets --project LigaArmenia.Web set "Jwt:Audience" "LigaArmenia.Android"
dotnet user-secrets --project LigaArmenia.Web set "Jwt:ExpirationMinutes" "60"
```

### Producción (variables de entorno)
Equivalentes usando doble guion bajo `__`:
- `ConnectionStrings__DefaultConnection`
- `AdminSeed__Enabled` (mantener en `false` tras la inicialización inicial)
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__ExpirationMinutes`
