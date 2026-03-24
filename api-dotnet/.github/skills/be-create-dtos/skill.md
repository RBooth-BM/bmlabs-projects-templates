---
name: be-create-dtos
description: >
  Utiliza este skill cuando necesites crear contratos de datos, objetos de transferencia, o modelos para APIs en .NET. 
  Aplica cuando el usuario mencione "crear DTO", "modelos de datos", "contratos API", "request/response objects", "serialización JSON", 
  o cuando necesite estructuras de datos para comunicación entre capas — incluso si no dice explícitamente "DTO" o "Data Transfer Object".
  Genera DTOs inmutables para respuestas y mutables para requests, con validaciones, documentación XML y patrones estándar.
user-invocable: true
argument-hint: "Nombre del DTO a crear (ej: 'ProductoDto')"
metadata: 
  category: "Desarrollo"
  tags: [".NET Core", "Entity Framework",  "Convenciones de Código"]
---

# SKILL: Creación de DTOs en .NET Core

## Propósito
Utiliza este skill para crear nuevos DTOs (Data Transfer Objects) en el proyecto de .NET Core siguiendo las convenciones y patrones establecidos. Los DTOs generados incluirán estructuras inmutables para respuestas y estructuras mutables para solicitudes, con validaciones y documentación XML. Este skill se basa en las mejores prácticas de diseño de DTOs en .NET Core, asegurando que los objetos de transferencia de datos sean claros, seguros y fáciles de mantener.

# Input
   Entidades en el proyecto  que se encuentran en el archivo Context.cs.

# Requisitos
- **Lenguaje**: C# 12, .NET 8.
- **Nulabilidad**: Habilitada, sin supresiones innecesarias.
- **Documentación**: Incluir comentarios XML breves (<summary>) en tipos públicos.
- **Seguridad**: Sanitizar entradas, evitar fugas de información sensible, manejar UTC para fechas.

## Convenciones del Proyecto

Generar DTOs, validadores y mapeos para una entidad  en .NET 8, asegurando que cumplan con las mejores prácticas de diseño y seguridad. El resultado debe ser un conjunto de archivos bien estructurados y documentados, listos para integrarse en un proyecto ASP.NET Core.

### Estructura Base de DTO

#### DTOs Inmutables (Recomendado)
```csharp
namespace bmlabs.core.dtos;

/// <summary>
/// DTO para representar [descripción de la entidad]
/// </summary>
public record NombreDto
{
    /// <summary>
    /// Identificador único de [entidad]
    /// </summary>
    public Guid Id { get; init; }
    
    // Propiedades adicionales...
}
```

#### DTOs Mutables (Para requests/forms)
```csharp
namespace bmlabs.core.dtos;

/// <summary>
/// DTO para el request de [descripción de la operación]
/// </summary>
public class NombreRequestDto
{
    /// <summary>
    /// Identificador único (opcional para edición)
    /// </summary>
    public Guid? Id { get; set; }
    
    // Propiedades adicionales...
}
```

### Reglas de Nomenclatura

#### Archivos y Clases
- **Archivos**: `{NombreEntidad}Dto.cs`
- **Clases**: `{NombreEntidad}Dto`
- **Requests**: `{NombreEntidad}RequestDto` o `{Operacion}RequestDto`
- **Responses**: `{NombreEntidad}ResponseDto` o `{Operacion}ResponseDto`
- **Namespace**: `bmlabs.core.dtos`

#### Tipos de DTOs Especiales
- **Upsert**: `{NombreEntidad}UpsertRequestDto`
- **Paginación**: `PagedRequestDto`, `PagedResponseDto<T>`
- **Métricas**: `{NombreMetrica}Dto`
- **Integración externa**: `{Servicio}Dto` (ej: `BukDto`)

### Tipos de Datos Comunes

#### Identificadores
```csharp
/// <summary>
/// Identificador único de [entidad]
/// </summary>
public Guid Id { get; init; }

/// <summary>  
/// Identificador opcional de [entidad relacionada]
/// </summary>
public Guid? IdEntidadRelacionada { get; init; }
```

#### Strings
```csharp
/// <summary>
/// Nombre de [entidad]
/// </summary>
public string Nombre { get; init; } = string.Empty;

/// <summary>
/// Descripción opcional de [entidad]
/// </summary>
public string? Descripcion { get; init; }
```

