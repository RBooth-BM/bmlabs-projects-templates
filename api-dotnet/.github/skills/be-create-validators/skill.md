---
name: be-create-validators
description: Guía para crear validators en .NET Core siguiendo las convenciones del proyecto.
user-invocable: true
argument-hint: "Nombre del DTO a validar (ej: 'ProductoDto')"
metadata: 
  category: "Desarrollo"
  tags: [".NET Core", "FluentValidation", "DTOs", "Convenciones de Código"]
---


# SKILL: Creación de Validators en .NET Core con FluentValidation

## Propósito
Este skill proporciona las convenciones y patrones para crear validators usando FluentValidation en .NET Core 

## Convenciones del Proyecto

### Estructura Base de Validator

```csharp
using FluentValidation;

namespace bmlabs.core.dtos.Validators;

/// <summary>
/// Validador para [NombreDto]
/// </summary>
public class NombreDtoValidator : AbstractValidator<NombreDto>
{
    public NombreDtoValidator()
    {
        // Reglas de validación
    }
}
```

### Reglas de Nomenclatura

#### Archivos y Clases
- **Archivos**: `{NombreDto}Validator.cs`
- **Clases**: `{NombreDto}Validator`
- **Namespace**: `bmlabs.core.dtos.Validators`
- **Herencia**: `AbstractValidator<T>` donde T es el DTO a validar

### Ubicación
- **Carpeta**: `core/dtos/validators/`
- **Un validator por DTO**
- **Un validator por archivo**

## Tipos de Validaciones Comunes

### Validaciones de Campos Requeridos

```csharp
RuleFor(x => x.Id)
    .NotEmpty()
    .WithMessage("El ID es requerido");

RuleFor(x => x.Nombre)
    .NotEmpty()
    .WithMessage("El nombre es requerido");
```

### Validaciones de Longitud de String

```csharp
RuleFor(x => x.Nombre)
    .NotEmpty()
    .WithMessage("El nombre es requerido")
    .MaximumLength(255)
    .WithMessage("El nombre no puede exceder 255 caracteres");

RuleFor(x => x.Descripcion)
    .MaximumLength(500)
    .WithMessage("La descripción no puede exceder 500 caracteres");
```

### Validaciones Numéricas

```csharp
// Para valores no negativos
RuleFor(x => x.Precio)
    .GreaterThanOrEqualTo(0)
    .WithMessage("El precio no puede ser negativo")
    .When(x => x.Precio.HasValue);

// Para rangos específicos
RuleFor(x => x.Cantidad)
    .GreaterThan(0)
    .WithMessage("La cantidad debe ser mayor a 0")
    .LessThanOrEqualTo(1000)
    .WithMessage("La cantidad no puede exceder 1000");
```

### Validaciones de Fechas

```csharp
// Fecha no futura
RuleFor(x => x.FechaIngreso)
    .NotEmpty()
    .WithMessage("La fecha de ingreso es requerida")
    .LessThanOrEqualTo(DateTimeOffset.UtcNow)
    .WithMessage("La fecha de ingreso no puede ser futura");

// Fecha opcional no futura
RuleFor(x => x.FechaLectura)
    .LessThanOrEqualTo(DateTime.Now)
    .When(x => x.FechaLectura.HasValue)
    .WithMessage("La fecha de lectura no puede ser futura");
```

### Validaciones de Enums/Estados Específicos

```csharp
RuleFor(x => x.Tipo)
    .NotEmpty()
    .WithMessage("El tipo de colaborador es requerido")
    .Must(tipo => tipo is "INTERNO" or "EXTERNO")
    .WithMessage("El tipo debe ser INTERNO o EXTERNO");

RuleFor(x => x.Estado)
    .Must(estado => estado is null or "APROBADA" or "RECHAZADA" or "EN ESPERA")
    .WithMessage("El estado debe ser APROBADA, RECHAZADA o EN ESPERA");
```

### Validaciones Condicionales

```csharp
// Campo requerido solo cuando se cumple una condición
RuleFor(x => x.IdResponsableResolucion)
    .NotEmpty()
    .WithMessage("El responsable de resolución es requerido")
    .When(x => x.Estado is "APROBADA" or "RECHAZADA");

// Comparación entre campos
RuleFor(x => x.FechaResolucion)
    .GreaterThanOrEqualTo(x => x.FechaSolicitud)
    .WithMessage("La fecha de resolución debe ser posterior a la fecha de solicitud")
    .When(x => x.FechaResolucion.HasValue && x.FechaSolicitud.HasValue);

// Validación cuando el campo no está vacío
RuleFor(x => x.UnidadMedida)
    .NotEmpty()
    .WithMessage("La unidad de medida es requerida")
    .MaximumLength(100)
    .WithMessage("La unidad de medida no puede exceder 100 caracteres")
    .When(x => !string.IsNullOrEmpty(x.UnidadMedida));
```

