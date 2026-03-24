---
name: be-create-controller
description: >
  Utiliza este skill cuando necesites crear endpoints HTTP/API, manejar rutas REST, implementar operaciones CRUD, o añadir funcionalidad web a tu aplicación .NET. 
  Aplica cuando el usuario mencione "crear endpoint", "API REST", "controlador", "ruta HTTP", "operaciones CRUD", o cuando necesite exponer funcionalidad de negocio via web —
  incluso si no dice explícitamente "controller" o ".NET". Genera controllers completos con inyección de dependencias, manejo de errores, documentación Swagger, 
  autenticación y patrones estándar (GetById, Search paginado, Upsert). Úsalo también para APIs de archivos, autenticación, o cualquier endpoint web.
user-invocable: true
argument-hint: "Nombre del controller a crear (ej: 'ProductoController')"
metadata: 
  category: "Desarrollo"
  tags: [".NET Core", "Controllers", "Convenciones de Código"]
---

# SKILL: Creación de Controllers en .NET Core Web API

## Propósito
Utiliza este skill para crear un nuevo controller en el proyecto de .NET Core Web API siguiendo las convenciones y patrones establecidos en la arquitectura del proyecto. El controller generado incluirá la estructura base, inyección de dependencias, manejo de errores y documentación Swagger/OpenAPI.  

## Arquitectura de Controllers

### Estructura Estándar
Los controllers siguen un patrón consistente con las siguientes características:
1. **Herencia**: `ControllerBase` (para APIs, no MVC con vistas)
2. **Atributos estándar**: `[ApiController]`, `[Authorize]`, `[Route]` 
3. **Inyección de dependencias**: Constructor con `ILogger` y servicios específicos
4. **Documentación XML**: Swagger/OpenAPI con respuestas documentadas
5. **Manejo consistente de errores**: try-catch estándar con logging


### Perfomance
No exponer métodos que puedan retornar grandes cantidades de datos sin paginación o filtros adecuados. Evitar métodos como `FindAll` o `GetAll` si la entidad puede tener más de 50 registros.


### Estructura de Archivos
```
controllers/
├── AuthController.cs        # Autenticación OIDC especial
├── CrudController.cs        # Controllers de operaciones CRUD estándar


``` 

## Convenciones del Proyecto

### Namespaces y Referencias

```csharp
using bmlabs.core.dtos;           // DTOs del core
using bmlabs.core.entities;      // Entidades del core  
using bmlabs.core.services;      // Servicios del core
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;   // Para predicados de búsqueda


// Namespace principal 
namespace bmslabs.controllers  
{
    // Controller implementation
}
```

### Estructura Base del Controller

```csharp
/// <summary>
/// Controller para la gestión de [Entidad]
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class [Entidad]Controller : ControllerBase
{
    private readonly ILogger<[Entidad]Controller> _logger;
    private readonly I[Entidad]Service _[entidad]Service;

    /// <summary>
    /// Constructor del controller [Entidad]
    /// </summary>
    /// <param name="logger">Logger para registro de eventos</param>
    /// <param name="[entidad]Service">Servicio para operaciones de [Entidad]</param>
    public [Entidad]Controller(
        ILogger<[Entidad]Controller> logger,
        I[Entidad]Service [entidad]Service
    )
    {
        _logger = logger;
        _[entidad]Service = [entidad]Service;
    }
}
```

### Reglas de Nomenclatura

- **Controllers**: `{Nombre}Controller`
- **Rutas**: `api/v1/[controller]` (versioning explícito)
- **Métodos HTTP**: Nombres descriptivos del negocio, no genéricos
- **Parámetros**: camelCase en query parameters
- **Responses**: PascalCase en propiedades de respuesta
- **Servicios**: Inyección usando interfaces `I{Nombre}Service`

## Patrones de Endpoints Comunes

### 1. GetById - Obtener Entidad por ID

```csharp
/// <summary>
/// Obtiene una entidad por su ID
/// </summary>
/// <param name="id">ID único de la entidad</param>
/// <returns>Los datos de la entidad solicitada</returns>
/// <response code="200">Entidad encontrada exitosamente</response>
/// <response code="404">Entidad no encontrada</response>
/// <response code="400">ID inválido</response>
[HttpGet("{id}")]
public async Task<ActionResult<[Entidad]Dto>> GetById(Guid id)
{
    try
    {
        var entidad = await _[entidad]Service.GetByIdAsync(id);
        if (entidad == null)
        {
            return NotFound($"[Entidad] con ID {id} no encontrada");
        }
        return Ok(entidad);
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}
```

### 2. Search - Búsqueda Paginada con Filtros

