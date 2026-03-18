# Guía del proyecto

## Cómo usar esta guía

- Este archivo define las reglas globales del repositorio.
- Este repositorio se utiliza para construir APIs backend en ASP.NET Core / .NET 8 o superior.
- Cada carpeta principal puede contener sus propios archivos `AGENTS.md`  con instrucciones más específicas.
- Si existe conflicto entre este archivo y uno ubicado en una carpeta más específica, prevalece el archivo más cercano al código que se está editando.
- Siempre prioriza el `AGENTS.md` más cercano dentro del árbol de directorios.
- Usa los archivos `SKILL.md` como guía práctica de implementación cuando una carpeta tenga una receta, convención o estilo específico.

## Propósito del repositorio

Este repositorio está orientado a la construcción de APIs backend seguras, mantenibles, observables y listas para operación en ambientes productivos.

Las carpetas principales del proyecto son:
- `controllers`
- `core/entities`
- `core/repositories`
- `core/dto`
- `core/dto/mapping`
- `core/dto/validator`
- `core/services`
- `helpers`

## Visión general de la arquitectura

Este proyecto sigue una arquitectura por capas, con separación clara de responsabilidades:

- `controllers`: puntos de entrada HTTP, manejo de requests, status codes y contratos de API.
- `core/entities`: entidades de negocio y modelos de dominio.
- `core/repositories`: abstracciones y lógica de acceso a datos.
- `core/dto`: DTOs de entrada y salida, modelos orientados al contrato de API.
- `core/dto/mapping`: mappings entre dtos y entities usando automapper.
- `core/dto/validator`: Validadores para dtos.
- `core/services`: lógica de aplicación, reglas de negocio y orquestación de casos de uso.
- `helpers`: utilidades compartidas, extensiones, helpers genéricos y soporte transversal sin lógica de dominio.

## Principios base

- Mantén los controllers delgados.
- No pongas lógica de negocio en controllers.
- No accedas a persistencia directamente desde controllers.
- No expongas entidades directamente como contrato de API.
- Usa DTOs para requests y responses.
- Mantén los services enfocados en lógica de negocio y de aplicación.
- Mantén los repositories enfocados en acceso a datos.
- Mantén los helpers genéricos, reutilizables y sin lógica de dominio.
- Prefiere código explícito, legible y mantenible por sobre abstracciones “mágicas”.
- No hardcodees secretos, tokens, passwords, URLs ni connection strings.
## Skills disponibles

Las siguientes skills proporcionan capacidades especializadas y flujos de trabajo refinados para producir outputs de alta calidad específicos para este proyecto .NET Core:

| Skill | Descripción |
|-------|-------------|
| `be-create-entities` | Guía para crear entidades de base de datos en .NET Core siguiendo las convenciones del proyecto. |
| `be-create-dtos` | Guía para crear DTOs (Data Transfer Objects) en .NET Core. |
| `be-create-mappings` | Guía para crear mappings en .NET Core usando AutoMapper. |
| `be-create-controllers` | Guía para crear controllers en .NET Core. |
| `be-create-services` | Guía para crear servicios en .NET Core siguiendo las convenciones del proyecto. |
| `be-create-repositories` | Guía para crear repositories en .NET Core. |
| `be-create-validators` | Guía para crear validators en .NET Core siguiendo las convenciones del proyecto usando FluentValidation. |- Toda configuración sensible o dependiente del ambiente debe leerse desde configuración externa.

## Línea base tecnológica

Salvo que un `AGENTS.md` más cercano indique otra cosa, asume por defecto:

- ASP.NET Core Web API
- .NET 8 o superior
- Inyección de dependencias con el contenedor nativo de ASP.NET Core
- Entity Framework Core para persistencia
- FluentValidation para validación, si está presente en el proyecto
- Swagger / OpenAPI habilitado
- Logging estructurado con `ILogger<T>` y/o Serilog
- xUnit para testing
- Diseño compatible con Docker

## Skills disponibles

Los archivos `SKILL.md` de cada carpeta deben usarse como guías especializadas de implementación.

### Skills esperadas por carpeta

| Carpeta             | Propósito de la skill local                                                              |
| -------             | ---------------------------                                                              |
| `controllers`       | patrones de endpoints, versionado, HTTP codes, auth, Swagger                             |
| `core/dto`          | nombres de DTOs, contratos request/response, paginación, filtros, límites de mapeo       |
| `core/entities`     | modelado de entidades, invariantes, diseño persistente, convenciones                     |
| `core/repositories` | patrones de repositorio, acceso con EF Core, queries async, convenciones de persistencia |
| `core/services`     | orquestación de casos de uso, reglas de negocio, validación, límites transaccionales     |
| `helpers`           | utilidades reutilizables, extensiones, límites de uso, anti-patrones                     |

