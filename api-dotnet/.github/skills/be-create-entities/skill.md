---
name: be-create-entities
description: >
  Utiliza este skill cuando necesites crear modelos de base de datos, tablas de dominio, o estructuras persistentes en .NET con Entity Framework.
  Aplica cuando el usuario mencione "crear entidad", "modelo de datos", "tabla de BD", "Entity Framework", "DbContext", "relaciones de datos",
  o cuando necesite definir el esquema de datos del negocio — incluso si no dice explícitamente "entity" o "base de datos".
  Genera entidades completas con propiedades, relaciones, atributos EF Core, validaciones y configuración DbContext.
user-invocable: true
argument-hint: "Nombre de la entidad a crear (ej: 'Producto')"
metadata: 
  category: "Desarrollo"
  tags: [".NET Core", "Entity Framework", "Base de Datos", "Convenciones de Código"]
---

# SKILL: Creación de Entidades de Base de Datos en .NET Core
## Propósito
Utiliza este skill para crear nuevas entidades de base de datos en el proyecto de .NET Core siguiendo las convenciones y patrones establecidos. Las entidades generadas incluirán la estructura base, propiedades, relaciones, atributos de Entity Framework y documentación XML, asegurando que las entidades sigan las mejores prácticas de diseño y sean compatibles con el DbContext del proyecto. Este skill se basa en las mejores prácticas de diseño de entidades en .NET Core, garantizando que las entidades sean claras, mantenibles y estén correctamente configuradas para su uso con Entity Framework.

## Convenciones del Proyecto

Si no existe la carpeta `core/entities/`, ejecuta el siguiente comando para crearla:

```bash
dotnet ef dbcontext scaffold  "Host=<HOST>;Port=25060;Database=<DATABASE>;Username=<USERNAME>;Password=<PASSWORD>" Npgsql.EntityFrameworkCore.PostgreSQL -o entities -c Context --no-pluralize --data-annotations --force -d -f -n bmlabs.core.entities
```
Este comando generará las entidades a partir de la base de datos, asegurando que sigan las convenciones de nomenclatura y estructura establecidas en el proyecto. Luego, puedes modificar las entidades generadas para agregar propiedades adicionales, relaciones o comentarios según sea necesario.

Pide los detalles de conexión a la base de datos al equipo de desarrollo o al administrador de la base de datos para ejecutar el comando correctamente.

### Estructura Base de Entidad
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bmlabs.core.entities;

[Table("nombre_tabla")]
public partial class NombreEntidad
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    // Propiedades adicionales...
}
```

### Reglas de Nomenclatura

#### Clases y Archivos
- **Clases**: PascalCase (ej: `Usuario`, `CategoriaEpp`, `BitacoraUsuario`)
- **Archivos**: Mismo nombre de la clase con extensión `.cs`
- **Tablas DB**: snake_case minúsculas (ej: `usuario`, `categoria_epp`, `bitacora_usuario`)
- **Columnas DB**: snake_case minúsculas (ej: `id`, `nombre`, `fecha_creacion`)

#### Propiedades
- **Propiedades C#**: PascalCase (ej: `Id`, `Nombre`, `FechaCreacion`)
- **Claves foráneas**: `Id{NombreEntidad}` (ej: `IdUsuario`, `IdCategoriaEpp`)

### Atributos Obligatorios

#### Para Entidades
```csharp
[Table("nombre_tabla")]  // Nombre de tabla en snake_case
public partial class NombreEntidad
```

#### Para Propiedades
```csharp
[Key]                           // Solo para llave primaria
[Column("nombre_columna")]     // Nombre de columna en snake_case
public TipoPropiedad Nombre { get; set; }

// Con tipo de datos específico
[Column("descripcion", TypeName = "character varying")]
public string? Descripcion { get; set; }

// Para fechas
[Column("fecha_creacion", TypeName = "datetime2")]
public DateTime FechaCreacion { get; set; }
```

### Tipos de Datos Comunes

#### Identificadores
```csharp
[Key]
[Column("id")]
public Guid Id { get; set; }
```

#### Strings
```csharp
// String con tipo específico
[Column("nombre", TypeName = "character varying")]
public string Nombre { get; set; } = null!;

// String nullable
[Column("descripcion")]
public string? Descripcion { get; set; }
```

#### Fechas
```csharp
[Column("fecha_creacion", TypeName = "datetime2")]
public DateTime FechaCreacion { get; set; }
```

#### Decimales
```csharp
[Column("precio")]
public decimal? Precio { get; set; }
```

#### Booleanos
```csharp
[Column("activo")]
public bool? Activo { get; set; }
```

### Relaciones

#### Claves Foráneas
```csharp
/// <summary>
/// ID referencia al usuario responsable
/// </summary>
[Column("id_usuario")]
public Guid? IdUsuario { get; set; }