```csharp
/// <summary>
/// Obtiene entidades paginadas con filtros opcionales
/// </summary>
/// <param name="nombre">Filtro por nombre (búsqueda parcial)</param>
/// <param name="activo">Filtro por estado activo/inactivo</param>
/// <param name="fechaCreacion">Filtro por fecha de creación exacta</param>
/// <param name="page">Número de página (base 1)</param>
/// <param name="pageSize">Cantidad de elementos por página (máximo 100)</param>
/// <returns>Lista paginada de entidades con información de navegación incluida</returns>
/// <response code="200">Lista de entidades obtenida exitosamente</response>
/// <response code="400">Parámetros de entrada inválidos</response>
[HttpGet("Search")]
public async Task<ActionResult<object>> Search(
    [FromQuery] string? nombre = null,
    [FromQuery] bool? activo = null,
    [FromQuery] DateTime? fechaCreacion = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    try
    {
        // Validar y normalizar parámetros de paginación
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        // Construir predicado de búsqueda
        Expression<Func<[Entidad], bool>> predicate = e =>
            (string.IsNullOrEmpty(nombre) || e.Nombre.ToLower().Contains(nombre.ToLower())) &&
            (activo == null || e.Activo == activo) &&
            (fechaCreacion == null || e.FechaCreacion.Date == fechaCreacion.Value.Date);

        // Ejecutar búsqueda paginada con navegación incluida
        var (items, totalCount) = await _[entidad]Service.SearchPagedAsync(predicate, page, pageSize, true);

        // Respuesta estándar con información de paginación
        return Ok(new
        {
            items,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        });
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}
```

### 3. Upsert - Crear o Actualizar (Patrón Preferido)

```csharp
/// <summary>
/// Crea una nueva entidad o actualiza una existente (Upsert)
/// </summary>
/// <param name="dto">Datos de la entidad a crear o actualizar</param>
/// <returns>Entidad creada o actualizada con información de si fue una creación (isNew = true) o actualización (isNew = false)</returns>
/// <response code="200">Entidad actualizada exitosamente</response>
/// <response code="201">Entidad creada exitosamente</response>
/// <response code="400">Datos de entrada inválidos</response>
/// <response code="404">Entidad no encontrada para actualización</response>
/// <response code="409">Conflicto con datos existentes (ej: nombre duplicado)</response>
[HttpPost("Upsert")]
public async Task<ActionResult<object>> Upsert([FromBody] [Entidad]Dto dto)
{
    try
    {
        if (dto == null)
        {
            return BadRequest("Los datos de la entidad son requeridos.");
        }

        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return BadRequest("El nombre de la entidad es requerido.");
        }

        // Validar ModelState para validaciones automáticas
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Realizar la operación upsert
        var (entidadResultado, isNew) = await _[entidad]Service.InsertOrUpdate(dto);

        var resultado = new
        {
            entidad = entidadResultado,
            isNew = isNew,
            message = isNew ? "Entidad creada exitosamente." : "Entidad actualizada exitosamente."
        };

        // Retornar el código de estado apropiado
        if (isNew)
        {
            return CreatedAtAction(nameof(GetById), new { id = entidadResultado.Id }, resultado);
        }
        else
        {
            return Ok(resultado);
        }
    }
    catch (ArgumentException ex)
    {
        return BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error en Upsert - Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}
```

### 4. CreateOrUpdate - Alternativa al Upsert

```csharp
/// <summary>
/// Crea una entidad nueva o actualiza una existente
/// </summary>
/// <param name="dto">Datos de la entidad a crear o actualizar</param>
/// <returns>La entidad creada o actualizada</returns>
/// <response code="200">Entidad procesada exitosamente</response>
/// <response code="400">Datos inválidos</response>
[HttpPatch("CreateOrUpdate")]
public async Task<ActionResult<[Entidad]Dto>> CreateOrUpdate([Entidad]Dto dto)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (entidad, isNew) = await _[entidad]Service.InsertOrUpdate(dto);
        return Ok(entidad);
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}
```

## Patrones Avanzados

### Búsqueda por ID: GUID Completo o Primeros 8 Caracteres

