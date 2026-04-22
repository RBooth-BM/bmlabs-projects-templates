---
name: vue-project-naming-structure
description: Garantiza la consistencia en la ubicación y nomenclatura de archivos según el estándar de BM.
license: Proprietary
compatibility: Vue 3 Standard Projects
metadata:
  author: Ricardo B.T.
  version: 2.1.0
---

# Skill: Project Naming & Structure (Estándar BM)

## Propósito
Mantener una base de código organizada y predecible donde cada archivo tiene una ubicación y nombre estandarizado.

## Invocación
Esta skill se activa automáticamente cuando se solicita crear un nuevo archivo o estructura.

## Instrucciones / Estándares Aplicados

### 0. Regla de Idioma en Nomenclatura
- Variables, funciones, componentes, tipos, stores, composables, archivos y carpetas deben nombrarse en ingles.
- No mezclar idiomas en identificadores; evita ejemplos copiables que combinen ingles y espanol.
- Espanol se permite solo en comentarios, labels UI y mensajes de validacion.

### 1. Estructura de Carpetas (src/)

| Carpeta | Contenido | Ejemplo |
|---|---|---|
| `src/assets/logo/` | Logos e imágenes de marca | `bm-labs-logo.png`, `logo-placeholder.svg` |
| `src/services/api/http-client.ts` | Cliente Axios centralizado | `http-client.ts` |
| `src/services/api/services/` | Servicios de API (clase estática) | `users.service.ts` |
| `src/types/` | Interfaces y tipos | `api.types.ts`, `models/user.types.ts` |
| `src/views/` | Vistas raíz para páginas | `views/auth/LoginView.vue` |
| `src/views/[module]/[entity]/` | Vistas de entidad con subcomponentes | `views/admin/users/UsersListView.vue` |
| `src/views/[module]/[entity]/components/` | Componentes específicos de una vista | `CreateEditUserModal.vue` |
| `src/components/shared/` | Componentes reutilizables globales | `AppButton.vue`, `BaseModal.vue` |
| `src/composables/` | Lógica reactiva reutilizable | `useUsers.ts`, `usePagination.ts` |
| `src/stores/` | Stores de Pinia | `auth.store.ts`, `ui.store.ts` |
| `src/layout/` | Layouts principales | `DefaultLayout.vue` |
| `src/layout/components/` | Componentes del layout | `AppSidebar.vue`, `UserCard.vue` |
| `src/config/` | Configuración de la app | `api.config.ts` |
| `src/constants/` | Constantes | `sort-direction.constants.ts` |
| `src/validators/` | Schemas Zod | `user.validator.ts` |
| `src/router/` | Configuración de rutas | `index.ts`, `guards/auth.guard.ts` |

### 2. Convenciones de Nomenclatura

| Tipo | Convención | Ejemplo |
|---|---|---|
| Componentes Vue | PascalCase | `AppButton.vue`, `BaseModal.vue` |
| Vistas Vue | PascalCase + sufijo `View` | `UsersListView.vue`, `LoginView.vue` |
| Modales Vue | PascalCase + sufijo `Modal` | `CreateEditUserModal.vue` |
| Servicios API | camelCase + `.service.ts` | `users.service.ts` |
| Stores Pinia | camelCase + `.store.ts` | `auth.store.ts` |
| Composables | camelCase con `use` | `useUsers.ts` |
| Tipos | camelCase + `.types.ts` | `api.types.ts` |
| Validators | camelCase + `.validator.ts` | `user.validator.ts` |
| IDs HTML | kebab-case | `id="btn-save-user"` |
| Clases CSS | kebab-case | `.section-container` |

### 3. Sufijos Obligatorios

| Sufijo | Uso |
|---|---|
| `View` | Componentes raíz en `views/` |
| `Modal` | Componentes que se renderizan en diálogos |
| `.service.ts` | Capa de servicios API |
| `.store.ts` | Stores de Pinia |
| `.types.ts` | Interfaces TypeScript |
| `.validator.ts` | Schemas Zod |

## Checklist de Calidad

- [ ] ¿El archivo está en la carpeta `src/` correcta?
- [ ] ¿Usa PascalCase para componentes y camelCase para lógica?
- [ ] ¿Se incluyó el sufijo obligatorio?
- [ ] ¿Los IDs en el template usan kebab-case?

## Que Genera
Una estructura de archivos coherente que facilita la navegación del proyecto.
