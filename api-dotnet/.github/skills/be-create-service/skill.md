---
name: be-create-service
description: Guía para crear servicios en .NET Core siguiendo las convenciones del proyecto.
user-invocable: true
argument-hint: "Nombre del servicio a crear (ej: 'ProductoService')"
metadata: 
  category: "Desarrollo"
  tags: [".NET Core", "Servicios", "Convenciones de Código"]
---

# SKILL: Creación de Services en .NET Core con CrudService Base

## Propósito
Este skill proporciona las convenciones y patrones para crear services en .NET Core siguiendo las prácticas establecidas en el proyecto SSA-EPP-BackOffice-API, basándose en el archivo `core/services/base/CrudService.cs` existente como base para todos los servicios.

## Arquitectura Base

### Service Pattern implementado
El proyecto utiliza el patrón Service con una arquitectura de 3 capas:
1. **Interfaz genérica base**: `ICrudService<TEntity, TDto, TRepository>` - Define el contrato común
2. **Implementación genérica base**: `CrudService<TEntity, TDto, TRepository>` - Implementa operaciones CRUD básicas  
3. **Services específicos**: Heredan de la base y agregan funcionalidad específica del negocio

### Estructura de archivos
```
core/services/
├── base/
│   └── CrudService.cs              # Servicio base genérico (YA EXISTE)
├── AreaService.cs                  # Service específico simple
├── UsuarioService.cs               # Service con lógica adicional
├── UsuarioRecintoService.cs        # Service con relaciones many-to-many
└── DependencyInjectionService.cs   # Registro de DI
```

Si el  archivo `core/services/base/CrudService.cs` no existe debe ser creado con la siguiente implementación 

