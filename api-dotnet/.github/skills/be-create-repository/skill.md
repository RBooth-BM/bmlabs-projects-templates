---
name: be-create-repository
description: >
  Utiliza este skill cuando necesites acceso a datos, operaciones CRUD, o consultas de base de datos en .NET con Entity Framework.
  Aplica cuando el usuario mencione "crear repository", "acceso a datos", "consultas BD", "CRUD operations", "Entity Framework", "data access layer",
  o cuando necesite encapsular operaciones de persistencia — incluso si no dice explícitamente "repository" o "patrón repositorio".
  Genera repositories con implementación genérica base, operaciones CRUD específicas, consultas optimizadas e inyección de dependencias.
user-invocable: true
argument-hint: "Nombre del repository a crear (ej: 'ProductoRepository')"
metadata: 
  category: "Desarrollo"
  tags: [".NET Core", "Entity Framework",  "Convenciones de Código"]
---

# SKILL: Creación de Repositories en .NET Core con Entity Framework

## Propósito
Utiliza este skill para crear nuevos repositories en el proyecto de .NET Core siguiendo las convenciones y patrones establecidos. Los repositories generados incluirán la estructura base, implementación de un repositorio genérico base, herencia para repositories específicos, inyección de dependencias y documentación XML, asegurando que los repositories sigan las mejores prácticas de diseño y sean compatibles con el DbContext del proyecto. Este skill se basa en las mejores prácticas de diseño de repositories con Entity Framework Core en .NET Core, garantizando que los repositories sean claros, mantenibles y estén correctamente configurados para su uso en la aplicación. Este skill se complementa con el skill de creación de entidades, ya que los repositories se basan en las entidades definidas en el proyecto para realizar operaciones de acceso a datos.

## Arquitectura Base

### Repository Pattern implementado
El proyecto utiliza el patrón Repository con una arquitectura de 3 capas:
1. **Interfaz genérica base**: `IRepository<T>` - Define el contrato común
2. **Implementación genérica base**: `Repository<T, C>` - Implementa operaciones CRUD básicas  
3. **Repositories específicos**: Heredan de la base y agregan funcionalidad específica

### Estructura de archivos
```
core/repositories/
├── base/
│   └── Repository.cs           # Repositorio base genérico
├── AreaRepository.cs           # Repository específico de Area
├── UsuarioRepository.cs        # Repository específico con overrides
├── SolicitudRepository.cs      # Repository con navegación compleja
└── DependencyInjectionExtensions.cs  # Registro de DI
```

## Convenciones del Proyecto

Si no existe el archivo `Repository.cs` en `core/repositories/base/`, crear el repositorio base genérico siguiendo el siguiente codigo. 