```csharp
/// <summary>
/// Patrón específico del proyecto para buscar por ID flexible
/// </summary>
[HttpGet("Search")]
public async Task<ActionResult<object>> Search([FromQuery] string? id = null, /* otros parámetros */)
{
    try
    {
        // Lógica para búsqueda por ID: GUID completo o primeros 8 caracteres
        Guid? guidId = null;
        bool searchByPartialId = false;

        if (!string.IsNullOrEmpty(id))
        {
            // Intentar parsear como GUID completo
            if (Guid.TryParse(id, out var parsedGuid))
            {
                guidId = parsedGuid;
            }
            // Si no es un GUID completo y tiene exactamente 8 caracteres, buscar por coincidencia parcial
            else if (id.Length == 8 && id.All(c => char.IsLetterOrDigit(c) || c == '-'))
            {
                searchByPartialId = true;
            }
        }

        Expression<Func<[Entidad], bool>> predicate = e =>
            (string.IsNullOrEmpty(id) ||
            (guidId.HasValue && e.Id == guidId.Value) ||
            (searchByPartialId && e.Id.ToString().StartsWith(id)));

        // Continuar con búsqueda...
    }
    catch (Exception ex)
    {
        // Manejo de errores estándar
    }
}
```

### Controller con Archivos (Multipart/Form-Data)

```csharp
/// <summary>
/// Endpoint que acepta archivos junto con datos JSON
/// Acepta datos JSON con archivos opcionales usando multipart/form-data
/// </summary>
/// <param name="request">Request que contiene los datos y archivos opcionales</param>
/// <returns>Entidad creada o actualizada con información de archivos subidos</returns>
[HttpPost("Upsert")]
public async Task<ActionResult<object>> Upsert([FromForm] [Entidad]UpsertRequestDto request)
{
    try
    {
        // Validar archivos si existen
        if (request.Archivos != null && request.Archivos.Any())
        {
            foreach (var archivo in request.Archivos)
            {
                // Validar tipo de archivo, tamaño, etc.
                if (archivo.Length > 10 * 1024 * 1024) // 10MB
                {
                    return BadRequest($"El archivo {archivo.FileName} excede el tamaño máximo permitido.");
                }
            }
        }

        // Procesar entidad con archivos
        var resultado = await _[entidad]Service.UpsertWithFiles(request);
        
        return Ok(resultado);
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}
```

### Controller de Autenticación (Caso Especial)

```csharp
/// <summary>
/// Controller especial para autenticación OIDC
/// No requiere [Authorize] en todos los endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUsuarioService _usuarioService;
    
    public AuthController(IConfiguration configuration, IUsuarioService usuarioService)
    {
        _configuration = configuration;
        _usuarioService = usuarioService;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var scheme = HttpContext.Request.IsHttps ? "https" : "http";
        var host = HttpContext.Request.Host.Value;
        var redirectUrl = $"{scheme}://{host}{Url.Action(nameof(Token), "Auth")}";

        return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, 
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        try
        {
            // Extraer claims del usuario autenticado
            var correoUsuario = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == "user_email" || c.Type == "preferred_username")?.Value;

            if (string.IsNullOrEmpty(correoUsuario))
            {
                return Unauthorized("No se pudo obtener el email del usuario autenticado");
            }

            var usuario = await _usuarioService.FindByEmail(correoUsuario);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado en el sistema");
            }

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
            return Problem(detail: ex.Message, title: "ERROR");
        }
    }
}
```

## Manejo de Errores Estándar

### Patrón de Try-Catch Consistente

```csharp
try
{
    // Lógica del endpoint
    
    return Ok(resultado);
}
catch (ArgumentException ex)
{
    return BadRequest(ex.Message);
}
catch (InvalidOperationException ex)
{
    return NotFound(ex.Message);
}
catch (Exception ex)
{
    // Obtener la excepción raíz
    while (ex.InnerException != null) ex = ex.InnerException;
    
    // Log con formato estándar del proyecto
    _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
    
    // Respuesta de error estándar
    return Problem(detail: ex.Message, title: "ERROR");
}
```

### Tipos de Excepciones y Respuestas

- **ArgumentException** → `400 BadRequest` con mensaje específico
- **InvalidOperationException** → `404 NotFound` con mensaje específico  
- **Otras excepciones** → `500 Problem` con detalles del error solo en logs y mensajes generico para el cliente (sin información sensible) indicando un idlog para rastreo en logs.
- **ModelState inválido** → `400 BadRequest(ModelState)`

### Logging Estándar

```csharp
// Para errores generales
_logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");

// Para información contextual
_logger.LogInformation("Operación completada para entidad {EntityType} con ID {Id}", typeof(Entity).Name, id);

// Para advertencias
_logger.LogWarning("Datos inválidos en solicitud: {Error}", ex.Message);
```

## Validaciones en Controllers

### Validación de ModelState

```csharp
[HttpPost("Create")]
public async Task<ActionResult<[Entidad]Dto>> Create([Entidad]Dto dto)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Procesar entidad válida
        var resultado = await _[entidad]Service.Insert(dto);
        return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
    }
    catch (Exception ex)
    {
        // Manejo de errores
    }
}
```

### Validaciones Manuales Específicas