```csharp
using AutoMapper;
using bmlabs_ssaepp_core.exceptions;
using bmlabs.core.repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace bmlabs.core.services;

/// <summary>
/// Interfaz genérica para servicios CRUD
/// </summary>
/// <typeparam name="TEntity">Tipo de entidad</typeparam>
/// <typeparam name="TDto">Tipo de DTO</typeparam>
/// <typeparam name="TRepository">Tipo de repositorio</typeparam>
public interface ICrudService<TEntity, TDto, TRepository>
where TEntity : class
where TDto : class
where TRepository : IRepository<TEntity>
{
    /// <summary>
    /// Obtiene el contexto de la base de datos
    /// </summary>
    DbContext Context { get; }
    
    /// <summary>
    /// Lista todos los elementos
    /// </summary>
    Task<List<TDto>> ListAsync();
    
    /// <summary>
    /// Obtiene un elemento por ID (string)
    /// </summary>
    Task<TDto> GetByIdAsync(string id);
    
    /// <summary>
    /// Obtiene un elemento por ID (Guid)
    /// </summary>
    Task<TDto> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Inserta o actualiza un elemento
    /// </summary>
    Task<(TDto, bool)> InsertOrUpdate(TDto dto);
    
    /// <summary>
    /// Inserta o actualiza un elemento con opción de actualizar solo campos no nulos
    /// </summary>
    Task<(TDto, bool)> InsertOrUpdate(TDto dto, bool updateOnlyNonNullFields);
    
    /// <summary>
    /// Inserta un nuevo elemento
    /// </summary>
    Task<TDto> Insert(TDto dto);
    
    /// <summary>
    /// Actualiza un elemento existente
    /// </summary>
    Task<TDto> Update(TDto dto);
    
    /// <summary>
    /// Busca elementos que cumplan con un predicado
    /// </summary>
    Task<List<TDto>> SearchAsync(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// Busca elementos paginados que cumplan con un predicado
    /// </summary>
    Task<(List<TDto> items, int totalCount)> SearchPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize);
}

/// <summary>
/// Implementación base para servicios CRUD genéricos
/// </summary>
/// <typeparam name="TEntity">Tipo de entidad</typeparam>
/// <typeparam name="TDto">Tipo de DTO</typeparam>
/// <typeparam name="TRepository">Tipo de repositorio</typeparam>
public class CrudService<TEntity, TDto, TRepository> : ICrudService<TEntity, TDto, TRepository>
    where TEntity : class
    where TDto : class
    where TRepository : IRepository<TEntity>
{
    protected readonly TRepository _repository;
    protected readonly IMapper _mapper;
    protected readonly ILogger _logger;

    /// <summary>
    /// Constructor del servicio CRUD
    /// </summary>
    /// <param name="logger">Logger para registro de eventos</param>
    /// <param name="mapper">Mapper para conversión entre entidades y DTOs</param>
    /// <param name="repository">Repositorio para acceso a datos</param>
    public CrudService(
        ILogger<CrudService<TEntity, TDto, TRepository>> logger,
        IMapper mapper,
        TRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    /// <summary>
    /// Obtiene el contexto de la base de datos
    /// </summary>
    public DbContext Context => _repository.Context();

    /// <summary>
    /// Lista todos los elementos
    /// </summary>
    public virtual async Task<List<TDto>> ListAsync()
    {
        var entities = await _repository.FindAll();
        return _mapper.Map<List<TDto>>(entities);
    }

    /// <summary>
    /// Obtiene un elemento por ID (string)
    /// </summary>
    public virtual async Task<TDto> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ServiceException(nameof(id));

        var entity = await _repository.FindById(Guid.Parse(id));
        return _mapper.Map<TDto>(entity);
    }

    /// <summary>
    /// Obtiene un elemento por ID (Guid)
    /// </summary>
    public virtual async Task<TDto> GetByIdAsync(Guid id)
    {
        var entity = await _repository.FindById(id);
        return _mapper.Map<TDto>(entity);
    }

    /// <summary>
    /// Inserta o actualiza un elemento
    /// </summary>
    public virtual async Task<(TDto, bool)> InsertOrUpdate(TDto dto)
    {
        var (entity, isNew) = await _repository.InsertOrUpdate(_mapper.Map<TEntity>(dto));
        return (_mapper.Map<TDto>(entity), isNew);
    }

    /// <summary>
    /// Inserta o actualiza un elemento con opción de actualizar solo campos no nulos
    /// </summary>
    public virtual async Task<(TDto, bool)> InsertOrUpdate(TDto dto, bool updateOnlyNonNullFields)
    {
        var (entity, isNew) = await _repository.InsertOrUpdate(_mapper.Map<TEntity>(dto), updateOnlyNonNullFields);
        return (_mapper.Map<TDto>(entity), isNew);
    }

    /// <summary>
    /// Inserta un nuevo elemento
    /// </summary>
    public virtual async Task<TDto> Insert(TDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);
        entity = await _repository.Insert(entity);
        return _mapper.Map<TDto>(entity);
    }

    /// <summary>
    /// Actualiza un elemento existente
    /// </summary>
    public virtual async Task<TDto> Update(TDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);
        entity = await _repository.Update(entity);
        return _mapper.Map<TDto>(entity);
    }

    /// <summary>
    /// Busca elementos que cumplan con un predicado
    /// </summary>
    public virtual async Task<List<TDto>> SearchAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = await _repository.FindBy(predicate);
        return _mapper.Map<List<TDto>>(entities);
    }

    /// <summary>
    /// Busca elementos paginados que cumplan con un predicado
    /// </summary>
    public virtual async Task<(List<TDto> items, int totalCount)> SearchPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize)
    {        
        var items = await _repository.FindBy(predicate, page, pageSize);
        var totalCount = items.Count;
        return (items: _mapper.Map<List<TDto>>(items), totalCount);
    } 
}

```

Si el archivo `core/services/base/CrudService.cs` ya existe, asegúrate de que su implementación siga el patrón y las convenciones descritas anteriormente para garantizar la consistencia en los servicios específicos que se creen posteriormente.

Los servicios específicos que hereden de `CrudService` podrán sobrescribir los métodos base para agregar lógica adicional, validaciones o incluir propiedades de navegación según sea necesario, manteniendo siempre la estructura y las buenas prácticas establecidas en el proyecto.