```csharp
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using bmlabs.core.entities;

namespace bmlabs.core.repositories
{
    namespace Base
    {

        /// <summary>
        /// Interfaz genérica para repositorios con operaciones CRUD básicas
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        public interface IRepository<T>
            where T : class
        {
            /// <summary>
            /// Obtiene el contexto de la base de datos
            /// </summary>
            Context Context();

            /// <summary>
            /// Busca una entidad por su instancia
            /// </summary>
            Task<T?> FindById(T entity);

            /// <summary>
            /// Busca una entidad por un array de claves
            /// </summary>
            Task<T?> FindById(object[] aids);

            /// <summary>
            /// Busca una entidad por su ID
            /// </summary>
            Task<T?> FindById(Guid id);

            /// <summary>
            /// Busca una entidad por su ID con opción de incluir propiedades de navegación
            /// </summary>
            Task<T?> FindById(Guid id, bool includeNavigationProperties);

            /// <summary>
            /// Busca entidades que cumplan con un predicado
            /// </summary>
            Task<List<T>> FindBy(Expression<Func<T, bool>> predicate);

            /// <summary>
            /// Busca entidades que cumplan con un predicado con opción de incluir propiedades de navegación
            /// </summary>
            Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, bool includeNavigationProperties);

            /// <summary>
            /// Busca entidades que cumplan con un predicado con paginación
            /// </summary>
            Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, int page, int pageSize);

            /// <summary>
            /// Busca entidades que cumplan con un predicado con paginación y opción de incluir propiedades de navegación
            /// </summary>
            Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, int page, int pageSize, bool includeNavigationProperties);

            /// <summary>
            /// Busca entidades que cumplan con un predicado con paginación y retorna el total de registros
            /// </summary>
            Task<(List<T> items, int totalCount)> FindByPaged(Expression<Func<T, bool>> predicate, int page, int pageSize);

            /// <summary>
            /// Busca entidades que cumplan con un predicado con paginación, opción de incluir propiedades de navegación y retorna el total de registros
            /// </summary>
            Task<(List<T> items, int totalCount)> FindByPaged(Expression<Func<T, bool>> predicate, int page, int pageSize, bool includeNavigationProperties);

            /// <summary>
            /// Retorna un IQueryable para consultas personalizadas
            /// </summary>
            IQueryable<T> FindByQuery(Expression<Func<T, bool>> predicate);

            /// <summary>
            /// Busca la primera entidad que cumpla con un predicado
            /// </summary>
            Task<T> FindFirst(Expression<Func<T, bool>> predicate);

            /// <summary>
            /// Busca todas las entidades
            /// </summary>
            Task<List<T>> FindAll();

            /// <summary>
            /// Busca todas las entidades con opción de incluir propiedades de navegación
            /// </summary>
            Task<List<T>> FindAll(bool includeNavigationProperties);

            /// <summary>
            /// Inserta una nueva entidad
            /// </summary>
            Task<T> Insert(T entity);

            /// <summary>
            /// Actualiza una entidad existente
            /// </summary>
            Task<T> Update(T entity);

            /// <summary>
            /// Elimina una entidad
            /// </summary>
            Task<T> Delete(T entity);

            /// <summary>
            /// Inserta o actualiza una entidad según si existe o no
            /// </summary>
            Task<(T, bool)> InsertOrUpdate(T entity);

            /// <summary>
            /// Inserta o actualiza una entidad con opción de actualizar solo campos no nulos
            /// </summary>
            Task<(T, bool)> InsertOrUpdate(T entity, bool updateOnlyNonNullFields);

            /// <summary>
            /// Elimina múltiples entidades de forma bulk
            /// </summary>
            Task<int> BulkDelete(IEnumerable<T> entities);

            /// <summary>
            /// Elimina múltiples entidades que cumplan con un predicado
            /// </summary>
            Task<int> BulkDelete(Expression<Func<T, bool>> predicate);

            /// <summary>
            /// Inserta múltiples entidades de forma bulk
            /// </summary>
            Task<List<T>> BulkInsert(IEnumerable<T> entities);
        }

        /// <summary>
        /// Repositorio genérico base que implementa operaciones CRUD básicas
        /// </summary>
        /// <typeparam name="T">Tipo de entidad</typeparam>
        /// <typeparam name="C">Tipo de contexto</typeparam>
        public abstract class Repository<T, C> : IRepository<T>
            where T : class
            where C : DbContext, new()
        {
            private C context;

            /// <summary>
            /// Obtiene el contexto tipado
            /// </summary>
            public C Context() => context;

            /// <summary>
            /// Constructor del repositorio
            /// </summary>
            /// <param name="context">Contexto de la base de datos</param>
            protected Repository(C context)
            {
                this.context = context;
            }

            /// <summary>
            /// Implementación explícita de la interfaz para obtener el contexto
            /// </summary>
            bmlabs.core.entities.Context IRepository<T>.Context()
            {
                return Context() as bmlabs.core.entities.Context;
            }

            /// <summary>
            /// Obtiene las claves primarias de una entidad
            /// </summary>
            private object[]? GetKeys(T entity)
            {
                object[]? aids = null;
                var entityType = context.Model.FindEntityType(typeof(T));
                var ids = entityType?.FindPrimaryKey();

                if (ids?.Properties.Count > 0)
                {
                    aids = new object[ids.Properties.Count];
                    for (int i = 0; i < ids.Properties.Count; i++)
                    {
                        var name = ids.Properties[i].Name;
                        var property = entity.GetType().GetProperty(name);
                        aids[i] = property?.GetValue(entity);
                    }
                }
                return aids;
            }

            /// <summary>
            /// Busca todas las entidades
            /// </summary>
            public async Task<List<T>> FindAll()
            {
                return await context.Set<T>().ToListAsync();
            }

            /// <summary>
            /// Busca todas las entidades con opción de incluir propiedades de navegación
            /// </summary>
            public virtual async Task<List<T>> FindAll(bool includeNavigationProperties)
            {
                if (!includeNavigationProperties)
                {
                    return await FindAll();
                }

                // Por defecto, usar Find con predicado siempre verdadero para incluir navegación
                return await FindBy(x => true, includeNavigationProperties);
            }

            /// <summary>
            /// Busca una entidad por su instancia
            /// </summary>
            public async Task<T?> FindById(T entity)
            {
                var aids = GetKeys(entity);
                if (aids == null) return null;
                return await context.Set<T>().FindAsync(aids);
            }

            /// <summary>
            /// Busca entidades que cumplan con un predicado
            /// </summary>
            public async Task<List<T>> FindBy(Expression<Func<T, bool>> predicate)
            {
                return await context.Set<T>().Where(predicate).ToListAsync();
            }

            /// <summary>
            /// Busca entidades que cumplan con un predicado con opción de incluir propiedades de navegación
            /// </summary>
            public virtual async Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, bool includeNavigationProperties)
            {
                if (!includeNavigationProperties)
                {
                    return await context.Set<T>().Where(predicate).ToListAsync();
                }

                // Por defecto, sin includes específicos - las clases derivadas deben sobrescribir este método
                return await context.Set<T>().Where(predicate).ToListAsync();
            }

            /// <summary>
            /// Busca entidades que cumplan con un predicado con paginación
            /// </summary>
            public async Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, int page, int pageSize)
            {
                return await context.Set<T>()
                    .Where(predicate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            /// <summary>
            /// Busca entidades que cumplan con un predicado con paginación y opción de incluir propiedades de navegación
            /// </summary>
            public virtual async Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, int page, int pageSize, bool includeNavigationProperties)
            {
                var query = context.Set<T>().Where(predicate);

                if (!includeNavigationProperties)
                {
                    return await query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                }

                // Por defecto, sin includes específicos - las clases derivadas deben sobrescribir este método
                return await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            /// <summary>
            /// Busca entidades que cumplan con un predicado con paginación y retorna el total de registros
            /// </summary>
            public async Task<(List<T> items, int totalCount)> FindByPaged(Expression<Func<T, bool>> predicate, int page, int pageSize)
            {
                var query = context.Set<T>().Where(predicate);
                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (items, totalCount);
            }

            /// <summary>
            /// Busca entidades que cumplan con un predicado con paginación, opción de incluir propiedades de navegación y retorna el total de registros
            /// </summary>
            public virtual async Task<(List<T> items, int totalCount)> FindByPaged(Expression<Func<T, bool>> predicate, int page, int pageSize, bool includeNavigationProperties)
            {
                var query = context.Set<T>().Where(predicate);

                if (!includeNavigationProperties)
                {
                    var totalCount = await query.CountAsync();
                    var items = await query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

                    return (items, totalCount);
                }

                // Por defecto, sin includes específicos - las clases derivadas deben sobrescribir este método
                var totalCountWithIncludes = await query.CountAsync();
                var itemsWithIncludes = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (itemsWithIncludes, totalCountWithIncludes);
            }

            /// <summary>
            /// Retorna un IQueryable para consultas personalizadas
            /// </summary>
            public IQueryable<T> FindByQuery(Expression<Func<T, bool>> predicate)
            {
                return context.Set<T>().Where(predicate);
            }

            /// <summary>
            /// Busca una entidad por un array de claves
            /// </summary>
            public async Task<T?> FindById(object[] aids)
            {
                return await context.Set<T>().FindAsync(aids);
            }

            /// <summary>
            /// Busca una entidad por su ID
            /// </summary>
            public async Task<T?> FindById(Guid id)
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("ID no puede ser un GUID vacío.", nameof(id));
                }

                return await context.Set<T>().FindAsync(id);
            }

            /// <summary>
            /// Busca una entidad por su ID con opción de incluir propiedades de navegación
            /// </summary>
            public virtual async Task<T?> FindById(Guid id, bool includeNavigationProperties)
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("ID no puede ser un GUID vacío.", nameof(id));
                }

                if (!includeNavigationProperties)
                {
                    return await context.Set<T>().FindAsync(id);
                }

                return await context.Set<T>().FindAsync(id);
            }

            /// <summary>
            /// Busca la primera entidad que cumpla con un predicado
            /// </summary>
            public async Task<T> FindFirst(Expression<Func<T, bool>> predicate)
            {
                return await context.Set<T>().FirstOrDefaultAsync(predicate);
            }

            /// <summary>
            /// Inserta una nueva entidad
            /// </summary>
            public async Task<T> Insert(T entity)
            {
                context.Set<T>().Add(entity);
                await context.SaveChangesAsync();
                return entity;
            }

            /// <summary>
            /// Elimina una entidad
            /// </summary>
            public async Task<T> Delete(T entity)
            {
                object[] aids = GetKeys(entity);
                entity = await context.Set<T>().FindAsync(aids);
                if (entity == null)
                {
                    return entity;
                }

                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
                return entity;
            }

            /// <summary>
            /// Inserta o actualiza una entidad según si existe o no
            /// </summary>
            public async Task<(T, bool)> InsertOrUpdate(T entity)
            {
                return await InsertOrUpdate(entity, false);
            }

            /// <summary>
            /// Inserta o actualiza una entidad con opción de actualizar solo campos no nulos
            /// </summary>
            public async Task<(T, bool)> InsertOrUpdate(T entity, bool updateOnlyNonNullFields)
            {
                var entityResult = await FindById(entity);

                if (entityResult == null)
                {
                    return (await Insert(entity), true);
                }
                else
                {
                    if (updateOnlyNonNullFields)
                    {
                        UpdateNonNullProperties(entityResult, entity);
                    }
                    else
                    {
                        context.Entry(entityResult).CurrentValues.SetValues(entity);
                    }
                    context.Entry(entityResult).State = EntityState.Modified;

                    await context.SaveChangesAsync();
                    return (entityResult, false);
                }
            }
            /// <summary>
            /// Actualiza solo las propiedades no nulas de una entidad
            /// </summary>
            private void UpdateNonNullProperties(T existingEntity, T newEntity)
            {
                var properties = typeof(T).GetProperties();
                var keyProperties = GetKeyPropertyNames();
                var entityType = context.Model.FindEntityType(typeof(T));

                foreach (var property in properties)
                {
                    // Omitir claves primarias para evitar modificar identificadores únicos
                    if (keyProperties.Contains(property.Name) || !property.CanWrite)
                        continue;

                    // Omitir propiedades de navegación para evitar problemas de lazy loading
                    var navigationProperty = entityType?.FindNavigation(property.Name);
                    if (navigationProperty != null)
                        continue;

                    var newValue = property.GetValue(newEntity);

                    // Solo actualizar si el nuevo valor no es nulo
                    if (newValue != null)
                    {
                        // Verificación para GUIDs vacíos - considerados como valores no válidos
                        if (property.PropertyType == typeof(Guid) && newValue.Equals(Guid.Empty))
                            continue;

                        // Verificación para GUIDs nullable vacíos
                        if (property.PropertyType == typeof(Guid?) && newValue.Equals(Guid.Empty))
                            continue;

                        property.SetValue(existingEntity, newValue);
                    }
                }
            }

            /// <summary>
            /// Obtiene los nombres de las propiedades que son claves primarias
            /// </summary>
            private HashSet<string> GetKeyPropertyNames()
            {
                var entityType = context.Model.FindEntityType(typeof(T));
                var keyProperties = entityType.FindPrimaryKey().Properties;
                return keyProperties.Select(p => p.Name).ToHashSet();
            }

            /// <summary>
            /// Actualiza una entidad existente
            /// </summary>
            public async Task<T> Update(T entity)
            {
                context.Entry(entity).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return entity;
            }

            /// <summary>
            /// Elimina múltiples entidades de forma bulk
            /// </summary>
            public async Task<int> BulkDelete(IEnumerable<T> entities)
            {
                if (!entities.Any()) return 0;

                context.Set<T>().RemoveRange(entities);
                return await context.SaveChangesAsync();
            }

            /// <summary>
            /// Elimina múltiples entidades que cumplan con un predicado
            /// </summary>
            public async Task<int> BulkDelete(Expression<Func<T, bool>> predicate)
            {
                var entities = await context.Set<T>().Where(predicate).ToListAsync();
                if (!entities.Any()) return 0;

                context.Set<T>().RemoveRange(entities);
                return await context.SaveChangesAsync();
            }

            /// <summary>
            /// Inserta múltiples entidades de forma bulk
            /// </summary>
            public async Task<List<T>> BulkInsert(IEnumerable<T> entities)
            {
                var entityList = entities.ToList();
                if (!entityList.Any()) return new List<T>();

                await context.Set<T>().AddRangeAsync(entityList);
                await context.SaveChangesAsync();
                return entityList;
            }
        }
    }
}

```