### Validaciones Personalizadas con Regex

```csharp
public class UsuarioDtoValidator : AbstractValidator<UsuarioDto>
{
    private static readonly Regex RutRegex = new(@"^\d{1,2}\.\d{3}\.\d{3}-[\dkK]$", RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

    public UsuarioDtoValidator()
    {
        RuleFor(x => x.Correo)
            .NotEmpty()
            .WithMessage("El correo es requerido")
            .MaximumLength(255)
            .WithMessage("El correo no puede exceder 255 caracteres")
            .Must(BeValidEmail)
            .WithMessage("El formato del correo no es válido");

        RuleFor(x => x.Rut)
            .NotEmpty()
            .WithMessage("El RUT es requerido")
            .MaximumLength(12)
            .WithMessage("El RUT no puede exceder 12 caracteres")
            .Must(BeValidRut)
            .WithMessage("El formato del RUT no es válido");
    }

    private static bool BeValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email.Trim().ToLowerInvariant());
    }

    private static bool BeValidRut(string? rut)
    {
        if (string.IsNullOrWhiteSpace(rut))
            return false;

        return RutRegex.IsMatch(rut.Trim());
    }
}
```

## Plantillas por Tipo de Validator

### Validator Simple

```csharp
using FluentValidation;

namespace bmlabs.core.dtos.Validators;

/// <summary>
/// Validador para EntidadSimpleDto
/// </summary>
public class EntidadSimpleDtoValidator : AbstractValidator<EntidadSimpleDto>
{
    public EntidadSimpleDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("El ID es requerido");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es requerido")
            .MaximumLength(255)
            .WithMessage("El nombre no puede exceder 255 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .WithMessage("La descripción no puede exceder 500 caracteres");
    }
}
```

### Validator con Validaciones Complejas

```csharp
using FluentValidation;
using System.Text.RegularExpressions;

namespace bmlabs.core.dtos.Validators;

/// <summary>
/// Validador para EntidadComplejaDto
/// </summary>
public class EntidadComplejaDtoValidator : AbstractValidator<EntidadComplejaDto>
{
    private static readonly Regex PatronRegex = new(@"^[A-Z]{2}-\d{4}$", RegexOptions.Compiled);

    public EntidadComplejaDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("El ID es requerido");

        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("El código es requerido")
            .Must(BeValidCodigo)
            .WithMessage("El formato del código no es válido (ejemplo: AB-1234)");

        RuleFor(x => x.Estado)
            .Must(estado => estado is "ACTIVO" or "INACTIVO" or "PENDIENTE")
            .WithMessage("El estado debe ser ACTIVO, INACTIVO o PENDIENTE");

        RuleFor(x => x.FechaCreacion)
            .NotEmpty()
            .WithMessage("La fecha de creación es requerida")
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("La fecha de creación no puede ser futura");

        RuleFor(x => x.Valor)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El valor no puede ser negativo")
            .When(x => x.Valor.HasValue);

        // Validación condicional
        RuleFor(x => x.IdResponsable)
            .NotEmpty()
            .WithMessage("El responsable es requerido")
            .When(x => x.Estado == "ACTIVO");
    }

    private static bool BeValidCodigo(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return false;

        return PatronRegex.IsMatch(codigo.Trim().ToUpperInvariant());
    }
}
```

### Validator con Fechas Relacionadas

```csharp
using FluentValidation;

namespace bmlabs.core.dtos.Validators;

/// <summary>
/// Validador para ProcesoDtoValidator con fechas relacionadas
/// </summary>
public class ProcesoDtoValidator : AbstractValidator<ProcesoDto>
{
    public ProcesoDtoValidator()
    {
        RuleFor(x => x.FechaInicio)
            .NotEmpty()
            .WithMessage("La fecha de inicio es requerida");

        RuleFor(x => x.FechaFin)
            .GreaterThan(x => x.FechaInicio)
            .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio")
            .When(x => x.FechaFin.HasValue);

        RuleFor(x => x.FechaAprobacion)
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("La fecha de aprobación debe ser posterior o igual a la fecha de inicio")
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("La fecha de aprobación no puede ser futura")
            .When(x => x.FechaAprobacion.HasValue);

        RuleFor(x => x.Estado)
            .Must(estado => estado is "DRAFT" or "EN_PROCESO" or "COMPLETADO" or "CANCELADO")
            .WithMessage("El estado debe ser DRAFT, EN_PROCESO, COMPLETADO o CANCELADO");

        // Campo requerido según el estado
        RuleFor(x => x.IdAprobador)
            .NotEmpty()
            .WithMessage("El aprobador es requerido")
            .When(x => x.Estado is "COMPLETADO");

        RuleFor(x => x.FechaAprobacion)
            .NotEmpty()
            .WithMessage("La fecha de aprobación es requerida")
            .When(x => x.Estado is "COMPLETADO");
    }
}
```