#### Fechas (UTC)
```csharp
/// <summary>
/// Fecha de creación (UTC)
/// </summary>
public DateTimeOffset FechaCreacion { get; init; }

/// <summary>
/// Fecha opcional de modificación (UTC)
/// </summary>
public DateTimeOffset? FechaModificacion { get; init; }
```

#### Decimales y Numéricos
```csharp
/// <summary>
/// Precio del elemento
/// </summary>
public decimal? Precio { get; init; }

/// <summary>
/// Cantidad de elementos
/// </summary>
public int Cantidad { get; init; }
```

#### Booleanos
```csharp
/// <summary>
/// Indica si está activo
/// </summary>
public bool Activo { get; init; }

/// <summary>
/// Indica si es crítico (opcional)
/// </summary>
public bool? Critico { get; init; }
```

### Propiedades de Navegación

#### Entidades Relacionadas
```csharp
/// <summary>
/// Información de la división (navegación)
/// </summary>
public DivisionDto? Division { get; init; }

/// <summary>
/// Lista de elementos relacionados
/// </summary>
public IReadOnlyList<ElementoDto> Elementos { get; init; } = new List<ElementoDto>();
```

### DTOs de Paginación

#### Request Paginado
```csharp
using System.ComponentModel.DataAnnotations;

namespace bmlabs.core.dtos;

/// <summary>
/// DTO para solicitudes paginadas
/// </summary>
public record PagedRequestDto
{
    /// <summary>
    /// Número de página (1-based)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0")]
    public int Page { get; init; } = 1;

    /// <summary>
    /// Cantidad de elementos por página
    /// </summary>
    [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Campo para ordenamiento
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Dirección del ordenamiento (asc, desc)
    /// </summary>
    public string? SortDirection { get; init; } = "asc";

    /// <summary>
    /// Filtro de búsqueda general
    /// </summary>
    public string? Search { get; init; }
}
```

#### Response Paginado
```csharp
/// <summary>
/// DTO para respuestas paginadas
/// </summary>
/// <typeparam name="T">Tipo de datos en la colección</typeparam>
public record PagedResponseDto<T>
{
    /// <summary>
    /// Datos de la página actual
    /// </summary>
    public IReadOnlyList<T> Data { get; init; } = new List<T>();

    /// <summary>
    /// Número de página actual
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Cantidad de elementos por página
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total de elementos en todas las páginas
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Total de páginas disponibles
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Indica si hay página anterior
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// Indica si hay página siguiente
    /// </summary>
    public bool HasNext => Page < TotalPages;
}
```

### DTOs Especializados

#### DTO de Request Complejo
```csharp
/// <summary>
/// DTO para el request de creación/actualización con archivos
/// </summary>
public class EntidadUpsertRequestDto
{
    /// <summary>
    /// Identificador único (para edición)
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Nombre de la entidad
    /// </summary>
    public string? Nombre { get; set; }

    /// <summary>
    /// Estado de la entidad
    /// </summary>
    public string? Estado { get; set; }

    /// <summary>
    /// Usuario que realizó la modificación
    /// </summary>
    public Guid UsuarioModificacion { get; set; }

    /// <summary>
    /// Fecha de modificación (UTC)
    /// </summary>
    public DateTimeOffset? FechaModificacion { get; set; }
}
```

#### DTO de Métricas
```csharp
/// <summary>
/// DTO para métricas de [descripción]
/// </summary>
public class MetricaDto
{
    /// <summary>
    /// Nombre del elemento medido
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Valor de la métrica
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Unidad de medida (opcional)
    /// </summary>
    public string? UnidadMedida { get; set; }
}
```

### Validación con Data Annotations

#### Validaciones Comunes
```csharp
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Email del usuario
/// </summary>
[EmailAddress(ErrorMessage = "El formato del email no es válido")]
public string Email { get; init; } = string.Empty;

/// <summary>
/// Rango numérico
/// </summary>
[Range(1, 100, ErrorMessage = "El valor debe estar entre 1 y 100")]
public int Valor { get; init; }

/// <summary>
/// Campo requerido
/// </summary>
[Required(ErrorMessage = "El nombre es requerido")]
public string Nombre { get; init; } = string.Empty;

/// <summary>
/// Longitud máxima
/// </summary>
[MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
public string? Descripcion { get; init; }
```

### Integración con APIs Externas