Luego, para cada nueva entidad, crear un repository específico que herede de la base y registre la interfaz correspondiente.
Si la clase de repositorio para una entidad existe pero no hereda de la base, modificarla para que herede de `Repository<T, Context>` y registre la interfaz `I{Entidad}Repository`. 
Si la clase de repositorio para una entidad no existe, crearla siguiendo el siguiente código de ejemplo, con esto ya hereda la funcionalidad básica del repositorio genérico. 

```csharp
using bmlabs.core.repositories.Base;
using bmlabs.core.entities;

namespace bmlabs.core.repositories
{
    /// <summary>
    /// Interfaz del repositorio para la entidad CategoriaEpp
    /// </summary>
    public interface ICategoriaEppRepository : IRepository<CategoriaEpp>
    {
    }

    /// <summary>
    /// Repositorio específico para la entidad CategoriaEpp
    /// </summary>
    public class CategoriaEppRepository : Repository<CategoriaEpp, Context>, ICategoriaEppRepository
    {
        /// <summary>
        /// Constructor del repositorio CategoriaEpp
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public CategoriaEppRepository(Context context) : base(context) { }
    }
}
```

Para agregar métodos específicos adicionales, simplemente agregar los métodos a la interfaz `I{Entidad}Repository` y luego implementarlos en la clase `{Entidad}Repository`.