## Orden de aplicación del contexto por carpeta

Cuando trabajes dentro de una carpeta, aplica las instrucciones en este orden:

1. `AGENTS.md` más cercano
3. este `AGENTS.md` de raíz

Si las instrucciones locales contradicen a las globales, prevalecen las locales.

## Reglas de diseño de API

- Usa convenciones REST, salvo que el proyecto ya tenga otro estilo definido.
- Prefiere rutas versionadas como `/api/v1/...` cuando esa convención exista.
- Devuelve respuestas HTTP consistentes y predecibles.
- Usa códigos HTTP correctos: `200`, `201`, `204`, `400`, `401`, `403`, `404`, `409`, `422`, `500`.
- Usa DTOs específicos de request y response cuando el contrato no sea trivial.
- En endpoints de listado, incorpora paginación, filtros y ordenamiento cuando corresponda.
- Mantén contratos claros, explícitos y estables.

## Reglas para controllers

Cuando crees o modifiques código en `controllers`:

- Los controllers deben encargarse solo del flujo HTTP.
- Los controllers pueden:
  - recibir y bindear requests;
  - llamar a services;
  - transformar resultados en respuestas HTTP;
  - aplicar autorización, rutas y metadata;
  - exponer metadata para Swagger cuando aporte valor.
- Los controllers no deben:
  - contener reglas de negocio;
  - contener acceso a datos;
  - construir flujos complejos de dominio;
  - ejecutar operaciones de persistencia directamente.

## Reglas para DTOs

Cuando crees o modifiques código en `core/dto`:

- Usa DTOs para aislar el contrato de API respecto de las entidades.
- Prefiere nombres explícitos, por ejemplo:
  - `CreateXRequest`
  - `UpdateXRequest`
  - `XResponse`
  - `XListResponse`
- Mantén los DTOs enfocados en transporte de datos.
- No filtres detalles de persistencia hacia DTOs salvo que el contrato lo requiera.
- Evita referencias circulares y estructuras excesivamente anidadas.

## Reglas para entities

Cuando crees o modifiques código en `core/entities`:

- Mantén las entidades enfocadas en el significado del dominio.
- Usa nombres descriptivos y propiedades explícitas.
- Conserva invariantes cuando el diseño del proyecto lo soporte.
- Evita lógica acoplada fuertemente al framework salvo que ya sea una convención del proyecto.
- No diseñes entidades pensando en la forma del controller; diseña DTOs pensando en la API.

## Reglas para repositories

Cuando crees o modifiques código en `core/repositories`:

- Los repositories son responsables del acceso a datos.
- Mantén asincronía en todas las operaciones I/O.
- No muevas lógica de negocio a repositories.
- Mantén queries cohesivas y expresivas.
- Respeta los patrones existentes de EF Core, por ejemplo:
  - uso consistente de `DbContext`;
  - `AsNoTracking()` para lecturas cuando corresponda;
  - `Include` solo cuando sea necesario;
  - propagación de `CancellationToken`.
- No devuelvas detalles de infraestructura hacia controllers.

## Reglas para services

Cuando crees o modifiques código en `core/services`:

- Los services implementan casos de uso y lógica de aplicación/negocio.
- Los services coordinan validaciones, llamadas a repositories y decisiones de negocio.
- Los services deben ser cohesivos y pequeños.
- Prefiere inyección por constructor.
- Prefiere dependencias explícitas.
- Mantén claros los límites transaccionales cuando aplique.
- Propaga `CancellationToken` hacia llamadas asincrónicas descendentes.

## Reglas para helpers

Cuando crees o modifiques código en `helpers`:

- Los helpers deben seguir siendo genéricos y reutilizables.
- Los helpers no deben transformarse en un contenedor de lógica de negocio.
- Prefiere métodos de extensión o utilidades focalizadas cuando mejoren claridad.
- No pongas lógica de persistencia, de controller ni de flujos de dominio en helpers.
- Si un helper pasa a ser específico de una capa o dominio, muévelo más cerca de su capa propietaria.

## Reglas de validación

- Valida entrada en el borde.
- Usa `FluentValidation` cuando exista y sea consistente con el proyecto.
- Separa validaciones de formato/entrada de validaciones de negocio.
- No confíes en identificadores críticos enviados por el cliente si esos datos pueden obtenerse desde el contexto autenticado o desde el servidor.

## Reglas de manejo de errores

