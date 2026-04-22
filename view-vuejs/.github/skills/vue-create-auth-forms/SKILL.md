---
name: vue-create-auth-forms
description: Genera vistas de autenticación con validación Zod, AppButton, token Bearer en sessionStorage y FormInput corporativo.
license: Proprietary
compatibility: Vue 3, Pinia, Vue Router, Zod
metadata:
  author: Ricardo B.T.
  version: 3.1.0
  standard: BM Auth Standard
---

# Skill: Crear Vistas de Autenticación (Auth Flows)

## Propósito
Implementar interfaces de acceso seguras usando `useAuthStore` (Pinia Setup Store), validación Zod, y componentes compartidos reales.

## Invocación
```bash
/J # Genera LoginView.vue
```

## Instrucciones / Estándares Aplicados

### 0. Fuente Visual Obligatoria (Login)
- La vista de login debe replicar la jerarquía visual corporativa usando la referencia local `references/LOGIN_VISUAL_REFERENCE.vue`.
- No se permite generar un login "genérico" o sin identidad visual (sin logo, sin card, sin contraste de tema).
- Si existe conflicto entre visual y componentes corporativos, se mantiene la visual y se mapean controles a componentes compartidos (`FormInput`, `AppButton`).

### 1. Validación con Zod
- Importar schema desde `@/validators/auth.validator` (NO definir inline en la vista).
- Usar `z.object({...}).safeParse(form)` para validar antes de enviar.
- Mensajes de error en español.
- NO usar VeeValidate. Validación manual con `reactive()`.

### 2. Integración con Auth Store
- `useAuthStore` para acciones de `login` y `logout`.
- Tokens Bearer en `sessionStorage('auth_token')`. El `httpClient` los inyecta automáticamente.
- El store persiste solo la info del `user` en `localStorage('auth_user')`.
- El store expone `setSession` / `clearSession` para gestionar la sesión.
- `isAuthenticated` verifica `!!sessionStorage.getItem('auth_token')`.

### 3. Feedback Visual
- `<AppButton>` con `:is-loading` vinculado al estado de la petición.
- Error global de API como texto rojo (`<p>` o `<span>`) debajo del formulario.
- Errores de campo individuales vía `:error-message` de `FormInput`.

### 4. Componentes de UI
- Usar `FormInput` (NO `<input>` nativo) con props `:error` y `:error-message`.
- `AppBreadcrumb` obligatorio al inicio del template.
- `AppButton` con `type="submit"` y `:full-width="true"`.

### 4.1 Contrato Visual del Login (Obligatorio)
- Contenedor principal centrado con `min-h-screen`, fondo claro/oscuro y padding responsive.
- Card principal (`max-w-md`) con borde, sombra y esquinas redondeadas.
- Bloque de marca superior: logo + título + subtítulo.
- Formulario con separación vertical consistente (`space-y-*`) y estado loading visible.
- Enlace de recuperación de contraseña debajo del botón principal.
- Pie de contexto del sistema en la parte inferior.
- Soporte dark mode en todos los bloques visibles (fondo, card, texto, bordes).

### 5. Navegación Post-Autenticación
- Redirección a `/dashboard` tras login exitoso con `router.push()`.
- Si hay `?redirect=` en la query, usar esa ruta. Ruta de fallback: `/dashboard`.
- Ruta de login: `/auth/login` (NO `/login`).

## Checklist de Calidad

- [ ] ¿Validación con Zod (importado desde `@/validators/`)?
- [ ] ¿Usa `AppButton` con `:is-loading` y `:full-width`?
- [ ] ¿Inyecta `useAuthStore` para la comunicación?
- [ ] ¿Error global y errores de campo individuales?
- [ ] ¿Responsive y dark mode?
- [ ] ¿`<script setup lang="ts">`?
- [ ] ¿Google login opcional?

## Que Genera
- `LoginView.vue` y/o `SignupView.vue` con formularios validados con Zod.

## Referencias y Código Reutilizable
**DEBES leer estas referencias antes de generar código:**
- [Login visual corporativo (referencia)](./references/LOGIN_VISUAL_REFERENCE.vue)
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/REAL_AUTH.md)