## Reglas para Validaciones Específicas del Dominio

### RUT Chileno
```csharp
private static readonly Regex RutRegex = new(@"^\d{1,2}\.\d{3}\.\d{3}-[\dkK]$", RegexOptions.Compiled);

private static bool BeValidRut(string? rut)
{
    if (string.IsNullOrWhiteSpace(rut))
        return false;

    return RutRegex.IsMatch(rut.Trim());
}
```

### Email
```csharp
private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

private static bool BeValidEmail(string? email)
{
    if (string.IsNullOrWhiteSpace(email))
        return false;

    return EmailRegex.IsMatch(email.Trim().ToLowerInvariant());
}
```

### Códigos de EPP
```csharp
private static readonly Regex CodigoEppRegex = new(@"^EPP-\d{4}$", RegexOptions.Compiled);

private static bool BeValidCodigoEpp(string? codigo)
{
    if (string.IsNullOrWhiteSpace(codigo))
        return false;

    return CodigoEppRegex.IsMatch(codigo.Trim().ToUpperInvariant());
}
```

## Mensajes de Error

### Convenciones para Mensajes
- **Idioma**: Español
- **Tono**: Formal pero claro
- **Estructura**: Descriptivo sobre qué está mal y qué se espera

#### Ejemplos de Mensajes Estándar
```csharp
// Campos requeridos
.WithMessage("El [campo] es requerido");
.WithMessage("El ID del [entidad] es requerido");

// Longitud
.WithMessage("El [campo] no puede exceder [número] caracteres");

// Formato
.WithMessage("El formato del [campo] no es válido");
.WithMessage("El formato del RUT no es válido");
.WithMessage("El formato del correo no es válido");

// Valores
.WithMessage("El [campo] no puede ser negativo");
.WithMessage("El [campo] debe ser mayor a 0");

// Estados/Enums
.WithMessage("El [campo] debe ser [OPCION1], [OPCION2] o [OPCION3]");

// Fechas
.WithMessage("La fecha de [campo] no puede ser futura");
.WithMessage("La fecha de [campo2] debe ser posterior a la fecha de [campo1]");

// Condicionales
.WithMessage("El [campo] es requerido cuando el estado es [ESTADO]");
```

## Mejores Prácticas

### Organización del Código
- Un validator por DTO
- Métodos privados estáticos para validaciones complejas
- Regex compilados como campos estáticos readonly
- Validaciones en orden lógico: ID, campos requeridos, formato, lógica de negocio

### Performance
- Usar `RegexOptions.Compiled` para regex que se usan frecuentemente
- Métodos de validación personalizada como `static`
- Evitar crear objetos innecesarios en validaciones

### Validaciones Condicionales
- Usar `When()` para validaciones que dependen del contexto
- Combinar múltiples condiciones con operadores lógicos
- Validar campos relacionados juntos

### Mensajes Claros
- Especificar exactamente qué está mal
- Incluir ejemplos cuando sea útil
- Usar terminología consistente con el dominio

## Integración con el Sistema

### Registro en DI (si aplica)
```csharp
// En Program.cs o DependencyInjection
services.AddScoped<IValidator<MiDto>, MiDtoValidator>();
```

### Uso en Controllers
```csharp
public async Task<IActionResult> Post([FromBody] MiDto dto)
{
    var validator = new MiDtoValidator();
    var result = await validator.ValidateAsync(dto);
    
    if (!result.IsValid)
    {
        return BadRequest(result.Errors);
    }
    
    // Lógica del negocio...
}
```

## Checklist para Nuevo Validator

- [ ] Archivo creado en `core/dtos/validators/`
- [ ] Namespace correcto: `bmlabs.core.dtos.Validators`
- [ ] Herencia de `AbstractValidator<T>`
- [ ] Comentario XML explicando el propósito
- [ ] Validaciones de campos ID requeridos
- [ ] Validaciones de longitud para strings
- [ ] Validaciones de formato para campos especiales
- [ ] Validaciones condicionales según el contexto
- [ ] Mensajes de error en español y descriptivos
- [ ] Métodos privados para validaciones complejas
- [ ] Regex compilados para validaciones de formato