Si una clase de servicio no existe en el proyecto, se puede crear siguiendo la estructura y los patrones descritos en esta guía, asegurando que se registre correctamente en el contenedor de dependencias para su uso en la aplicación.


## Convenciones del Proyecto

### Interfaz Específica

```csharp
using AutoMapper;
using bmlabs.core.dtos;
using bmlabs.core.entities;
using bmlabs.core.repositories;
using Microsoft.Extensions.Logging;

namespace bmlabs.core.services;

/// <summary>
/// Interfaz del servicio para la entidad [Entidad]
/// </summary>
public interface I[Entidad]Service : ICrudService<[Entidad], [Entidad]Dto, I[Entidad]Repository>
{
    // Métodos específicos adicionales si los hay
}
```

### Implementación Específica Simple

```csharp
using AutoMapper;
using bmlabs.core.dtos;
using bmlabs.core.entities;
using bmlabs.core.repositories;
using Microsoft.Extensions.Logging;

namespace bmlabs.core.services;

/// <summary>
/// Servicio específico para la entidad [Entidad]
/// </summary>
public class [Entidad]Service : CrudService<[Entidad], [Entidad]Dto, I[Entidad]Repository>, I[Entidad]Service
{
    /// <summary>
    /// Constructor del servicio [Entidad]
    /// </summary>
    /// <param name="logger">Logger para registro de eventos</param>
    /// <param name="mapper">Mapper para conversión entre entidades y DTOs</param>
    /// <param name="repository">Repositorio para acceso a datos de [Entidad]</param>
    public [Entidad]Service(ILogger<[Entidad]Service> logger, IMapper mapper, I[Entidad]Repository repository) 
        : base(logger, mapper, repository)
    {
    }
}
```

### Reglas de Nomenclatura

- **Interfaces**: `I{NombreEntidad}Service`
- **Clases**: `{NombreEntidad}Service`
- **Namespace**: `bmlabs.core.services`
- **Archivos**: `{NombreEntidad}Service.cs`
- **Herencia**: `CrudService<TEntidad, TEntidadDto, ITEntidadRepository>`
- **Implementación**: `I{NombreEntidad}Service`

## Funcionalidades del CrudService Base

### Operaciones CRUD Disponibles

#### Consultas
```csharp
// Listar todos los elementos
Task<List<TDto>> ListAsync();

// Obtener por ID
Task<TDto> GetByIdAsync(string id);
Task<TDto> GetByIdAsync(Guid id);

// Búsqueda con predicados
Task<List<TDto>> SearchAsync(Expression<Func<TEntity, bool>> predicate);

// Búsqueda paginada
Task<(List<TDto> items, int totalCount)> SearchPagedAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize);
```

#### Modificaciones
```csharp
// Insertar nuevo elemento
Task<TDto> Insert(TDto dto);

// Actualizar elemento existente
Task<TDto> Update(TDto dto);

// Insertar o actualizar según si existe
Task<(TDto, bool)> InsertOrUpdate(TDto dto);

// Insertar o actualizar solo campos no nulos
Task<(TDto, bool)> InsertOrUpdate(TDto dto, bool updateOnlyNonNullFields);
```

#### Acceso al Contexto
```csharp
// Obtener contexto de base de datos
DbContext Context { get; }
```

## Implementaciones por Complejidad

### Service Simple (Sin Lógica Adicional)

```csharp
using AutoMapper;
using bmlabs.core.dtos;
using bmlabs.core.entities;
using bmlabs.core.repositories;
using Microsoft.Extensions.Logging;

namespace bmlabs.core.services;

/// <summary>
/// Interfaz del servicio para la entidad Area
/// </summary>
public interface IAreaService : ICrudService<Area, AreaDto, IAreaRepository>
{
}

/// <summary>
/// Servicio específico para la entidad Area
/// </summary>
public class AreaService : CrudService<Area, AreaDto, IAreaRepository>, IAreaService
{
    /// <summary>
    /// Constructor del servicio Area
    /// </summary>
    /// <param name="logger">Logger para registro de eventos</param>
    /// <param name="mapper">Mapper para conversión entre entidades y DTOs</param>
    /// <param name="repository">Repositorio para acceso a datos de Area</param>
    public AreaService(ILogger<AreaService> logger, IMapper mapper, IAreaRepository repository) : base(logger, mapper, repository)
    {
    }
}
```

