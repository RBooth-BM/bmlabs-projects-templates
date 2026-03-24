---
name: be-create-mappings
description: >
  Utiliza este skill cuando necesites convertir datos entre capas, transformar objetos, o configurar mapeos automáticos en .NET con AutoMapper.
  Aplica cuando el usuario mencione "mapear datos", "convertir entidades", "AutoMapper", "transformar objetos", "perfiles de mapeo",
  o cuando necesite conectar DTOs con entidades — incluso si no dice explícitamente "mapping" o "AutoMapper".
  Genera perfiles completos de mapeo con conversiones de tipos, propiedades de navegación, configuraciones personalizadas y documentación.
user-invocable: true
argument-hint: "Nombre del DTO a mapear (ej: 'ProductoDto')"
metadata: 
  category: "Desarrollo"
  tags: [".NET Core", "AutoMapper", "DTOs", "Convenciones de Código"]
---


# SKILL: Creación de Mappings en .NET Core con AutoMapper

## Propósito
Utiliza este skill para crear nuevos mappings usando AutoMapper en .NET Core siguiendo las convenciones y patrones establecidos. Los perfiles de mapeo generados incluirán la configuración necesaria para mapear entre entidades y DTOs, manejo de conversiones de tipos, mapeo de propiedades de navegación y documentación XML, asegurando que los mappings sigan las mejores prácticas de diseño y sean compatibles con el sistema de mapeo del proyecto. Este skill se basa en las mejores prácticas de diseño de mappings con AutoMapper en .NET Core, garantizando que los perfiles de mapeo sean claros, mantenibles y estén correctamente configurados para su uso en la aplicación.

## Convenciones del Proyecto

### Estructura Base de Profile