```csharp

### Interfaz Específica

```csharp
using bmlabs.core.repositories.Base;
using bmlabs.core.entities;

namespace bmlabs.core.repositories
{
    /// <summary>
    /// Interfaz del repositorio para la entidad [Entidad]
    /// </summary>
    public interface I[Entidad]Repository : IRepository<[Entidad]>
    {
        // Métodos específicos adicionales si los hay
    }
}
```

### Implementación Específica Simple

```csharp
using bmlabs.core.repositories.Base;
using bmlabs.core.entities;

namespace bmlabs.core.repositories
{
    /// <summary>
    /// Repositorio específico para la entidad [Entidad]
    /// </summary>
    public class [Entidad]Repository : Repository<[Entidad], Context>, I[Entidad]Repository
    {
        /// <summary>
        /// Constructor del repositorio [Entidad]
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public [Entidad]Repository(Context context) : base(context) { }
    }
}
```

### Reglas de Nomenclatura

- **Interfaces**: `I{NombreEntidad}Repository`
- **Clases**: `{NombreEntidad}Repository`
- **Namespace**: `bmlabs.core.repositories`
- **Archivos**: `{NombreEntidad}Repository.cs`
- **Herencia**: `Repository<TEntidad, Context>`
- **Implementación**: `I{NombreEntidad}Repository`

## Funcionalidades del Repository Base

### Operaciones de Consulta

#### Buscar por ID
```csharp
// Buscar por Guid
Task<T?> FindById(Guid id);