### Service con Relaciones Many-to-Many

```csharp
using AutoMapper;
using bmlabs.core.dtos;
using bmlabs.core.entities;
using bmlabs.core.repositories;
using Microsoft.Extensions.Logging;

namespace bmlabs.core.services;

/// <summary>
/// Interfaz del servicio para relaciones Usuario-Recinto
/// </summary>
public interface IUsuarioRecintoService : ICrudService<UsuarioRecinto, UsuarioRecintoDto, IUsuarioRecintoRepository>
{
    Task<List<RecintoDto>> GetRecintosByUsuarioIdAsync(Guid usuarioId);
    Task UpsertRecintosForUsuarioAsync(Guid usuarioId, List<Guid> recintoIds);
}

/// <summary>
/// Servicio para gestionar relaciones Many-to-Many entre Usuario y Recinto
/// </summary>
public class UsuarioRecintoService : CrudService<UsuarioRecinto, UsuarioRecintoDto, IUsuarioRecintoRepository>, IUsuarioRecintoService
{
    /// <summary>
    /// Constructor del servicio UsuarioRecinto
    /// </summary>
    /// <param name="logger">Logger para registro de eventos</param>
    /// <param name="mapper">Mapper para conversión entre entidades y DTOs</param>
    /// <param name="repository">Repositorio para acceso a datos de UsuarioRecinto</param>
    public UsuarioRecintoService(ILogger<UsuarioRecintoService> logger, IMapper mapper, IUsuarioRecintoRepository repository) : base(logger, mapper, repository)
    {
    }

    /// <summary>
    /// Obtiene todos los recintos asociados a un usuario
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <returns>Lista de recintos del usuario</returns>
    public async Task<List<RecintoDto>> GetRecintosByUsuarioIdAsync(Guid usuarioId)
    {
        // Buscar con navegación incluida para obtener los datos del recinto
        var usuarioRecintos = await _repository.FindBy(ur => ur.IdUsuario == usuarioId, includeNavigationProperties: true);
        return usuarioRecintos.Select(ur => _mapper.Map<RecintoDto>(ur.IdRecintoNavigation)).ToList();
    }

    /// <summary>
    /// Actualiza las relaciones usuario-recinto (upsert pattern para many-to-many)
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <param name="recintoIds">Lista de IDs de recintos a asociar</param>
    public async Task UpsertRecintosForUsuarioAsync(Guid usuarioId, List<Guid> recintoIds)
    {
        // Obtener relaciones existentes
        var existingRecintos = await _repository.FindBy(ur => ur.IdUsuario == usuarioId);
        var existingRecintoIds = existingRecintos.Select(ur => ur.IdRecinto).ToHashSet();

        // Agregar nuevos recintos (los que están en la nueva lista pero no en la existente)
        foreach (var recintoId in recintoIds)
        {
            if (!existingRecintoIds.Contains(recintoId))
            {
                var usuarioRecinto = new UsuarioRecinto
                {
                    IdUsuario = usuarioId,
                    IdRecinto = recintoId
                };
                await _repository.Insert(usuarioRecinto);
            }
        }

        // Eliminar recintos que ya no están en la lista (los que están en existente pero no en nueva)
        foreach (var usuarioRecinto in existingRecintos)
        {
            if (!recintoIds.Contains((Guid)usuarioRecinto.IdRecinto))
            {
                await _repository.Delete(usuarioRecinto);
            }
        }
    }
}
```

### Service con Métodos Adicionales y Múltiples Repositorios