#### DTOs de Servicios Externos
```csharp
using System.Text.Json.Serialization;

namespace bmlabs.core.dtos;

/// <summary>
/// Response del servicio externo [nombre]
/// </summary>
public class ServicioExternoResponseDto
{
    /// <summary>
    /// Paginación del servicio externo
    /// </summary>
    public ServicioPaginationDto Pagination { get; set; } = new();

    /// <summary>
    /// Datos del servicio externo
    /// </summary>
    public List<ServicioDataDto> Data { get; set; } = new();
}

/// <summary>
/// DTO de datos del servicio externo
/// </summary>
public class ServicioDataDto
{
    /// <summary>
    /// Código del elemento
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del elemento
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
```

## Plantillas por Tipo de DTO

### DTO Simple (Read-Only)
```csharp
namespace bmlabs.core.dtos;

/// <summary>
/// DTO para representar [descripción]
/// </summary>
public record EntidadDto
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Nombre de la entidad
    /// </summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Descripción opcional
    /// </summary>
    public string? Descripcion { get; init; }

    /// <summary>
    /// Fecha de creación (UTC)
    /// </summary>
    public DateTimeOffset FechaCreacion { get; init; }

    /// <summary>
    /// Indica si está activo
    /// </summary>
    public bool Activo { get; init; }
}
```

### DTO con Navegación
```csharp
namespace bmlabs.core.dtos;

/// <summary>
/// DTO para representar [descripción] con navegación
/// </summary>
public record EntidadConNavegacionDto
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identificador de la entidad padre
    /// </summary>
    public Guid IdEntidadPadre { get; init; }

    /// <summary>
    /// Nombre de la entidad
    /// </summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Información de la entidad padre (navegación)
    /// </summary>
    public EntidadPadreDto? EntidadPadre { get; init; }

    /// <summary>
    /// Lista de entidades hijas
    /// </summary>
    public IReadOnlyList<EntidadHijaDto> EntidadesHijas { get; init; } = new List<EntidadHijaDto>();
}
```

### DTO de Request
```csharp
using System.ComponentModel.DataAnnotations;

namespace bmlabs.core.dtos;

/// <summary>
/// DTO para request de creación/actualización de [entidad]
/// </summary>
public class EntidadRequestDto
{
    /// <summary>
    /// Identificador único (opcional para creación)
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Nombre de la entidad
    /// </summary>
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción opcional
    /// </summary>
    [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Identificador de entidad relacionada
    /// </summary>
    [Required(ErrorMessage = "La entidad relacionada es requerida")]
    public Guid IdEntidadRelacionada { get; set; }

    /// <summary>
    /// Indica si está activo
    /// </summary>
    public bool Activo { get; set; } = true;
}
```

## Mejores Prácticas

### Cuándo Usar Record vs Class
- **Record**: Para DTOs inmutables de lectura (responses, consultas)
- **Class**: Para DTOs mutables de escritura (requests, forms, updates)

### Uso de Propiedades Nullable
- **Guid?**: Para IDs opcionales o de navegación
- **string?**: Para campos opcionales
- **DateTime?/DateTimeOffset?**: Para fechas opcionales
- **decimal?**: Para valores numéricos opcionales

### Seguridad y Validación
- Usar Data Annotations para validación básica
- No exponer propiedades sensibles en DTOs de response
- Validar todos los inputs en DTOs de request
- Usar DTOs diferentes para create/update cuando sea necesario

### Documentación
- Comentarios XML obligatorios para todas las propiedades públicas
- Especificar si las fechas son UTC
- Indicar el propósito de cada propiedad de navegación
- Documentar rangos válidos y restricciones

## Checklist para Nuevo DTO

- [ ] Archivo creado en `core/dtos/`
- [ ] Namespace correcto: `bmlabs.core.dtos`
- [ ] Nombre apropiado con sufijo `Dto`
- [ ] Tipo apropiado: `record` para inmutable, `class` para mutable
- [ ] Comentarios XML para la clase y todas las propiedades
- [ ] Propiedades con tipos apropiados y modificadores de acceso
- [ ] Valores por defecto para strings (`string.Empty`)
- [ ] Listas como `IReadOnlyList<T>` o `List<T>` con inicialización
- [ ] Validaciones con Data Annotations si es request
- [ ] Fechas como `DateTimeOffset` para UTC
- [ ] IDs como `Guid` o `Guid?` según corresponda