```csharp
[HttpPost("Upsert")]
public async Task<ActionResult<object>> Upsert([FromBody] [Entidad]Dto dto)
{
    try
    {
        // Validaciones de null
        if (dto == null)
        {
            return BadRequest("Los datos de la entidad son requeridos.");
        }

        // Validaciones de campos requeridos
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return BadRequest("El nombre de la entidad es requerido.");
        }

        // Validaciones de Guid vacíos
        if (dto.IdCategoria == Guid.Empty)
        {
            return BadRequest("La categoría es requerida.");
        }

        // Validaciones de rangos
        if (dto.Precio <= 0)
        {
            return BadRequest("El precio debe ser mayor a 0.");
        }

        // Continuar con procesamiento
    }
    catch (Exception ex)
    {
        // Manejo de errores
    }
}
```

### Validación de Parámetros de Paginación

```csharp
[HttpGet("Search")]
public async Task<ActionResult<object>> Search(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    try
    {
        // Normalizar parámetros de paginación
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Límite máximo del proyecto

        // Continuar con búsqueda
    }
    catch (Exception ex)
    {
        // Manejo de errores
    }
}
```

## Códigos de Estado HTTP

### Códigos Estándar del Proyecto

- **200 OK**: Operado exitosamente (GET, PUT, PATCH exitosos)
- **201 Created**: Recurso creado (POST exitoso con CreatedAtAction)
- **204 No Content**: Operado exitosamente sin contenido (DELETE)
- **400 Bad Request**: Datos inválidos, ModelState inválido
- **401 Unauthorized**: No autenticado
- **403 Forbidden**: No autorizado (aunque no se ve explícitamente en los ejemplos)
- **404 Not Found**: Recurso no encontrado
- **409 Conflict**: Conflicto de datos (duplicados, etc.) - implícito
- **500 Internal Server Error**: Error interno (via Problem())

### Respuestas Consistentes

```csharp
// Éxito con datos
return Ok(datos);

// Creación exitosa
return CreatedAtAction(nameof(GetById), new { id = entidad.Id }, entidad);

// Recurso no encontrado
return NotFound($"Entidad con ID {id} no encontrada");

// Datos inválidos
return BadRequest("Mensaje específico del error");
return BadRequest(ModelState);

// Error interno
return Problem(detail: ex.Message, title: "ERROR");

// No autorizado
return Unauthorized("Mensaje específico");
```

## Responses y DTOs

### Respuesta de Búsqueda Paginada Estándar

```csharp
return Ok(new
{
    items,                // List<EntidadDto> - Los elementos de la página actual
    totalCount,           // int - Total de elementos que cumplen el criterio
    page,                 // int - Página actual (base 1)
    pageSize,            // int - Tamaño de página utilizado
    totalPages = (int)Math.Ceiling((double)totalCount / pageSize) // int - Total de páginas
});
```

### Respuesta de Upsert

```csharp
return Ok(new
{
    entidad = entidadResultado,    // EntidadDto - La entidad procesada
    isNew = isNew,                 // bool - true si fue creación, false si fue actualización
    message = isNew ? "Entidad creada exitosamente." : "Entidad actualizada exitosamente."
});
```

### Respuesta de Operaciones Asíncronas

```csharp
return Ok(new
{
    success = true,
    message = "Procesamiento iniciado",
    detalles = $"Se están procesando {elementos.Count} elementos en segundo plano",
    cantidadElementos = elementos.Count,
    procesamientoAsincrono = true
});
```

## Autorización y Autenticación

### Autorización a Nivel de Controller

```csharp
[ApiController]
[Authorize]  // Todos los endpoints requieren autenticación
[Route("api/v1/[controller]")]
public class [Entidad]Controller : ControllerBase
{
    // Todos los métodos heredan [Authorize]
}
```

### Endpoints Públicos en Controller Autorizado

```csharp
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class [Entidad]Controller : ControllerBase
{
    [AllowAnonymous]  // Este endpoint específico no requiere autenticación
    [HttpGet("Public")]
    public async Task<ActionResult> GetPublicData()
    {
        // Lógica para datos públicos
    }
}
```

### Obtener Usuario Actual desde Claims

```csharp
[Authorize]
[HttpGet("profile")]
public async Task<IActionResult> Profile()
{
    // Obtener email del usuario autenticado desde claims
    var correoUsuario = HttpContext.User.Claims
        .FirstOrDefault(c => c.Type == "user_email" || c.Type == "preferred_username")?.Value;

    if (string.IsNullOrEmpty(correoUsuario))
    {
        return Unauthorized("No se pudo obtener el email del usuario autenticado");
    }

    // Buscar usuario en el sistema
    var usuario = await _usuarioService.FindByEmail(correoUsuario);
    
    return Ok(usuario);
}
```