```csharp
using System.Linq.Expressions;
using AutoMapper;
using bmlabs.core.dtos;
using bmlabs.core.entities;
using bmlabs.core.repositories;
using Microsoft.Extensions.Logging;

namespace bmlabs.core.services;

/// <summary>
/// Interfaz del servicio para la entidad Epp
/// </summary>
public interface IEppService : ICrudService<Epp, EppDto, IEppRepository>
{
    Task<(List<EppDto> items, int totalCount)> SearchPagedAsync(Expression<Func<Epp, bool>> predicate, int page, int pageSize, bool includeNavigationProperties = false);
    Task<List<EppDto>> ListAsync(Expression<Func<Epp, bool>> predicate, bool includeNavigationProperties = false);
    Task<(EppDto epp, bool isNew)> UpsertAsync(EppDto eppDto);
    Task<List<PeriocidadEpp>> GetPeriocidadByEppIdAsync(Guid eppId);
    Task<bool> UpsertPeriocidadEppAsync(List<PeriocidadEppDto> periodicidades);
}

/// <summary>
/// Servicio específico para la entidad Epp
/// </summary>
public class EppService : CrudService<Epp, EppDto, IEppRepository>, IEppService
{
    private readonly IBitacoraEppRepository _bitacoraEppRepository;
    private readonly IS3Service? _s3Service;
    private readonly IHistorialPrecioEppRepository _historialPrecioEppRepository;

    /// <summary>
    /// Constructor del servicio Epp
    /// </summary>
    /// <param name="logger">Logger para registro de eventos</param>
    /// <param name="mapper">Mapper para conversión entre entidades y DTOs</param>
    /// <param name="repository">Repositorio para acceso a datos de Epp</param>
    /// <param name="bitacoraEppRepository">Repositorio para bitácora de EPP</param>
    /// <param name="s3Service">Servicio S3 para manejo de archivos (opcional)</param>
    /// <param name="historialPrecioEppRepository">Repositorio para historial de precios de EPP</param>
    public EppService(
        ILogger<EppService> logger, 
        IMapper mapper, 
        IEppRepository repository, 
        IBitacoraEppRepository bitacoraEppRepository,
        IHistorialPrecioEppRepository historialPrecioEppRepository, 
        IS3Service? s3Service = null) 
        : base(logger, mapper, repository)
    {
        _bitacoraEppRepository = bitacoraEppRepository;
        _s3Service = s3Service;
        _historialPrecioEppRepository = historialPrecioEppRepository;
    }

    // Implementación de métodos específicos...
    public async Task<(List<EppDto> items, int totalCount)> SearchPagedAsync(Expression<Func<Epp, bool>> predicate, int page, int pageSize, bool includeNavigationProperties = false)
    {
        var (entities, totalCount) = await _repository.FindByPaged(predicate, page, pageSize, includeNavigationProperties);
        var dtos = _mapper.Map<List<EppDto>>(entities);
        return (dtos, totalCount);
    }

    public async Task<(EppDto epp, bool isNew)> UpsertAsync(EppDto eppDto)
    {
        var entity = _mapper.Map<Epp>(eppDto);
        var (updatedEntity, isNew) = await _repository.InsertOrUpdate(entity);
        var resultDto = _mapper.Map<EppDto>(updatedEntity);
        return (resultDto, isNew);
    }

    // Más métodos específicos...
}
```

## Patrones de Uso Comunes

### Patrón Upsert para Relaciones Many-to-Many