```csharp
using AutoMapper;
using bmlabs.core.entities;

namespace bmlabs.core.dtos.mapping;

/// <summary>
/// Perfil de mapeo para [Entidad] y [Entidad]Dto
/// </summary>
public class EntidadProfile : Profile
{
    public EntidadProfile()
    {
        // Mapeo de entidad a DTO
        CreateMap<Entidad, EntidadDto>();

        // Mapeo de DTO a entidad
        CreateMap<EntidadDto, Entidad>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Reglas de Nomenclatura

#### Archivos y Clases
- **Archivos**: `{NombreEntidad}Profile.cs`
- **Clases**: `{NombreEntidad}Profile`
- **Namespace**: `bmlabs.core.dtos.mapping`
- **Herencia**: `Profile` (AutoMapper)

### Ubicación
- **Carpeta**: `core/dtos/mapping/`
- **Un profile por entidad/DTO**
- **Un profile por archivo**

## Tipos de Mapeos

### Mapeo Simple (Propiedades Coincidentes)

```csharp
/// <summary>
/// Perfil de mapeo para Cargo y CargoDto
/// </summary>
public class CargoProfile : Profile
{
    public CargoProfile()
    {
        // Mapeo automático cuando las propiedades coinciden
        CreateMap<Cargo, CargoDto>();

        // Mapeo de vuelta con validación de null
        CreateMap<CargoDto, Cargo>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Mapeo con Propiedades Explícitas

```csharp
/// <summary>
/// Perfil de mapeo para Division y DivisionDto
/// </summary>
public class DivisionProfile : Profile
{
    public DivisionProfile()
    {
        // Mapeo explícito de propiedades
        CreateMap<Division, DivisionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<DivisionDto, Division>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Mapeo con Navegación Simple

```csharp
/// <summary>
/// Perfil de mapeo para Area y AreaDto
/// </summary>
public class AreaProfile : Profile
{
    public AreaProfile()
    {
        // Mapeo de entidad a DTO incluyendo navegación
        CreateMap<Area, AreaDto>()
            .ForMember(dest => dest.Division, opt => opt.MapFrom(src => src.IdDivisionNavigation))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Mapeo de DTO a entidad ignorando navegación
        CreateMap<AreaDto, Area>()
            .ForMember(dest => dest.IdDivisionNavigation, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

## Manejo de Fechas

### Conversión DateTime ↔ DateTimeOffset

```csharp
/// <summary>
/// Perfil de mapeo para Epp y EppDto
/// </summary>
public class EppProfile : Profile
{
    public EppProfile()
    {
        // Entidad → DTO: DateTime a DateTimeOffset (UTC)
        CreateMap<Epp, EppDto>()
            .ForMember(dest => dest.FechaUltModificacion, opt => opt.MapFrom(src =>
                new DateTimeOffset(src.FechaUltModificacion, TimeSpan.Zero)));

        // DTO → Entidad: DateTimeOffset a DateTime
        CreateMap<EppDto, Epp>()
            .ForMember(dest => dest.FechaUltModificacion, opt => opt.MapFrom(src => src.FechaUltModificacion.DateTime))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Fechas Nullable

```csharp
/// <summary>
/// Mapeo de fechas opcionales
/// </summary>
public class SolicitudProfile : Profile
{
    public SolicitudProfile()
    {
        CreateMap<Solicitud, SolicitudDto>()
            .ForMember(dest => dest.FechaSolicitud, opt => opt.MapFrom(src =>
                src.FechaSolicitud.HasValue ? new DateTimeOffset(src.FechaSolicitud.Value, TimeSpan.Zero) : (DateTimeOffset?)null))
            .ForMember(dest => dest.FechaResolucion, opt => opt.MapFrom(src =>
                src.FechaResolucion.HasValue ? new DateTimeOffset(src.FechaResolucion.Value, TimeSpan.Zero) : (DateTimeOffset?)null));

        CreateMap<SolicitudDto, Solicitud>()
            .ForMember(dest => dest.FechaSolicitud, opt => opt.MapFrom(src =>
                src.FechaSolicitud.HasValue ? src.FechaSolicitud.Value.DateTime : (DateTime?)null))
            .ForMember(dest => dest.FechaResolucion, opt => opt.MapFrom(src =>
                src.FechaResolucion.HasValue ? src.FechaResolucion.Value.DateTime : (DateTime?)null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

## Manejo de Navegación Compleja

### Mapeo con Múltiples Navegaciones

```csharp
/// <summary>
/// Perfil de mapeo complejo con múltiples navegaciones
/// </summary>
public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        // Entidad → DTO: Mapear todas las navegaciones
        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.FechaIngreso, opt => opt.MapFrom(src =>
                new DateTimeOffset(src.FechaIngreso, TimeSpan.Zero)))
            .ForMember(dest => dest.EstadoUsuario, opt => opt.MapFrom(src => src.IdUsuarioEstadoNavigation))
            .ForMember(dest => dest.Division, opt => opt.MapFrom(src => src.IdDivisionNavigation))
            .ForMember(dest => dest.SubArea, opt => opt.MapFrom(src => src.IdSubAreaNavigation))
            .ForMember(dest => dest.Cargo, opt => opt.MapFrom(src => src.IdCargoNavigation))
            .ForMember(dest => dest.Recinto, opt => opt.MapFrom(src => src.IdRecintoNavigation))
            .ForMember(dest => dest.CentroCosto, opt => opt.MapFrom(src => src.IdCentroCostoNavigation));

        // DTO → Entidad: Ignorar todas las navegaciones
        CreateMap<UsuarioDto, Usuario>()
            .ForMember(dest => dest.FechaIngreso, opt => opt.MapFrom(src => src.FechaIngreso.DateTime))
            .ForMember(dest => dest.IdUsuarioEstadoNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdDivisionNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdSubAreaNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdCargoNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdRecintoNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdCentroCostoNavigation, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Mapeo de Colecciones con LINQ

```csharp
/// <summary>
/// Mapeo de relaciones uno-a-muchos y muchos-a-muchos
/// </summary>
public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDto>()
            // Mapeo de colección con filtro LINQ
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                src.UsuarioRol.Where(ur => ur.IdRolNavigation != null).Select(ur => ur.IdRolNavigation).ToList()));

        CreateMap<UsuarioDto, Usuario>()
            // Ignorar colecciones en mapeo inverso
            .ForMember(dest => dest.UsuarioRol, opt => opt.Ignore())
            .ForMember(dest => dest.SolicitudIdResponsableResolucionNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.SolicitudIdUsuarioColaboradorNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.SolicitudIdUsuarioResponsableNavigation, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Mapeo de Colecciones

```csharp
/// <summary>
/// Mapeo con colecciones simples
/// </summary>
public class SolicitudProfile : Profile
{
    public SolicitudProfile()
    {
        CreateMap<Solicitud, SolicitudDto>()
            // Mapeo directo de colección
            .ForMember(dest => dest.Seguimientos, opt => opt.MapFrom(src => src.Seguimiento));

        CreateMap<SolicitudDto, Solicitud>()
            // Ignorar colección en mapeo inverso
            .ForMember(dest => dest.Seguimiento, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

## Plantillas por Complejidad

### Profile Simple (Sin Navegación)

```csharp
using AutoMapper;
using bmlabs.core.entities;

namespace bmlabs.core.dtos.mapping;

/// <summary>
/// Perfil de mapeo para [Entidad] y [Entidad]Dto
/// </summary>
public class EntidadSimpleProfile : Profile
{
    public EntidadSimpleProfile()
    {
        // Mapeo automático bidireccional
        CreateMap<EntidadSimple, EntidadSimpleDto>();

        CreateMap<EntidadSimpleDto, EntidadSimple>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Profile con Fechas

```csharp
using AutoMapper;
using bmlabs.core.entities;

namespace bmlabs.core.dtos.mapping;

/// <summary>
/// Perfil de mapeo para [Entidad] y [Entidad]Dto con fechas
/// </summary>
public class EntidadConFechasProfile : Profile
{
    public EntidadConFechasProfile()
    {
        CreateMap<EntidadConFechas, EntidadConFechasDto>()
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src =>
                new DateTimeOffset(src.FechaCreacion, TimeSpan.Zero)))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src =>
                src.FechaModificacion.HasValue ? new DateTimeOffset(src.FechaModificacion.Value, TimeSpan.Zero) : (DateTimeOffset?)null));

        CreateMap<EntidadConFechasDto, EntidadConFechas>()
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion.DateTime))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src =>
                src.FechaModificacion.HasValue ? src.FechaModificacion.Value.DateTime : (DateTime?)null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Profile con Navegación

```csharp
using AutoMapper;
using bmlabs.core.entities;

namespace bmlabs.core.dtos.mapping;

/// <summary>
/// Perfil de mapeo para [Entidad] y [Entidad]Dto con navegación
/// </summary>
public class EntidadConNavegacionProfile : Profile
{
    public EntidadConNavegacionProfile()
    {
        CreateMap<EntidadConNavegacion, EntidadConNavegacionDto>()
            .ForMember(dest => dest.EntidadPadre, opt => opt.MapFrom(src => src.IdEntidadPadreNavigation))
            .ForMember(dest => dest.EntidadesHijas, opt => opt.MapFrom(src => src.EntidadesHijas))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<EntidadConNavegacionDto, EntidadConNavegacion>()
            .ForMember(dest => dest.IdEntidadPadreNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.EntidadesHijas, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

### Profile Complejo (Todo Incluido)

```csharp
using AutoMapper;
using bmlabs.core.entities;

namespace bmlabs.core.dtos.mapping;

/// <summary>
/// Perfil de mapeo complejo para [Entidad] y [Entidad]Dto
/// </summary>
public class EntidadComplejaProfile : Profile
{
    public EntidadComplejaProfile()
    {
        CreateMap<EntidadCompleja, EntidadComplejaDto>()
            // Conversión de fechas
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src =>
                new DateTimeOffset(src.FechaCreacion, TimeSpan.Zero)))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src =>
                src.FechaModificacion.HasValue ? new DateTimeOffset(src.FechaModificacion.Value, TimeSpan.Zero) : (DateTimeOffset?)null))
            // Navegaciones simples
            .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.IdCategoriaNavigation))
            .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.IdUsuarioNavigation))
            // Colecciones con filtros
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src =>
                src.Items.Where(i => i.Activo).ToList()))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                src.EntidadRoles.Where(er => er.IdRolNavigation != null).Select(er => er.IdRolNavigation).ToList()));

        CreateMap<EntidadComplejaDto, EntidadCompleja>()
            // Conversión de fechas
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion.DateTime))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src =>
                src.FechaModificacion.HasValue ? src.FechaModificacion.Value.DateTime : (DateTime?)null))
            // Ignorar navegaciones
            .ForMember(dest => dest.IdCategoriaNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForMember(dest => dest.EntidadRoles, opt => opt.Ignore())
            // Validación de null
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

## Patrones de Conversión Específicos

### DateTime ↔ DateTimeOffset

```csharp
// Entidad → DTO (DateTime → DateTimeOffset UTC)
.ForMember(dest => dest.Fecha, opt => opt.MapFrom(src =>
    new DateTimeOffset(src.Fecha, TimeSpan.Zero)))

// DTO → Entidad (DateTimeOffset → DateTime)
.ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => src.Fecha.DateTime))
```

### DateTime? ↔ DateTimeOffset?

```csharp
// Entidad → DTO (DateTime? → DateTimeOffset?)
.ForMember(dest => dest.Fecha, opt => opt.MapFrom(src =>
    src.Fecha.HasValue ? new DateTimeOffset(src.Fecha.Value, TimeSpan.Zero) : (DateTimeOffset?)null))

// DTO → Entidad (DateTimeOffset? → DateTime?)
.ForMember(dest => dest.Fecha, opt => opt.MapFrom(src =>
    src.Fecha.HasValue ? src.Fecha.Value.DateTime : (DateTime?)null))
```

### Navegación por ID

```csharp
// Mapear navegación a DTO
.ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.IdCategoriaNavigation))

// Ignorar navegación en mapeo inverso
.ForMember(dest => dest.IdCategoriaNavigation, opt => opt.Ignore())
```

### Colecciones Filtradas

```csharp
// Filtrar y mapear colección
.ForMember(dest => dest.ItemsActivos, opt => opt.MapFrom(src =>
    src.Items.Where(i => i.Activo == true).ToList()))

// Relación muchos-a-muchos
.ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
    src.UsuarioRoles.Where(ur => ur.IdRolNavigation != null).Select(ur => ur.IdRolNavigation).ToList()))
```

## Mejores Prácticas

### Validación de Null
- **Siempre usar**: `.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null))`
- **En mapeo DTO → Entidad**: Previene sobrescribir propiedades con valores null

### Manejo de fechas
- **UTC por defecto**: Usar `TimeSpan.Zero` en DateTimeOffset
- **Consistencia**: Siempre DateTime en entidades, DateTimeOffset en DTOs
- **Null safety**: Validar HasValue antes de convertir

### Navegaciones
- **Entidad → DTO**: Mapear navegaciones completas
- **DTO → Entidad**: Ignorar todas las navegaciones con `opt.Ignore()`
- **Evitar ciclos**: AutoMapper maneja automáticamente referencias circulares

### Colecciones
- **Filtros LINQ**: Aplicar en mapeo Entidad → DTO
- **Ignorar en reversa**: Siempre ignorar colecciones en DTO → Entidad
- **Performance**: Considerar lazy loading vs eager loading

### Documentación
- **Comentarios XML**: Obligatorios para cada profile
- **Separar mapeos**: Comentar claramente dirección de mapeo
- **Casos especiales**: Documentar lógica compleja

## Integración con el Sistema

### Registro en DI
```csharp
// Program.cs
services.AddAutoMapper(typeof(AreaProfile).Assembly);

// O registro manual
services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AreaProfile>();
    cfg.AddProfile<UsuarioProfile>();
    // etc...
});
```

### Uso en Services
```csharp
public class MiService
{
    private readonly IMapper _mapper;

    public MiService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<EntidadDto> GetByIdAsync(Guid id)
    {
        var entidad = await _repository.GetByIdAsync(id);
        return _mapper.Map<EntidadDto>(entidad);
    }

    public async Task<EntidadDto> CreateAsync(EntidadDto dto)
    {
        var entidad = _mapper.Map<Entidad>(dto);
        var resultado = await _repository.CreateAsync(entidad);
        return _mapper.Map<EntidadDto>(resultado);
    }
}
```

## Checklist para Nuevo Profile

- [ ] Archivo creado en `core/dtos/mapping/`
- [ ] Namespace correcto: `bmlabs.core.dtos.mapping`
- [ ] Herencia de `Profile`
- [ ] Comentario XML explicando el propósito
- [ ] Mapeo bidireccional (Entidad ↔ DTO)
- [ ] Conversión correcta de fechas (DateTime ↔ DateTimeOffset)
- [ ] Mapeo de propiedades de navegación
- [ ] Ignorar navegaciones en mapeo DTO → Entidad
- [ ] Usar `.ForAllMembers(opts => opts.Condition(...))` en mapeo DTO → Entidad
- [ ] Manejar colecciones apropiadamente
- [ ] Filtros LINQ donde sea necesario
- [ ] Comentarios que separen direcciones de mapeo

