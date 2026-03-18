# Copilot Instructions - PrevencionRiesgos

## Objetivo

Este repositorio implementa APIs backend con ASP.NET Core 10+, agnóstico a la base de datos y Entity Framework Core, siguiendo una arquitectura por capas.

Al generar o modificar código, prioriza seguridad, mantenibilidad, observabilidad y contratos HTTP estables.

## Estructura del Repositorio

Estructura esperada del proyecto:

```text
controllers/
core/
  dto/
    mapping/
    validator/
  entities/
  exceptions/
  repositories/
  services/
documentation/
helpers/
```

Responsabilidades por carpeta:

- controllers: endpoints HTTP, binding, códigos de estado, metadata OpenAPI, autorización.
- core/dto: contratos request/response orientados a API, no entidades EF.
- core/dto/mapping: perfiles AutoMapper entre entidades y DTOs.
- core/dto/validator: reglas FluentValidation de entrada.
- core/entities: entidades de dominio/persistencia.
- core/repositories: acceso a datos con EF Core (sin reglas de negocio).
- core/services: casos de uso y lógica de negocio.
- core/exceptions: excepciones de dominio/aplicación.
- helpers: utilidades transversales reutilizables (sin lógica de negocio).

## Principios de Desarrollo

- Controllers delgados; la lógica de negocio vive en services.
- Repositories solo para persistencia/consultas.
- Nunca exponer entidades directamente como contrato público.
- Validar input en el borde (controllers/endpoints).
- Usar asincronía real (`async/await`) en operaciones I/O.
- Propagar `CancellationToken` en capas descendentes.
- Evitar secretos hardcodeados; usar configuración/variables de entorno.

## Estándar API REST en .NET

### Diseño de recursos

- Rutas versionadas: `/api/v1/...`.
- URLs orientadas a recursos, no a acciones.
- Verbos HTTP consistentes:
  - GET: lectura.
  - POST: creación.
  - PUT/PATCH: actualización.
  - DELETE: eliminación.

### Códigos de estado

Usar códigos explícitos y consistentes:

- 200 OK, 201 Created, 204 No Content.
- 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found.
- 409 Conflict, 422 Unprocessable Entity.
- 500 Internal Server Error solo para fallos no controlados.

### Errores estandarizados

- Responder errores con Problem Details (RFC 9457).
- Implementar manejo global de excepciones con middleware.
- No exponer stack traces ni datos internos en respuestas.

### Validación

- FluentValidation como mecanismo principal de validación.
- Diferenciar validación de formato (input) de validación de negocio.
- Mensajes de validación claros y accionables.

### Versionado y documentación

- Mantener versionado explícito en rutas o estrategia uniforme definida por el proyecto.
- Documentar endpoints, request/response y códigos esperados en Swagger/OpenAPI.
- Documentar autenticación (Bearer/JWT) en OpenAPI.

## Patrones de Implementación

### DTOs

- Usar nombres explícitos (ejemplo: `CreateXRequest`, `UpdateXRequest`, `XResponse`).
- Para IDs usar `Guid`.
- Para fechas usar `DateTimeOffset` en UTC.
- Evitar DTOs ambiguos o con responsabilidades mixtas.

### Repositories

- Interfaces y clases concretas por entidad cuando aplique.
- Consultas de lectura con `AsNoTracking()` cuando no haya actualización.
- Evitar `Include` innecesario y sobrecarga de navegación.
- No contener decisiones de negocio.

### Services

- Orquestan validaciones, repositorios y reglas de negocio.
- Devuelven resultados listos para capa HTTP (vía DTOs o contratos de aplicación).
- Mantener métodos pequeños, cohesionados y testeables.

### Controllers

- Inyección por constructor.
- Sin acceso directo a `DbContext`.
- Respuestas tipadas y coherentes.

## Seguridad

- Autenticación con JWT Bearer (o estándar vigente del proyecto).
- Autorización por políticas/roles/claims según caso de uso.
- No confiar en datos sensibles enviados por el cliente si se pueden inferir del token/contexto.
- Sanitizar y validar entradas para evitar abuso de datos.

## Observabilidad

- Logging estructurado (niveles correctos: Information, Warning, Error, Critical).
- Registrar eventos relevantes con contexto (trace/correlation id si existe).
- Evitar ruido y duplicación de logs.

## Rendimiento

- Evitar N+1 queries y cargas excesivas.
- Paginación obligatoria en listados grandes (ejemplo: `page`, `pageSize`, máximo 100).
- Considerar filtros, ordenamiento y búsqueda eficiente.
- Usar compresión/caché cuando el caso lo justifique.

## Testing

- Unit tests para services y reglas de negocio.
- Integration tests para controllers/repositories/endpoints.
- Cubrir caso feliz, validaciones, errores esperados y bordes críticos.

## Flujo de Trabajo Recomendado (A→F)


2. a. Entidades desde DB (scaffold EF Core cuando corresponda).
3. b. DTOs, validadores y mapeos.
4. c. Repositories.
5. d. Services.
6. e. Controllers.

## Configuración y Entorno

- Usar `.env` con DotNetEnv cuando aplique.
- Variables típicas: `DB_*`, `JWT_*`, `API_KEY_CARGA_CERTIFICADO`.
- Configuración por ambiente en `appsettings.*.json`.

## Reglas para Copilot

- Seguir convenciones existentes del repositorio antes de introducir nuevas.
- No crear documentación adicional en Markdown, salvo solicitud explícita.
- No agregar comentarios innecesarios; documentar solo el porqué cuando sea relevante.
- Priorizar código claro, explícito y alineado con Clean Architecture.

## Skills Disponibles

Las siguientes skills proporcionan guías específicas para crear componentes siguiendo las convenciones del proyecto:

| Skill | Descripción |
|-------|-------------|
| `be-create-entities` | Guía para crear entidades de base de datos en .NET Core siguiendo las convenciones del proyecto. |
| `be-create-dtos` | Guía para crear DTOs (Data Transfer Objects) en .NET Core. |
| `be-create-mappings` | Guía para crear mappings en .NET Core usando AutoMapper. |
| `be-create-controller` | Guía para crear controllers en .NET Core. |
| `be-create-service` | Guía para crear servicios en .NET Core siguiendo las convenciones del proyecto. |
| `be-create-repository` | Guía para crear repositories en .NET Core. |
| `be-create-validators` | Guía para crear validators en .NET Core siguiendo las convenciones del proyecto usando FluentValidation. |

Estas skills siguen la arquitectura por capas definida y aseguran consistencia con las convenciones establecidas en el proyecto.