```csharp
/// <summary>
/// Patrón estándar para actualizar relaciones many-to-many
/// </summary>
public async Task UpsertRelacionesAsync(Guid entidadPrincipalId, List<Guid> entidadesRelacionadasIds)
{
    // 1. Obtener relaciones existentes
    var existingRelaciones = await _repository.FindBy(r => r.IdEntidadPrincipal == entidadPrincipalId);
    var existingIds = existingRelaciones.Select(r => r.IdEntidadRelacionada).ToHashSet();

    // 2. Agregar nuevas relaciones
    foreach (var nuevaId in entidadesRelacionadasIds)
    {
        if (!existingIds.Contains(nuevaId))
        {
            var nuevaRelacion = new EntidadRelacion
            {
                IdEntidadPrincipal = entidadPrincipalId,
                IdEntidadRelacionada = nuevaId
            };
            await _repository.Insert(nuevaRelacion);
        }
    }

    // 3. Eliminar relaciones obsoletas
    foreach (var relacionExistente in existingRelaciones)
    {
        if (!entidadesRelacionadasIds.Contains(relacionExistente.IdEntidadRelacionada))
        {
            await _repository.Delete(relacionExistente);
        }
    }
}
```

### Búsquedas con Navegación

```csharp
/// <summary>
/// Obtener entidades relacionadas con navegación incluida
/// </summary>
public async Task<List<TRelacionadaDto>> GetEntidadesRelacionadasAsync(Guid entidadId)
{
    // Usar includeNavigationProperties = true para cargar las propiedades de navegación
    var relaciones = await _repository.FindBy(r => r.IdEntidadPrincipal == entidadId, includeNavigationProperties: true);
    
    // Mapear las entidades relacionadas usando las propiedades de navegación
    return relaciones.Select(r => _mapper.Map<TRelacionadaDto>(r.EntityNavigation)).ToList();
}
```

### Validaciones en Services

```csharp
public async Task<TDto> ValidatedInsert(TDto dto)
{
    // Validaciones de negocio
    if (dto.Campo == null)
        throw new ServiceException("Campo requerido");

    // Validar duplicados
    var existing = await _repository.FindBy(x => x.Codigo == dto.Codigo);
    if (existing.Any())
        throw new ServiceException("Ya existe un elemento con ese código");

    return await base.Insert(dto);
}
```

### Override de Métodos Base

```csharp
/// <summary>
/// Override del método Insert para agregar lógica de negocio
/// </summary>
public override async Task<TDto> Insert(TDto dto)
{
    // Validaciones específicas
    if (string.IsNullOrEmpty(dto.Nombre))
        throw new ServiceException("El nombre es requerido");

    // Lógica de negocio antes de insertar
    dto.FechaCreacion = DateTime.UtcNow;
    dto.Estado = "ACTIVO";

    // Llamar al método base
    var result = await base.Insert(dto);

    // Lógica post-inserción (bitácora, notificaciones, etc.)
    _logger.LogInformation("Entidad creada exitosamente con ID {Id}", result.Id);

    return result;
}
```

## Transacciones con Múltiples Operaciones

```csharp
public async Task<bool> ComplexBusinessOperation(ComplexDto dto)
{
    using var transaction = Context.Database.BeginTransaction();
    try
    {
        // Operación 1: Crear entidad principal
        var mainEntity = await Insert(dto.MainData);

        // Operación 2: Crear entidades relacionadas
        foreach (var related in dto.RelatedItems)
        {
            related.MainEntityId = mainEntity.Id;
            await _relatedRepository.Insert(_mapper.Map<RelatedEntity>(related));
        }

        // Operación 3: Actualizar relaciones many-to-many
        await UpsertRelacionesAsync(mainEntity.Id, dto.RelatedIds);

        await transaction.CommitAsync();
        return true;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

## Registro de Dependencias

### Extensión para DI

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace bmlabs.core.services
{
    public static class DependencyInjectionService
    {
        public static void AddServices(IServiceCollection services)
        {
            // Registro de servicios específicos por entidad
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUsuarioRecintoService, UsuarioRecintoService>();
            services.AddScoped<IEppService, EppService>();
            services.AddScoped<ISolicitudService, SolicitudService>();
            // ... más servicios
        }
    }
}
```

### Uso en Program.cs

```csharp
// Registrar servicios
builder.Services.AddServices();
```

## Uso en Controllers

