# Sistema de Diseño UI/UX y Recursos Institucionales — Liga Barrial La Armenia

Esta guía detalla cómo reutilizar el sistema visual oscuro-esmeralda en nuevos módulos y cómo incorporar manualmente los logotipos e imágenes oficiales de la liga.

---

## 1. Hojas de estilo y tokens CSS

Todas las vistas administrativas heredan automáticamente `Views/Shared/_Layout.cshtml`, que carga:

1. `wwwroot/css/design-tokens.css` — Variables CSS de colores, tipografía, espaciados, bordes, sombras y transiciones.
2. `wwwroot/css/components.css` — Clases reutilizables (`.la-card`, `.la-btn`, `.la-table`, `.la-badge`, `.la-alert`, `.la-empty-state`, `.la-form-control`, etc.).
3. `wwwroot/css/layout.css` — Sidebar colapsable, topbar y área de contenido responsiva.
4. `wwwroot/js/layout.js` y `wwwroot/js/security-ui.js` — Comportamiento del sidebar, toggle de contraseñas y modal de confirmación.

Evita crear estilos inline o duplicar CSS en cada vista. Utiliza las variables CSS existentes (`var(--la-accent-primary)`, `var(--la-surface-card)`, `var(--la-text-secondary)`, etc.).

---

## 2. Componentes reutilizables para nuevos módulos

### Encabezado de página
```html
<div class="la-page-header">
    <div>
        <span class="la-page-eyebrow">Módulo · Subsección</span>
        <h1 class="la-page-title">Título del Módulo</h1>
        <p class="la-page-description">Descripción breve de la pantalla.</p>
    </div>
    <a asp-action="Create" class="la-btn la-btn-primary">Nuevo registro</a>
</div>
```

### Tarjeta de contenido y tabla moderna
```html
<div class="la-card mb-4">
    <!-- Filtros o contenido -->
</div>

<div class="la-table-wrap">
    <table class="la-table">
        <thead>
            <tr>
                <th>Columna 1</th>
                <th class="text-end">Acciones</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Dato</td>
                <td class="text-end">
                    <a class="la-btn la-btn-secondary la-btn-sm">Editar</a>
                </td>
            </tr>
        </tbody>
    </table>
</div>
```

### Botones disponibles
- `.la-btn .la-btn-primary` (verde esmeralda — acción principal)
- `.la-btn .la-btn-secondary` (acción secundaria / volver / cancelar)
- `.la-btn .la-btn-warning` (acciones de precaución)
- `.la-btn .la-btn-danger` (desactivar / eliminar)
- `.la-btn .la-btn-ghost` (acción discreta)
- Modificador de tamaño: `.la-btn-sm`

### Alertas automáticas con `TempData`
Desde cualquier controlador MVC, asigna alguna de estas claves y `_Alerts.cshtml` las mostrará automáticamente:
- `TempData["SuccessMessage"]`
- `TempData["WarningMessage"]`
- `TempData["ErrorMessage"]`
- `TempData["InfoMessage"]`

### Modal de confirmación reutilizable
Agrega atributos `data-confirm-*` a cualquier `<form method="post">` sensible:
```html
<form asp-action="Desactivar" method="post"
      data-confirm-title="Confirmar acción"
      data-confirm-message="¿Deseas desactivar este registro?"
      data-confirm-button="Sí, desactivar"
      data-confirm-variant="danger">
    @Html.AntiForgeryToken()
    <button type="submit" class="la-btn la-btn-danger la-btn-sm">Desactivar</button>
</form>
```

### Agregar enlaces al menú lateral (`_Sidebar.cshtml`)
Cuando un nuevo módulo esté implementado, reemplaza su bloque `.la-nav-upcoming` en `LigaArmenia.Web/Views/Shared/Partials/_Sidebar.cshtml` por un enlace `<a asp-controller="..." asp-action="Index">` protegido con `@if (Can(SystemPermissions.ModuloConsultar))`.

---

## 3. Incorporación manual de logotipos e imágenes institucionales

El sistema utiliza `IWebAssetHelper` para detectar automáticamente los archivos en `wwwroot/images/`.
Mientras los archivos no existan, se muestran placeholders limpios **sin errores 404 ni iconos de imágenes rotas**.

Solo debes copiar tus archivos oficiales en las rutas exactas indicadas abajo:

| Recurso | Ruta exacta en `LigaArmenia.Web/` | Formato recomendado | Dimensiones recomendadas |
|---|---|---|---|
| Logotipo institucional (Login y respaldo de Sidebar) | `wwwroot/images/branding/logo-institucional.png` | PNG con fondo transparente | `256 x 256 px` (cuadrado 1:1) |
| Logotipo compacto del Sidebar (opcional) | `wwwroot/images/branding/logo-sidebar.png` | PNG con fondo transparente | `128 x 128 px` (cuadrado 1:1) |
| Favicon del navegador | `wwwroot/images/branding/favicon.ico` | ICO (multi-tamaño 16/32/48) | `32 x 32 px` |
| Imagen principal / deportiva del Login | `wwwroot/images/login/login-hero.jpg` (o `.png`) | JPG optimizado o PNG | `1200 x 800 px` (horizontal 3:2, < 400 KB) |
| Fotografías institucionales / deportivas generales | `wwwroot/images/general/` | JPG / WebP / PNG | Según sección (ej. `1200 x 675 px` 16:9) |

Una vez copiados los archivos en estas rutas, recarga el navegador (`Ctrl + F5`) y el sistema los mostrará automáticamente sin modificar código.
