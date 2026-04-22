---
name: vue-create-protected-routes
description: Configura guardias de navegación en Vue Router para securizar el acceso a vistas administrativas basado en el estado de autenticación.
license: Proprietary
compatibility: Vue Router 4, Vue 3, Pinia
metadata:
author: Ricardo B.T.
version: 1.2.0
---

# Skill: Protected Routes & Navigation Guards

## Propósito
Implementar un sistema de seguridad en el lado del cliente que impida el acceso no autorizado a rutas privadas y gestione automáticamente la redirección de usuarios entre el flujo de login y el área administrativa.

## Invocación
```bash
/K # Configura o actualiza la seguridad del router
```

## Instrucciones / Estándares Aplicados

Si el usuario requiere securizar rutas o implementar guardias de navegación, entonces DEBES seguir este estándar técnico:

### 1. Configuración de Meta Data
- Todas las rutas que requieran autenticación deben incluir el atributo `meta: { requiresAuth: true }`.
- Las rutas públicas opcionales pueden incluir `meta: { public: true }`.

### 2. Inyección de Auth Store
- Inicializar `useAuthStore()` DENTRO de `router.beforeEach` (no a nivel de módulo).
- Preferir `authStore.checkAuthIfNeeded()` para validar sesión sin llamadas redundantes.
- Evitar lectura de sesión hardcodeada en múltiples archivos; usar constantes compartidas cuando existan.

### 3. Lógica de Redirección Inteligente
- **Privacidad**: Si `requiresAuth` es true y no hay sesión activa → Redirigir a `{ name: 'login', query: { redirect: to.fullPath } }`.
- **Pre-Login**: Si el usuario intenta acceder a la ruta de login pero ya tiene sesión → Redirigir a `{ name: 'dashboard' }`.
- **Ruta de login**: `/auth/login` (nombre: `'login'`). NO usar `/login`.
- Prevenir bucles de redirección: si `to.name === 'login'` y no hay sesión, permitir navegación.

### 3.1 Patrón de Guard Moderno (Vue Router 4)
- Usar retorno (`return '/ruta'` o `return { name: 'x' }`) en guards, evitando patrón legado con `next()`.
- Si el guard hace llamadas async, usar `await` explícito antes de decidir redirección.
- Para cambios de parámetros en la misma ruta, usar `watch(() => route.params...)` u `onBeforeRouteUpdate` en el componente.

### 4. Lazy Loading
- Asegura que todas las rutas protegidas utilicen importaciones dinámicas `import('@/views/...')` para optimizar el bundle.

### 5. Rutas Base y Constantes
- Preferir constantes compartidas para rutas (`APP_ROUTES`) y claves de sesión (`AUTH_STORAGE_KEYS`) cuando el proyecto ya las tenga.
- Mantener las rutas públicas útiles (`/home`) fuera de autenticación si el template base las expone para inspección o bootstrap.
- Si el template base desactiva temporalmente el guard para inspección visual, no usar esa referencia para generar código productivo; la generación debe seguir el patrón seguro de esta skill.

## Checklist de Calidad

- [ ] ¿Se definió `meta: { requiresAuth: true }` en las rutas privadas?
- [ ] ¿Se usa `useAuthStore` para la validación de sesión?
- [ ] ¿Se inicializa `useAuthStore()` dentro de `beforeEach`?
- [ ] ¿Se redirige al login ante falta de credenciales?
- [ ] ¿Se previene el "doble login" redirigiendo al dashboard?
- [ ] ¿Se evita el uso de `next()` en guards nuevos?
- [ ] ¿Se previenen bucles de redirección?
- [ ] ¿Se utiliza importación dinámica para las vistas?
- [ ] ¿El archivo reside en `@/router/index.ts`?

## Que Genera
La configuración completa de un `router.beforeEach` robusto que actúa como firewall de entrada a la aplicación administrativa.

## Referencias y Código Reutilizable
Para generar el código con el máximo nivel de detalle, DEBES leer los ejemplos reales, validaciones Zod y arquitectura de BM ubicados en la referencia técnica:
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/REAL_ROUTES.md)

Fuentes productivas consideradas para esta referencia consolidada:
- `airexp-backoffice-app`
- `ssa-epp-backoffice-app`
- `polpaico-muestreo-app`