### Inyección en Constructor

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsuarioRecintoController : ControllerBase
{
    private readonly IUsuarioRecintoService _usuarioRecintoService;

    public UsuarioRecintoController(IUsuarioRecintoService usuarioRecintoService)
    {
        _usuarioRecintoService = usuarioRecintoService;
    }
}
```

### Operaciones Específicas de Relaciones

```csharp
[HttpGet("usuario/{usuarioId}/recintos")]
public async Task<ActionResult<List<RecintoDto>>> GetRecintosByUsuario(Guid usuarioId)
{
    var recintos = await _usuarioRecintoService.GetRecintosByUsuarioIdAsync(usuarioId);
    return Ok(recintos);
}

[HttpPost("usuario/{usuarioId}/recintos")]
public async Task<ActionResult> UpdateUsuarioRecintos(Guid usuarioId, [FromBody] List<Guid> recintoIds)
{
    await _usuarioRecintoService.UpsertRecintosForUsuarioAsync(usuarioId, recintoIds);
    return Ok();
}
```

## Mejores Prácticas

### Separación de Responsabilidades
- **Services**: Lógica de negocio y orquestación
- **Repositories**: Acceso a datos únicamente  
- **Controllers**: Manejo HTTP y validación de contratos
- **DTOs**: Contratos de entrada y salida

### Manejo de Relaciones Many-to-Many
- **HashSet para comparación**: Usar `ToHashSet()` para comparaciones eficientes
- **Operaciones separadas**: Primero agregar nuevas, luego eliminar obsoletas
- **Include Navigation**: Usar `includeNavigationProperties: true` cuando necesites datos relacionados

### Performance
- **Paginación**: Usar métodos paginados para listas grandes
- **Navegación selectiva**: Solo cargar propiedades de navegación cuando sea necesario
- **Operaciones bulk**: Para muchas inserciones/eliminaciones, considerar operaciones masivas

### Logging y Monitoreo
```csharp
public async Task<TDto> TrackedOperation(TDto dto)
{
    _logger.LogInformation("Iniciando operación para entidad {EntityType} con ID {Id}", typeof(TEntity).Name, dto.Id);
    
    try
    {
        var result = await base.Insert(dto);
        _logger.LogInformation("Operación completada exitosamente para entidad {EntityType} con ID {Id}", typeof(TEntity).Name, result.Id);
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en operación para entidad {EntityType}", typeof(TEntity).Name);
        throw;
    }
}
```

## Checklist para Nuevo Service

- [ ] Archivo creado en `core/services/`
- [ ] Namespace correcto: `bmlabs.core.services`
- [ ] Imports correctos: `using Microsoft.Extensions.Logging;`
- [ ] Interfaz definida: `I{Entidad}Service : ICrudService<{Entidad}, {Entidad}Dto, I{Entidad}Repository>`
- [ ] Implementación: `{Entidad}Service : CrudService<{Entidad}, {Entidad}Dto, I{Entidad}Repository>, I{Entidad}Service`
- [ ] Constructor con ILogger, IMapper y Repository principales
- [ ] Comentarios XML para interfaz, clase y métodos públicos
- [ ] Métodos adicionales específicos del negocio si los hay
- [ ] Sistema upsert para relaciones many-to-many si aplica
- [ ] Override de métodos base si se necesita lógica adicional
- [ ] Inyección de repositorios adicionales si se requieren
- [ ] Registro en DependencyInjectionService.cs
- [ ] Validaciones de negocio apropiadas
- [ ] Manejo de errores con ServiceException
- [ ] Logging para operaciones importantes

## Plantilla Completa para Service con Relaciones

```csharp
using AutoMapper;
using bmlabs.core.dtos;
using bmlabs.core.entities;
using bmlabs.core.repositories;
using Microsoft.Extensions.Logging;

namespace bmlabs.core.services;

/// <summary>
/// Interfaz del servicio para relaciones EntityA-EntityB
/// </summary>
public interface IEntityAEntityBService : ICrudService<EntityAEntityB, EntityAEntityBDto, IEntityAEntityBRepository>
{
    Task<List<EntityBDto>> GetEntitiesBByEntityAIdAsync(Guid entityAId);
    Task UpsertEntitiesBForEntityAAsync(Guid entityAId, List<Guid> entityBIds);
    Task<bool> ExistsRelationAsync(Guid entityAId, Guid entityBId);
    Task RemoveAllRelationsForEntityAAsync(Guid entityAId);
}