## Documentación Swagger/OpenAPI

### Documentación Completa de Endpoint

```csharp
/// <summary>
/// Obtiene entidades paginadas con filtros opcionales
/// </summary>
/// <param name="nombre">Filtro por nombre (búsqueda parcial case-insensitive)</param>
/// <param name="activo">Filtro por estado activo/inactivo</param>
/// <param name="fechaCreacion">Filtro por fecha de creación exacta (formato: YYYY-MM-DD)</param>
/// <param name="idCategoria">Filtro por ID de la categoría</param>
/// <param name="page">Número de página (base 1, mínimo: 1, defecto: 1)</param>
/// <param name="pageSize">Cantidad de elementos por página (mínimo: 1, máximo: 100, defecto: 10)</param>
/// <returns>Lista paginada de entidades con información de navegación incluida</returns>
/// <response code="200">Lista de entidades obtenida exitosamente</response>
/// <response code="400">Parámetros de entrada inválidos</response>
/// <response code="401">No autenticado</response>
/// <response code="500">Error interno del servidor</response>
[HttpGet("Search")]
public async Task<ActionResult<object>> Search(
    [FromQuery] string? nombre = null,
    [FromQuery] bool? activo = null,
    [FromQuery] DateTime? fechaCreacion = null,
    [FromQuery] Guid? idCategoria = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    // Implementación
}
```

### Documentación de Controller

```csharp
/// <summary>
/// Controller para la gestión de [Entidad]s
/// 
/// Provee endpoints para operaciones CRUD completas incluyendo:
/// - Consulta individual por ID
/// - Listado completo de entidades
/// - Búsqueda paginada con filtros múltiples
/// - Creación y actualización mediante patrón Upsert
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class [Entidad]Controller : ControllerBase
```

## Inyección de Dependencias

### Constructor Estándar

```csharp
public class [Entidad]Controller : ControllerBase
{
    private readonly ILogger<[Entidad]Controller> _logger;
    private readonly I[Entidad]Service _[entidad]Service;

    public [Entidad]Controller(
        ILogger<[Entidad]Controller> logger,
        I[Entidad]Service [entidad]Service
    )
    {
        _logger = logger;
        _[entidad]Service = [entidad]Service;
    }
}
```

### Constructor con Múltiples Servicios

```csharp
public class [Entidad]Controller : ControllerBase
{
    private readonly ILogger<[Entidad]Controller> _logger;
    private readonly I[Entidad]Service _[entidad]Service;
    private readonly IRelatedService _relatedService;
    private readonly IUsuarioService _usuarioService; // Para obtener usuario actual

    public [Entidad]Controller(
        ILogger<[Entidad]Controller> logger,
        I[Entidad]Service [entidad]Service,
        IRelatedService relatedService,
        IUsuarioService usuarioService
    )
    {
        _logger = logger;
        _[entidad]Service = [entidad]Service;
        _relatedService = relatedService;
        _usuarioService = usuarioService;
    }
}
```

### Inyección de Dependencias Opcionales

```csharp
public class EppController : ControllerBase
{
    private readonly ILogger<EppController> _logger;
    private readonly IEppService _eppService;
    private readonly IS3Service _s3Service; // Servicio externo opcional

    public EppController(
        ILogger<EppController> logger,
        IEppService eppService,
        IS3Service s3Service // Sin '?' porque está registrado en DI
    )
    {
        _logger = logger;
        _eppService = eppService;
        _s3Service = s3Service;
    }
}
```

## Operaciones Específicas del Negocio

### Endpoints de Relaciones Many-to-Many

```csharp
/// <summary>
/// Obtiene todos los recintos asociados a un usuario
/// </summary>
/// <param name="usuarioId">ID del usuario</param>
/// <returns>Lista de recintos asociados al usuario</returns>
[HttpGet("usuario/{usuarioId}/recintos")]
public async Task<ActionResult<List<RecintoDto>>> GetRecintosByUsuario(Guid usuarioId)
{
    try
    {
        var recintos = await _usuarioRecintoService.GetRecintosByUsuarioIdAsync(usuarioId);
        return Ok(recintos);
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}

/// <summary>
/// Actualiza las relaciones usuario-recinto
/// </summary>
/// <param name="usuarioId">ID del usuario</param>
/// <param name="recintoIds">Lista de IDs de recintos a asociar</param>
[HttpPost("usuario/{usuarioId}/recintos")]
public async Task<ActionResult> UpdateUsuarioRecintos(Guid usuarioId, [FromBody] List<Guid> recintoIds)
{
    try
    {
        await _usuarioRecintoService.UpsertRecintosForUsuarioAsync(usuarioId, recintoIds);
        return Ok(new { message = "Recintos actualizados exitosamente" });
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}
```