- Prefiere middleware centralizado de excepciones si existe.
- Usa `ProblemDetails` o el contrato de error estándar del proyecto.
- No expongas stack traces ni detalles internos de infraestructura en respuestas públicas.
- Registra excepciones con suficiente contexto operativo, pero nunca incluyas secretos o datos sensibles.

## Reglas de seguridad

- Usa autenticación y autorización según el estándar del proyecto, normalmente JWT o Azure AD.
- Usa claims, roles o policies cuando corresponda.
- No confíes en contexto de seguridad enviado por el cliente si puede derivarse del token.
- No loggees secretos, tokens, passwords, connection strings ni datos personales sensibles.
- Respeta CORS y la separación por ambientes configurada en el proyecto.

## Reglas de logging y observabilidad

- Usa logging estructurado.
- Registra hitos relevantes y fallos útiles para operación.
- Usa niveles correctos de log: `Information`, `Warning`, `Error`, `Critical`.
- Preserva correlation id, trace id y contexto de request si el proyecto lo soporta.
- Evita logs ruidosos o excesivos.

## Reglas de calidad de código

- Prefiere métodos pequeños y con una sola responsabilidad.
- Usa nombres descriptivos.
- Evita duplicación.
- No introduzcas abstracciones innecesarias.
- No agregues nuevas librerías salvo que sea realmente necesario y coherente con el proyecto.
- Comenta el por qué, no lo obvio.

## Reglas de testing

Cuando implementes una feature, considera también su impacto en pruebas.

- Agrega o propone pruebas para el nuevo comportamiento.
- Prefiere:
  - unit tests para services y reglas de negocio;
  - integration tests para controllers y repositories cuando corresponda.
- Cubre:
  - caso feliz;
  - errores de validación;
  - errores de negocio esperados;
  - casos borde que realmente aporten valor.

## Resultado esperado al crear una nueva feature

Cuando se solicite crear una nueva feature de API, normalmente se espera:

- request DTO
- response DTO
- reglas de validación
- interfaz y clase de service
- interfaz y clase de repository si aplica
- endpoint en controller
- registro en DI
- contrato amigable para Swagger/OpenAPI
- pruebas o una propuesta concreta de pruebas

## Qué no hacer

- No pongas lógica de negocio en controllers.
- No retornes entidades directamente desde la API.
- No hardcodees valores de ambiente.
- No pongas lógica de dominio en helpers.
- No uses repositories como reemplazo de services.
- No crees services gigantes con múltiples responsabilidades no relacionadas.
- No tragues excepciones silenciosamente.
- No inventes patrones arquitectónicos que el repositorio no esté usando.

## Archivos locales sugeridos

Archivos opcionales recomendados por carpeta:

- `controllers/AGENTS.md`
- `controllers/SKILL.md`
- `core/dto/AGENTS.md`
- `core/dto/SKILL.md`
- `core/entities/AGENTS.md`
- `core/entities/SKILL.md`
- `core/repositories/AGENTS.md`
- `core/repositories/SKILL.md`
- `core/services/AGENTS.md`
- `core/services/SKILL.md`
- `helpers/AGENTS.md`
- `helpers/SKILL.md`

## Intención sugerida para archivos locales

Ejemplos de qué debería contener cada archivo de carpeta:

- `controllers/AGENTS.md`: estilo de rutas, auth, status codes, reglas de controllers delgados.
- `controllers/SKILL.md`: cómo crear un endpoint CRUD, cómo documentar Swagger, cómo aplicar paginación.
- `core/dto/AGENTS.md`: reglas de nombres y separación de contratos.
- `core/dto/SKILL.md`: ejemplos de DTOs create, update, list.
- `core/entities/AGENTS.md`: restricciones de modelado de dominio y persistencia.
- `core/entities/SKILL.md`: patrones válidos de diseño de entidades.
- `core/repositories/AGENTS.md`: límites del repository, reglas de query, convenciones EF Core.
- `core/repositories/SKILL.md`: ejemplos de lecturas, escrituras y métodos async.
- `core/services/AGENTS.md`: reglas de orquestación, límites de dependencias y validación.
- `core/services/SKILL.md`: ejemplos de métodos de servicio, flujo de validación y transacciones.
- `helpers/AGENTS.md`: límites de helpers y anti-patrones.
- `helpers/SKILL.md`: ejemplos de helpers y extensiones aceptables.

## Regla final

Ante la duda, prioriza siempre:

- la guía más cercana a la carpeta;
- la convención ya existente del repositorio;
- la solución más simple, clara y lista para producción en ASP.NET Core.