// Buscar por ID con navegación
Task<T?> FindById(Guid id, bool includeNavigationProperties);

// Buscar por array de claves (claves compuestas)
Task<T?> FindById(object[] aids);

// Buscar por instancia de entidad
Task<T?> FindById(T entity);
```

#### Buscar con Predicados
```csharp
// Buscar con expresión lambda
Task<List<T>> FindBy(Expression<Func<T, bool>> predicate);

// Buscar con navegación
Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, bool includeNavigationProperties);

// Buscar con paginación
Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, int page, int pageSize);

// Buscar con paginación y navegación
Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, int page, int pageSize, bool includeNavigationProperties);
```

#### Buscar con Paginación Avanzada
```csharp
// Paginación que retorna total de registros
Task<(List<T> items, int totalCount)> FindByPaged(Expression<Func<T, bool>> predicate, int page, int pageSize);

// Paginación con navegación que retorna total
Task<(List<T> items, int totalCount)> FindByPaged(Expression<Func<T, bool>> predicate, int page, int pageSize, bool includeNavigationProperties);
```

#### Operaciones Adicionales
```csharp
// Obtener todas las entidades
Task<List<T>> FindAll();
Task<List<T>> FindAll(bool includeNavigationProperties);

// Buscar primera coincidencia
Task<T> FindFirst(Expression<Func<T, bool>> predicate);

// Consulta personalizada (devuelve IQueryable)
IQueryable<T> FindByQuery(Expression<Func<T, bool>> predicate);
```

### Operaciones de Modificación

#### CRUD Básico
```csharp
// Insertar nueva entidad
Task<T> Insert(T entity);

// Actualizar entidad existente
Task<T> Update(T entity);

// Eliminar entidad
Task<T> Delete(T entity);
```

#### Operaciones Avanzadas
```csharp
// Insertar o actualizar según si existe
Task<(T, bool)> InsertOrUpdate(T entity);

// Insertar o actualizar solo campos no nulos
Task<(T, bool)> InsertOrUpdate(T entity, bool updateOnlyNonNullFields);
```

#### Operaciones Masivas (Bulk)
```csharp
// Eliminar múltiples entidades
Task<int> BulkDelete(IEnumerable<T> entities);

// Eliminar por predicado
Task<int> BulkDelete(Expression<Func<T, bool>> predicate);

// Insertar múltiples entidades
Task<List<T>> BulkInsert(IEnumerable<T> entities);
```

## Implementación con Navegación

### Repository Simple (Sin Navegación)

```csharp
using bmlabs.core.repositories.Base;
using bmlabs.core.entities;

namespace bmlabs.core.repositories
{
    /// <summary>
    /// Interfaz del repositorio para la entidad Area
    /// </summary>
    public interface IAreaRepository : IRepository<Area>
    {
    }

