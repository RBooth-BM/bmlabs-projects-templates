---
name: vue-create-api-service
description: Genera servicios de API como clases estáticas tipadas usando el httpClient centralizado.
license: Proprietary
compatibility: Vue 3, Axios, TypeScript
metadata:
  author: Ricardo B.T.
  version: 2.0.0
  standard: BM Static Class
---

# Skill: Crear API Service (Data Transport)

## Propósito
Implementar la capa de transporte de datos del proyecto usando clases estáticas tipadas que consumen el `httpClient` centralizado con interceptores de auth, error y loading.

## Invocación
```bash
/B [Entidad] # Genera el servicio de API para la entidad
```

### 0. Revisión Obligatoria del OpenAPI
**ANTES de generar cualquier código, DEBES:**
- Consultar el Swagger/OpenAPI proporcionado por el usuario.
- Identificar el método HTTP correcto para cada endpoint (GET, POST, PATCH, PUT, DELETE).
- Mapear los tipos de datos y formatos (string/uuid, number/int32, etc.).
- Verificar si hay endpoints separados para Create/Update o un endpoint unificado.
- Confirmar que cada endpoint a implementar exista literalmente en el OpenAPI (path + método).
- Verificar la ruta final compuesta (`apiConfig.baseURL + BASE_URL + endpoint`) para evitar duplicación de segmentos o `//`.

### 1. Uso de Cliente Centralizado
- Todo servicio DEBE importar `httpClient` desde `@/services/api/http-client`.
- **Prohibido** crear instancias de Axios o importar `axios` directamente.
- El `httpClient` inyecta automáticamente el token Bearer desde `sessionStorage('auth_token')`.
- En 401, el interceptor limpia sesión y redirige a `/auth/login`.

### 2. Patrón: Clase Estática
- `export class EntityService { static async method() {} }`
- `private static readonly BASE_URL = '/endpoint'`. **REGLA CRÍTICA**: Debe ser relativo a `apiConfig.baseURL`. Si el swagger dice `/api/v1/Auth` y el baseURL es `/api`, el BASE_URL del servicio debe ser `/v1/Auth`.
- Si el base path o subpaths se reutilizan entre servicios, moverlos a `src/constants/` y reutilizarlos para evitar hardcode repetido.
- **NO** objetos const. **NO** export default.
- **MÉTODO HTTP CORRECTO**: Usar el método especificado en el OpenAPI (POST, PATCH, PUT, etc.). No asumir POST para updates.- **ENDPOINT EXISTENTE**: Solo generar métodos para paths que aparezcan en el Swagger. Si no existe `DELETE`, no crear `delete()`.
### 3. DTOs y Tipado
- Interfaces DTO exportadas junto al servicio o en `@/types/`.
- Retornos siempre tipados: `httpClient.get<TipoRespuesta>(...)`.
- Interfaz `PaginatedResponse<T>` para endpoints paginados.

### 4. Estructura de Archivos
```
src/services/api/
  http-client.ts              # Cliente Axios centralizado
  services/
    auth.service.ts           # Servicio de autenticación
    users.service.ts           # Ejemplo de servicio CRUD
    [entity].service.ts        # Nuevo servicio
```

## Checklist de Calidad
- [ ] ¿Revisó el OpenAPI para determinar métodos HTTP correctos?
- [ ] ¿Importa `httpClient` del proyecto?
- [ ] ¿Clase con métodos `static async`?
- [ ] ¿`BASE_URL` como propiedad `private static readonly`?
- [ ] ¿Cada endpoint implementado existe en Swagger (path + método)?
- [ ] ¿Ruta final compuesta validada sin duplicidades (`/api/api`, `//`)?
- [ ] ¿Todos los retornos tipados con interfaces?
- [ ] ¿Método `getPaged()` con paginación?
- [ ] ¿Sin lógica de negocio (solo transporte)?
- [ ] ¿0 usos de `any`?

## Que Genera
- `[entity].service.ts`: Clase estática con métodos CRUD tipados.

## Referencias y Código Reutilizable
**DEBES leer estas referencias antes de generar código:**
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/REAL_API.md)