### Endpoints con Carga Masiva

```csharp
/// <summary>
/// Carga masiva de usuarios desde archivo Excel
/// </summary>
/// <param name="archivo">Archivo Excel con usuarios</param>
/// <param name="idUsuarioResponsable">ID del usuario que ejecuta la carga</param>
/// <returns>Resultado de la carga masiva con estadísticas</returns>
[HttpPost("CargaMasiva")]
public async Task<ActionResult<object>> CargaMasiva(
    [FromForm] IFormFile archivo,
    [FromForm] Guid idUsuarioResponsable)
{
    try
    {
        // Validar archivo
        if (archivo == null || archivo.Length == 0)
        {
            return BadRequest("El archivo es requerido.");
        }

        if (!archivo.FileName.EndsWith(".xlsx") && !archivo.FileName.EndsWith(".xls"))
        {
            return BadRequest("Solo se permiten archivos Excel (.xlsx, .xls).");
        }

        if (archivo.Length > 10 * 1024 * 1024) // 10MB
        {
            return BadRequest("El archivo excede el tamaño máximo permitido (10MB).");
        }

        var resultado = await _usuarioService.CargaMasivaAsync(archivo, idUsuarioResponsable);

        return Ok(new
        {
            message = "Carga masiva completada",
            totalProcesados = resultado.TotalProcesados,
            exitosos = resultado.Exitosos,
            errores = resultado.Errores,
            detallesErrores = resultado.DetallesErrores
        });
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error en carga masiva - Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}
```

### Endpoints de Descarga/Exportación

```csharp
/// <summary>
/// Descarga reporte de entidades en formato Excel
/// </summary>
/// <param name="fechaInicio">Fecha de inicio del reporte</param>
/// <param name="fechaFin">Fecha de fin del reporte</param>
/// <param name="incluirInactivos">Incluir entidades inactivas</param>
/// <returns>Archivo Excel con el reporte</returns>
[HttpGet("DescargarReporte")]
public async Task<IActionResult> DescargarReporte(
    [FromQuery] DateTime? fechaInicio = null,
    [FromQuery] DateTime? fechaFin = null,
    [FromQuery] bool incluirInactivos = false)
{
    try
    {
        var archivoBytes = await _[entidad]Service.GenerarReporteExcel(fechaInicio, fechaFin, incluirInactivos);
        
        var nombreArchivo = $"Reporte_[Entidades]_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        return File(archivoBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error en descarga de reporte - Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}
```

## Controllers Especializados

### Controller de Métricas/Dashboard

```csharp
/// <summary>
/// Controller para métricas y estadísticas del sistema
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class MetricasController : ControllerBase
{
    private readonly ILogger<MetricasController> _logger;
    private readonly IMetricasService _metricasService;

    public MetricasController(ILogger<MetricasController> logger, IMetricasService metricasService)
    {
        _logger = logger;
        _metricasService = metricasService;
    }

    /// <summary>
    /// Obtiene métricas del dashboard principal
    /// </summary>
    [HttpGet("Dashboard")]
    public async Task<ActionResult<DashboardDto>> GetMetricasDashboard()
    {
        try
        {
            var metricas = await _metricasService.GetDashboardMetricsAsync();
            return Ok(metricas);
        }
        catch (Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
            return Problem(detail: ex.Message, title: "ERROR");
        }
    }

    /// <summary>
    /// Obtiene métricas filtradas por período
    /// </summary>
    [HttpGet("Periodo")]
    public async Task<ActionResult<MetricasPeriodoDto>> GetMetricasPeriodo(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] string? tipoMetrica = null)
    {
        try
        {
            if (fechaFin <= fechaInicio)
            {
                return BadRequest("La fecha de fin debe ser mayor a la fecha de inicio.");
            }

            var metricas = await _metricasService.GetMetricasPeriodoAsync(fechaInicio, fechaFin, tipoMetrica);
            return Ok(metricas);
        }
        catch (Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
            return Problem(detail: ex.Message, title: "ERROR");
        }
    }
}
```

### Controller de Archivos/Storage