// Propiedad de navegación
[ForeignKey("IdUsuario")]
[InverseProperty("Entidades")]
public virtual Usuario? IdUsuarioNavigation { get; set; }
```

#### Uno a Muchos (Desde el lado "Muchos")
```csharp
[Column("id_categoria")]
public Guid IdCategoria { get; set; }

[ForeignKey("IdCategoria")]
[InverseProperty("Items")]
public virtual Categoria IdCategoriaNavigation { get; set; } = null!;
```

#### Uno a Muchos (Desde el lado "Uno")
```csharp
[InverseProperty("IdCategoriaNavigation")]
public virtual ICollection<Item> Items { get; set; } = new List<Item>();
```

### Documentación

#### Comentarios XML
```csharp
/// <summary>
/// Nombre del EPP
/// </summary>
[Column("nombre")]
public string? Nombre { get; set; }

/// <summary>
/// ID referencia a la Categoria del EPP
/// </summary>
[Column("id_categoria_epp")]
public Guid? IdCategoriaEpp { get; set; }
```

## Plantilla Completa para Nueva Entidad

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace bmlabs.core.entities;

[Table("mi_entidad")]
public partial class MiEntidad
{
    /// <summary>
    /// Identificador único de la entidad
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre de la entidad
    /// </summary>
    [Column("nombre", TypeName = "character varying")]
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// Descripción de la entidad
    /// </summary>
    [Column("descripcion", TypeName = "character varying")]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    [Column("fecha_creacion", TypeName = "datetime2")]
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// ID referencia al usuario que creó el registro
    /// </summary>
    [Column("id_usuario_creacion")]
    public Guid IdUsuarioCreacion { get; set; }

    /// <summary>
    /// Indica si el registro está activo
    /// </summary>
    [Column("activo")]
    public bool Activo { get; set; }

    // Propiedades de navegación
    [ForeignKey("IdUsuarioCreacion")]
    [InverseProperty("MiEntidadCreadas")]
    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    // Relaciones (si aplica)
    [InverseProperty("IdMiEntidadNavigation")]
    public virtual ICollection<EntidadRelacionada> EntidadesRelacionadas { get; set; } = new List<EntidadRelacionada>();
}
```

## Configuración en DbContext

### Registro en Context.cs
```csharp
public virtual DbSet<MiEntidad> MiEntidad { get; set; }
```

### Configuración en OnModelCreating
```csharp
modelBuilder.Entity<MiEntidad>(entity =>
{
    entity.HasKey(e => e.Id).HasName("mi_entidad_pkey");
    
    entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
    entity.Property(e => e.Nombre).HasComment("Nombre de la entidad");
    entity.Property(e => e.FechaCreacion)
        .HasDefaultValueSql("GETDATE()")
        .HasComment("Fecha de creación del registro");
    
    entity.HasOne(d => d.IdUsuarioCreacionNavigation)
        .WithMany(p => p.MiEntidadCreadas)
        .HasConstraintName("mi_entidad_id_usuario_creacion_fkey");
});
```

## Casos Especiales

### Entidades de Auditoría
```csharp
/// <summary>
/// Fecha de última modificación del registro
/// </summary>
[Column("fecha_ult_modificacion", TypeName = "datetime2")]
public DateTime FechaUltModificacion { get; set; }

/// <summary>
/// Usuario que realizó la última modificación del registro
/// </summary>
[Column("usuario_ult_modificacion")]
public Guid UsuarioUltModificacion { get; set; }
```

### Entidades de Relación (Many-to-Many)
```csharp
[Table("usuario_rol")]
public partial class UsuarioRol
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("id_usuario")]
    public Guid IdUsuario { get; set; }

    [Column("id_rol")]
    public Guid IdRol { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("UsuarioRol")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [ForeignKey("IdRol")]
    [InverseProperty("UsuarioRol")]
    public virtual Rol IdRolNavigation { get; set; } = null!;
}
```

## Checklist para Nueva Entidad

- [ ] Archivo creado en `core/entities/`
- [ ] Namespace correcto: `bmlabs.core.entities`
- [ ] Clase marcada como `partial class`
- [ ] Atributo `[Table("nombre_tabla")]` con nombre en snake_case
- [ ] Propiedad `Id` de tipo `Guid` con `[Key]` y `[Column("id")]`
- [ ] Todas las propiedades con `[Column("nombre_columna")]`
- [ ] Comentarios XML para propiedades importantes
- [ ] Relaciones configuradas con `[ForeignKey]` e `[InverseProperty]`
- [ ] Colecciones inicializadas con `new List<T>()`
- [ ] Entidad registrada en `Context.cs` como `DbSet<T>`
- [ ] Configuración agregada en `OnModelCreating` si es necesaria