/// <summary>
/// Servicio para gestionar relaciones Many-to-Many entre EntityA y EntityB
/// </summary>
public class EntityAEntityBService : CrudService<EntityAEntityB, EntityAEntityBDto, IEntityAEntityBRepository>, IEntityAEntityBService
{
    /// <summary>
    /// Constructor del servicio EntityAEntityB
    /// </summary>
    /// <param name="logger">Logger para registro de eventos</param>
    /// <param name="mapper">Mapper para conversión entre entidades y DTOs</param>
    /// <param name="repository">Repositorio para acceso a datos de EntityAEntityB</param>
    public EntityAEntityBService(ILogger<EntityAEntityBService> logger, IMapper mapper, IEntityAEntityBRepository repository) 
        : base(logger, mapper, repository)
    {
    }

    /// <summary>
    /// Obtiene todas las entidades B asociadas a una entidad A
    /// </summary>
    /// <param name="entityAId">ID de la entidad A</param>
    /// <returns>Lista de entidades B asociadas</returns>
    public async Task<List<EntityBDto>> GetEntitiesBByEntityAIdAsync(Guid entityAId)
    {
        var relations = await _repository.FindBy(r => r.IdEntityA == entityAId, includeNavigationProperties: true);
        return relations.Select(r => _mapper.Map<EntityBDto>(r.IdEntityBNavigation)).ToList();
    }

    /// <summary>
    /// Actualiza las relaciones EntityA-EntityB usando patrón upsert
    /// </summary>
    /// <param name="entityAId">ID de la entidad A</param>
    /// <param name="entityBIds">Lista de IDs de entidades B a asociar</param>
    public async Task UpsertEntitiesBForEntityAAsync(Guid entityAId, List<Guid> entityBIds)
    {
        var existingRelations = await _repository.FindBy(r => r.IdEntityA == entityAId);
        var existingEntityBIds = existingRelations.Select(r => r.IdEntityB).ToHashSet();

        // Agregar nuevas relaciones
        foreach (var entityBId in entityBIds)
        {
            if (!existingEntityBIds.Contains(entityBId))
            {
                var newRelation = new EntityAEntityB
                {
                    IdEntityA = entityAId,
                    IdEntityB = entityBId
                };
                await _repository.Insert(newRelation);
            }
        }

        // Eliminar relaciones obsoletas
        foreach (var existingRelation in existingRelations)
        {
            if (!entityBIds.Contains(existingRelation.IdEntityB))
            {
                await _repository.Delete(existingRelation);
            }
        }

        _logger.LogInformation("Relaciones actualizadas para EntityA {EntityAId}: {Count} relaciones", entityAId, entityBIds.Count);
    }

    /// <summary>
    /// Verifica si existe una relación específica
    /// </summary>
    /// <param name="entityAId">ID de la entidad A</param>
    /// <param name="entityBId">ID de la entidad B</param>
    /// <returns>True si existe la relación</returns>
    public async Task<bool> ExistsRelationAsync(Guid entityAId, Guid entityBId)
    {
        var relations = await _repository.FindBy(r => r.IdEntityA == entityAId && r.IdEntityB == entityBId);
        return relations.Any();
    }

    /// <summary>
    /// Elimina todas las relaciones para una entidad A específica
    /// </summary>
    /// <param name="entityAId">ID de la entidad A</param>
    public async Task RemoveAllRelationsForEntityAAsync(Guid entityAId)
    {
        var relations = await _repository.FindBy(r => r.IdEntityA == entityAId);
        foreach (var relation in relations)
        {
            await _repository.Delete(relation);
        }
        
        _logger.LogInformation("Todas las relaciones eliminadas para EntityA {EntityAId}", entityAId);
    }
}
```

