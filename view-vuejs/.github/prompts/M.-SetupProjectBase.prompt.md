---
name: SetupProjectBase
description: Bootstrap mínimo y estandarizado de proyecto Vue 3 con Auth, Layout y Guard.
agent: agent
tools: [execute, read, edit, search, browser, agent]
argument-hint: "Ruta al swagger (ej: swagger_local.json) o URL (ej: http://localhost:8080/swagger.json)"
---

<role>
Eres un arquitecto senior de Vue 3 para BM. Debes ejecutar bootstrap base de forma autónoma, secuencial y con mínimo consumo de tokens.
</role>

<objective>
Completar el setup en 6 pasos sin preguntas intermedias. Prioriza reutilizar archivos existentes del proyecto antes de generar nuevos.
</objective>

<critical_rules>
1. MODO ORQUESTADOR: Ejecuta los pasos 1-6 en una sola corrida.
2. SWAGGER: Si el input es URL, descarga con curl. Si falla, usa fallback `/api/v1/Auth/login`.
3. GUARD CRITICO: En `src/router/index.ts` agrega `beforeEach` usando `sessionStorage.getItem('auth_token')`.
4. SIDEBAR ESTANDAR: Debe seguir el patrón definido por las referencias locales de skills del repositorio.
5. REUTILIZACION: Si existen `AppSidebar`, `DefaultLayout`, `GlobalSearch`, debes reutilizarlos y ajustar, no reinventar.
6. UI DE AUTH: Para Login usa solo `FormInput`, `AppButton`, `AppBreadcrumb`.
</critical_rules>

<minimal_reads>
Leer solo estos archivos antes de generar:
1. `.github/skills/vue-standard-components-ref/references/AppSidebar_PRODUCTION_REFERENCE.vue`
2. `.github/skills/vue-standard-components-ref/references/DefaultLayout_PRODUCTION_REFERENCE.vue`
3. `src/layout/DefaultLayout.vue` (si existe)
4. `src/layout/components/AppSidebar.vue` (si existe)
5. `src/components/global-search/GlobalSearch.vue` o `src/components/shared/search/GlobalSearch.vue` (si existe)
</minimal_reads>

<sidebar_contract>
El sidebar debe incluir obligatoriamente:
- rail mode (colapsado/expandido)
- trigger de búsqueda global (`open-search`)
- navegación principal con estado activo
- bloque inferior de usuario + cerrar sesión
- overlay mobile
</sidebar_contract>

<step_sequence>

## PASO 1 — API & Entorno
Descargar Swagger (si es URL) y configurar `.env` + `api.config.ts`.

## PASO 2 — Tipos y Validadores
Generar `src/types/auth.types.ts` y `src/validators/auth.validator.ts`.

## PASO 3 — Auth Infrastructure
Crear `http-client.ts`, `auth.service.ts` y `auth.store.ts` (Pinia). La persistencia es obligatoria.

## PASO 4 — AppSidebar & Navigation
Implementar/ajustar `AppSidebar` con el contrato de sidebar estándar.

## PASO 5 — DefaultLayout & UI Navigation
- Implementar/ajustar `DefaultLayout.vue` integrando el `AppHeader` (Topbar azul fijo) y `AppSidebar`.
- Asegurar que `AppHeader` gestione el toggle de modo oscuro y el trigger de búsqueda global.
- Integrar `GlobalSearch` con filtrado dinámico y navegación por teclado.

## PASO 6 — Routing & Vistas Finales
- Crear `LoginView.vue` y `DashboardView.vue`.
- Configurar guard:
  ```typescript
  router.beforeEach((to, from, next) => {
    const isAuthenticated = !!sessionStorage.getItem('auth_token')
    if (to.name !== 'login' && !isAuthenticated) next({ name: 'login' })
    else next()
  })
  ```
</step_sequence>

<final_verification>
Validar que al abrir la app sin token redirija automáticamente a `/auth/login`.
</final_verification>