```csharp
/// <summary>
/// Controller para manejo de archivos y almacenamiento
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class StorageController : ControllerBase
{
    private readonly ILogger<StorageController> _logger;
    private readonly IS3Service _s3Service;

    public StorageController(ILogger<StorageController> logger, IS3Service s3Service)
    {
        _logger = logger;
        _s3Service = s3Service;
    }

    /// <summary>
    /// Sube un archivo al almacenamiento
    /// </summary>
    [HttpPost("Upload")]
    public async Task<ActionResult<object>> UploadFile([FromForm] IFormFile archivo, [FromForm] string? carpeta = null)
    {
        try
        {
            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest("El archivo es requerido.");
            }

            // Validaciones de archivo
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx" };
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            
            if (!extensionesPermitidas.Contains(extension))
            {
                return BadRequest($"Tipo de archivo no permitido. Extensiones permitidas: {string.Join(", ", extensionesPermitidas)}");
            }

            if (archivo.Length > 50 * 1024 * 1024) // 50MB
            {
                return BadRequest("El archivo excede el tamaño máximo permitido (50MB).");
            }

            var resultado = await _s3Service.UploadFileAsync(archivo, carpeta);

            return Ok(new
            {
                fileName = resultado.FileName,
                fileUrl = resultado.FileUrl,
                fileSize = archivo.Length,
                uploadedAt = DateTime.UtcNow,
                message = "Archivo subido exitosamente"
            });
        }
        catch (Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            _logger.LogError($"Error en upload de archivo - Source:{ex.Source}, Trace:{ex}");
            return Problem(detail: ex.Message, title: "ERROR");
        }
    }

    /// <summary>
    /// Descarga un archivo por su nombre
    /// </summary>
    [HttpGet("Download/{fileName}")]
    public async Task<IActionResult> DownloadFile(string fileName, [FromQuery] string? carpeta = null)
    {
        try
        {
            var archivoStream = await _s3Service.DownloadFileAsync(fileName, carpeta);
            
            if (archivoStream == null)
            {
                return NotFound($"Archivo {fileName} no encontrado.");
            }

            var contentType = GetContentType(fileName);
            return File(archivoStream, contentType, fileName);
        }
        catch (Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            _logger.LogError($"Error en descarga de archivo - Source:{ex.Source}, Trace:{ex}");
            return Problem(detail: ex.Message, title: "ERROR");
        }
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }
}
```

## Testing y Calidad

### Endpoints [NonAction] para Testing

```csharp
/// <summary>
/// Método marcado como [NonAction] - presente pero no expuesto como endpoint
/// Usado para compatibilidad o testing interno
/// </summary>
[NonAction]
[HttpPost("Create")]
public async Task<ActionResult<[Entidad]Dto>> Create([Entidad]Dto dto)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entidad = await _[entidad]Service.Insert(dto);
        return CreatedAtAction(nameof(GetById), new { id = entidad.Id }, entidad);
    }
    catch (Exception ex)
    {
        while (ex.InnerException != null) ex = ex.InnerException;
        _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
        return Problem(detail: ex.Message, title: "ERROR");
    }
}
```

## Plantilla Completa de Controller