    /// <summary>
    /// Repositorio específico para la entidad Area
    /// </summary>
    public class AreaRepository : Repository<Area, Context>, IAreaRepository
    {
        /// <summary>
        /// Constructor del repositorio Area
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public AreaRepository(Context context) : base(context) { }
    }
}
```

### Repository con Navegación Básica

```csharp
using System.Linq.Expressions;
using bmlabs.core.entities;
using bmlabs.core.repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace bmlabs.core.repositories
{
    /// <summary>
    /// Interfaz del repositorio para la entidad Epp
    /// </summary>
    public interface IEppRepository : IRepository<Epp>
    {
    }

    /// <summary>
    /// Repositorio específico para la entidad Epp
    /// </summary>
    public class EppRepository : Repository<Epp, Context>, IEppRepository
    {
        /// <summary>
        /// Constructor del repositorio Epp
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public EppRepository(Context context) : base(context) { }

        // Override para FindAll con navegación
        public override async Task<List<Epp>> FindAll(bool includeNavigationProperties = false)
        {
            if (!includeNavigationProperties)
            {
                return await Context().Set<Epp>().ToListAsync();
            }

            return await Context().Set<Epp>()
                .Include(e => e.IdCategoriaEppNavigation)
                .Include(e => e.UsuarioUltModificacionNavigation)
                .ToListAsync();
        }

        // Override para FindBy con navegación
        public override async Task<List<Epp>> FindBy(Expression<Func<Epp, bool>> predicate, bool includeNavigationProperties)
        {
            if (!includeNavigationProperties)
            {
                return await Context().Set<Epp>().Where(predicate).ToListAsync();
            }

            return await Context().Set<Epp>()
                .Include(e => e.IdCategoriaEppNavigation)
                .Include(e => e.UsuarioUltModificacionNavigation)
                .Where(predicate)
                .ToListAsync();
        }

        // Override para paginación con navegación
        public override async Task<(List<Epp> items, int totalCount)> FindByPaged(Expression<Func<Epp, bool>> predicate, int page, int pageSize, bool includeNavigationProperties = false)
        {
            if (!includeNavigationProperties)
            {
                return await base.FindByPaged(predicate, page, pageSize, false);
            }

            var query = Context().Set<Epp>()
                .Include(e => e.IdCategoriaEppNavigation)
                .Include(e => e.UsuarioUltModificacionNavigation)
                .Where(predicate);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
```

### Repository con Navegación Compleja

```csharp
using System.Linq.Expressions;
using bmlabs.core.entities;
using bmlabs.core.repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace bmlabs.core.repositories
{
    /// <summary>
    /// Interfaz del repositorio para la entidad Usuario
    /// </summary>
    public interface IUsuarioRepository : IRepository<Usuario>
    {
    }

    /// <summary>
    /// Repositorio específico para la entidad Usuario
    /// </summary>
    public class UsuarioRepository : Repository<Usuario, Context>, IUsuarioRepository
    {
        /// <summary>
        /// Constructor del repositorio Usuario
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public UsuarioRepository(Context context) : base(context) { }

        /// <summary>
        /// Override para incluir las propiedades de navegación específicas de Usuario
        /// </summary>
        public override async Task<List<Usuario>> FindBy(Expression<Func<Usuario, bool>> predicate, bool includeNavigationProperties)
        {
            if (!includeNavigationProperties)
            {
                return await Context().Set<Usuario>().Where(predicate).ToListAsync();
            }

            return await Context().Set<Usuario>()
                .Include(u => u.IdDivisionNavigation)
                .Include(u => u.IdUsuarioEstadoNavigation)
                .Include(u => u.IdCargoNavigation)
                .Include(u => u.IdSubAreaNavigation!)
                    .ThenInclude(csa => csa.IdAreaNavigation!)
                .Include(u => u.IdRecintoNavigation)
                .Include(u => u.IdCentroCostoNavigation)
                .Include(u => u.UsuarioRol)
                    .ThenInclude(ur => ur.IdRolNavigation)
                .Where(predicate)
                .ToListAsync();
        }

        public override async Task<(List<Usuario> items, int totalCount)> FindByPaged(Expression<Func<Usuario, bool>> predicate, int page, int pageSize, bool includeNavigationProperties = false)
        {
            if (!includeNavigationProperties)
            {
                return await base.FindByPaged(predicate, page, pageSize, false);
            }

            var query = Context().Set<Usuario>()
                .Include(u => u.IdDivisionNavigation)
                .Include(u => u.IdUsuarioEstadoNavigation)
                .Include(u => u.IdCargoNavigation)
                .Include(u => u.IdSubAreaNavigation!)
                    .ThenInclude(sa => sa.IdAreaNavigation!)
                .Include(u => u.IdRecintoNavigation)
                .Include(u => u.IdCentroCostoNavigation)
                .Include(u => u.UsuarioRol)
                    .ThenInclude(ur => ur.IdRolNavigation)
                .Where(predicate);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
```

## Métodos que Requieren Override

### FindAll con Navegación
```csharp
public override async Task<List<T>> FindAll(bool includeNavigationProperties = false)
{
    if (!includeNavigationProperties)
    {
        return await Context().Set<T>().ToListAsync();
    }

    return await Context().Set<T>()
        .Include(x => x.PropiedadNavegacion)
        .Include(x => x.OtraPropiedadNavegacion)
        .ToListAsync();
}
```

### FindBy con Navegación
```csharp
public override async Task<List<T>> FindBy(Expression<Func<T, bool>> predicate, bool includeNavigationProperties)
{
    if (!includeNavigationProperties)
    {
        return await Context().Set<T>().Where(predicate).ToListAsync();
    }

    return await Context().Set<T>()
        .Include(x => x.PropiedadNavegacion)
        .Include(x => x.OtraPropiedadNavegacion)
        .Where(predicate)
        .ToListAsync();
}
```

### FindByPaged con Navegación
```csharp
public override async Task<(List<T> items, int totalCount)> FindByPaged(Expression<Func<T, bool>> predicate, int page, int pageSize, bool includeNavigationProperties = false)
{
    if (!includeNavigationProperties)
    {
        return await base.FindByPaged(predicate, page, pageSize, false);
    }

    var query = Context().Set<T>()
        .Include(x => x.PropiedadNavegacion)
        .Include(x => x.OtraPropiedadNavegacion)
        .Where(predicate);

    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return (items, totalCount);
}
```

### FindById con Navegación (Opcional)
```csharp
public override async Task<T?> FindById(Guid id, bool includeNavigationProperties)
{
    if (id == Guid.Empty)
    {
        throw new ArgumentException("ID no puede ser un GUID vacío.", nameof(id));
    }

    if (!includeNavigationProperties)
    {
        return await Context().Set<T>().FindAsync(id);
    }

    return await Context().Set<T>()
        .Include(x => x.PropiedadNavegacion)
        .Include(x => x.OtraPropiedadNavegacion)
        .FirstOrDefaultAsync(x => x.Id == id);
}
```

## Patrones de Include Avanzados

### Include Simple
```csharp
.Include(u => u.IdCargoNavigation)
.Include(u => u.IdDivisionNavigation)
```

### ThenInclude para Navegación Anidada
```csharp
.Include(u => u.IdSubAreaNavigation!)
    .ThenInclude(sa => sa.IdAreaNavigation!)
```

### Include con Colecciones
```csharp
.Include(u => u.UsuarioRol)
    .ThenInclude(ur => ur.IdRolNavigation)
```

### Include Múltiple Anidado
```csharp
.Include(s => s.IdUsuarioColaboradorNavigation!)
    .ThenInclude(u => u.IdCargoNavigation)
.Include(s => s.IdUsuarioColaboradorNavigation!)
    .ThenInclude(u => u.IdRecintoNavigation)
.Include(s => s.IdUsuarioColaboradorNavigation!)
    .ThenInclude(u => u.IdSubAreaNavigation!)
        .ThenInclude(sa => sa.IdAreaNavigation!)
```

## Registro de Dependencias

### Extensión para DI

Si no existe este archivo debe ser creado para centralizar el registro de repositorios en el contenedor de dependencias.

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace bmlabs.core.repositories
{
    /// <summary>
    /// Extensiones para registro de dependencias de repositorios
    /// </summary>
    public static class DependencyInjectionRepositories
    {
        /// <summary>
        /// Registra todos los repositorios en el contenedor de dependencias
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <returns>Colección de servicios configurada</returns>
        public static void AddRepositories(IServiceCollection services)
        {
            // Registrar repositorios como Scoped para que se creen por request
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IEppRepository, EppRepository>();
            services.AddScoped<ISolicitudRepository, SolicitudRepository>();
            // ... más repositorios
        }
    }
}
```

### Uso en Program.cs o Startup.cs para registrar los repositorios en el contenedor de dependencias.
```csharp
// Registrar repositorios
builder.Services.AddRepositories();
```

## Uso en Services 

### Inyección en Constructor
```csharp
public class UsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }
}
```

### Operaciones Básicas
```csharp
// Buscar por ID
var usuario = await _usuarioRepository.FindById(id);

// Buscar por ID con navegación
var usuario = await _usuarioRepository.FindById(id, includeNavigationProperties: true);

// Buscar con filtros
var usuarios = await _usuarioRepository.FindBy(u => u.Activo && u.IdDivision == divisionId);

// Buscar con paginación
var (items, total) = await _usuarioRepository.FindByPaged(
    u => u.Nombre.Contains(search), 
    page: 1, 
    pageSize: 10, 
    includeNavigationProperties: true);

// Crear nuevo
var nuevoUsuario = await _usuarioRepository.Insert(usuario);

// Actualizar
var usuarioActualizado = await _usuarioRepository.Update(usuario);

// Insertar o actualizar
var (resultado, esNuevo) = await _usuarioRepository.InsertOrUpdate(usuario);
```

## Mejores Prácticas

### Navegación
- **Override necesario**: Siempre sobrescribir métodos cuando se necesite navegación
- **Include específicos**: Solo incluir las propiedades de navegación necesarias
- **ThenInclude**: Usar para navegación anidada
- **Null-forgiving**: Usar `!` para propiedades que sabemos que no son null en el contexto

### Performance
- **Paginación**: Usar métodos `FindByPaged` para listas grandes  
- **Navegación selectiva**: Solo cargar navegación cuando sea necesaria
- **AsNoTracking**: Considerar para consultas solo de lectura

### Validación
- **Guid.Empty**: La base valida GUIDs vacíos automáticamente
- **Null checks**: El repository base maneja casos null apropiadamente
- **Claves primarias**: GetKeys() maneja claves compuestas automáticamente

### Contexto
- **Scoped**: Repositories se registran como Scoped (uno por request HTTP)
- **SaveChanges**: La base llama SaveChanges automáticamente en Insert/Update/Delete
- **Transacciones**: Usar Unit of Work pattern si se necesitan transacciones complejas

## Checklist para Nuevo Repository

- [ ] Archivo creado en `core/repositories/`
- [ ] Namespace correcto: `bmlabs.core.repositories`
- [ ] Interfaz definida: `I{Entidad}Repository : IRepository<{Entidad}>`
- [ ] Implementación: `{Entidad}Repository : Repository<{Entidad}, Context>, I{Entidad}Repository`
- [ ] Constructor con Context apropiado
- [ ] Comentarios XML para interfaz y clase
- [ ] Override de métodos con navegación si la entidad tiene propiedades de navegación
- [ ] Include statements específicos para la entidad
- [ ] ThenInclude para navegación anidada si aplica
- [ ] Registro en DependencyInjectionExtensions.cs
- [ ] Pruebas de funcionamiento básico

## Ejemplo de Repository Completo

```csharp
using System.Linq.Expressions;
using bmlabs.core.entities;
using bmlabs.core.repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace bmlabs.core.repositories
{
    /// <summary>
    /// Interfaz del repositorio para la entidad MiEntidad
    /// </summary>
    public interface IMiEntidadRepository : IRepository<MiEntidad>
    {
        // Métodos adicionales específicos si los hay
    }

    /// <summary>
    /// Repositorio específico para la entidad MiEntidad
    /// </summary>
    public class MiEntidadRepository : Repository<MiEntidad, Context>, IMiEntidadRepository
    {
        /// <summary>
        /// Constructor del repositorio MiEntidad
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public MiEntidadRepository(Context context) : base(context) { }

        public override async Task<List<MiEntidad>> FindAll(bool includeNavigationProperties = false)
        {
            if (!includeNavigationProperties)
            {
                return await Context().Set<MiEntidad>().ToListAsync();
            }

            return await Context().Set<MiEntidad>()
                .Include(m => m.CategoriaNavigation)
                .Include(m => m.UsuarioNavigation)
                .ToListAsync();
        }

        public override async Task<List<MiEntidad>> FindBy(Expression<Func<MiEntidad, bool>> predicate, bool includeNavigationProperties)
        {
            if (!includeNavigationProperties)
            {
                return await Context().Set<MiEntidad>().Where(predicate).ToListAsync();
            }

            return await Context().Set<MiEntidad>()
                .Include(m => m.CategoriaNavigation)
                .Include(m => m.UsuarioNavigation)
                .Where(predicate)
                .ToListAsync();
        }

        public override async Task<(List<MiEntidad> items, int totalCount)> FindByPaged(Expression<Func<MiEntidad, bool>> predicate, int page, int pageSize, bool includeNavigationProperties = false)
        {
            if (!includeNavigationProperties)
            {
                return await base.FindByPaged(predicate, page, pageSize, false);
            }

            var query = Context().Set<MiEntidad>()
                .Include(m => m.CategoriaNavigation)
                .Include(m => m.UsuarioNavigation)
                .Where(predicate);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
```