```csharp
using bmlabs.core.dtos;
using bmlabs.core.entities;
using bmlabs.core.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace bmslabs.controllers
{
    /// <summary>
    /// Controller para la gestión de [Entidad]s
    /// 
    /// Provee endpoints para operaciones CRUD completas incluyendo:
    /// - Consulta individual por ID
    /// - Listado completo de entidades
    /// - Búsqueda paginada con filtros múltiples
    /// - Creación y actualización mediante patrón Upsert
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class [Entidad]Controller : ControllerBase
    {
        private readonly ILogger<[Entidad]Controller> _logger;
        private readonly I[Entidad]Service _[entidad]Service;

        /// <summary>
        /// Constructor del controller [Entidad]
        /// </summary>
        /// <param name="logger">Logger para registro de eventos</param>
        /// <param name="[entidad]Service">Servicio para operaciones de [Entidad]</param>
        public [Entidad]Controller(
            ILogger<[Entidad]Controller> logger,
            I[Entidad]Service [entidad]Service
        )
        {
            _logger = logger;
            _[entidad]Service = [entidad]Service;
        }

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        /// <param name="id">ID único de la entidad</param>
        /// <returns>Los datos de la entidad solicitada</returns>
        /// <response code="200">Entidad encontrada exitosamente</response>
        /// <response code="404">Entidad no encontrada</response>
        /// <response code="400">ID inválido</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<[Entidad]Dto>> GetById(Guid id)
        {
            try
            {
                var entidad = await _[entidad]Service.GetByIdAsync(id);
                if (entidad == null)
                {
                    return NotFound($"[Entidad] con ID {id} no encontrada");
                }
                return Ok(entidad);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
                return Problem(detail: ex.Message, title: "ERROR");
            }
        }



        /// <summary>
        /// Obtiene entidades paginadas con filtros opcionales
        /// </summary>
        /// <param name="nombre">Filtro por nombre (búsqueda parcial case-insensitive)</param>
        /// <param name="activo">Filtro por estado activo/inactivo</param>
        /// <param name="fechaCreacion">Filtro por fecha de creación exacta</param>
        /// <param name="page">Número de página (base 1, mínimo: 1, defecto: 1)</param>
        /// <param name="pageSize">Cantidad de elementos por página (mínimo: 1, máximo: 100, defecto: 10)</param>
        /// <returns>Lista paginada de entidades con información de navegación incluida</returns>
        /// <response code="200">Lista de entidades obtenida exitosamente</response>
        /// <response code="400">Parámetros de entrada inválidos</response>
        [HttpGet("Search")]
        public async Task<ActionResult<object>> Search(
            [FromQuery] string? nombre = null,
            [FromQuery] bool? activo = null,
            [FromQuery] DateTime? fechaCreacion = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validar y normalizar parámetros de paginación
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                // Construir predicado de búsqueda
                Expression<Func<[Entidad], bool>> predicate = e =>
                    (string.IsNullOrEmpty(nombre) || e.Nombre.ToLower().Contains(nombre.ToLower())) &&
                    (activo == null || e.Activo == activo) &&
                    (fechaCreacion == null || e.FechaCreacion.Date == fechaCreacion.Value.Date);

                // Ejecutar búsqueda paginada con navegación incluida
                var (items, totalCount) = await _[entidad]Service.SearchPagedAsync(predicate, page, pageSize, true);

                // Respuesta estándar con información de paginación
                return Ok(new
                {
                    items,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                _logger.LogError($"Error Source:{ex.Source}, Trace:{ex}");
                return Problem(detail: ex.Message, title: "ERROR");
            }
        }

        /// <summary>
        /// Crea una nueva entidad o actualiza una existente (Upsert)
        /// </summary>
        /// <param name="dto">Datos de la entidad a crear o actualizar</param>
        /// <returns>Entidad creada o actualizada con información de si fue una creación o actualización</returns>
        /// <response code="200">Entidad actualizada exitosamente</response>
        /// <response code="201">Entidad creada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Entidad no encontrada para actualización</response>
        /// <response code="409">Conflicto con datos existentes</response>
        [HttpPost("Upsert")]
        public async Task<ActionResult<object>> Upsert([FromBody] [Entidad]Dto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Los datos de la entidad son requeridos.");
                }

                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    return BadRequest("El nombre de la entidad es requerido.");
                }

                // Validar ModelState para validaciones automáticas
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Realizar la operación upsert
                var (entidadResultado, isNew) = await _[entidad]Service.InsertOrUpdate(dto);

                var resultado = new
                {
                    entidad = entidadResultado,
                    isNew = isNew,
                    message = isNew ? "Entidad creada exitosamente." : "Entidad actualizada exitosamente."
                };

                // Retornar el código de estado apropiado
                if (isNew)
                {
                    return CreatedAtAction(nameof(GetById), new { id = entidadResultado.Id }, resultado);
                }
                else
                {
                    return Ok(resultado);
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                _logger.LogError($"Error en Upsert - Source:{ex.Source}, Trace:{ex}");
                return Problem(detail: ex.Message, title: "ERROR");
            }
        }
    }
}
```

## Checklist para Nuevo Controller

- [ ] Archivo creado en `controllers/`
- [ ] Namespace correcto siguiendo convención del proyecto
- [ ] Imports correctos según funcionalidad requerida
- [ ] Atributos de clase: `[ApiController]`, `[Authorize]`, `[Route("api/v1/[controller]")]`
- [ ] Constructor con `ILogger<T>` y servicios necesarios
- [ ] Comentarios XML para clase, métodos y parámetros
- [ ] Endpoint GetById con validación de null y NotFound
- [ ] Endpoint GetAll para listado completo
- [ ] Endpoint Search con paginación y filtros
- [ ] Endpoint Upsert con validaciones y respuesta differenciada por isNew
- [ ] Try-catch consistente en todos los endpoints
- [ ] Logging con formato estándar del proyecto
- [ ] Validación de parámetros de paginación (page >= 1, pageSize 1-100)
- [ ] Respuestas estándar con códigos HTTP correctos
- [ ] ModelState.IsValid para validaciones de DTO
- [ ] Validaciones manuales para campos críticos
- [ ] Documentación Swagger con response codes
- [ ] Manejo de archivos si aplica (FromForm, validaciones de tamaño y tipo)
- [ ] Endpoints especializados según lógica de negocio
- [ ] Métodos [NonAction] para testing o compatibilidad si es necesario
- [ ] No exponer metodos FindAll o GetAll si la entidad pudiese tener mas de 50 registros
- [ ] Autorización apropiada por endpoint si se requiere diferente de [Authorize]

