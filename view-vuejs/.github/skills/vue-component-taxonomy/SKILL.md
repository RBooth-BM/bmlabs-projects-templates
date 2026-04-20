---
name: vue-component-taxonomy
description: Clasificación y nomenclatura unificada de componentes por categoría, asegurando uniformidad en generaciones repetidas.
license: Proprietary
compatibility: Vue 3, Tailwind CSS, Lucide Icons
metadata:
  author: BM
  version: 1.0.0
---

# Skill: Taxonomía de Componentes Vue 3

## Propósito
Definir una clasificación jerárquica clara de componentes reutilizables para que el generador SIEMPRE produce el mismo tipo de componente para la misma función, independientemente de cuántas veces se invoque.

## Jerarquía de Componentes

```
App Components (Nivel Aplicación)
├── Layout Components
│   ├── DefaultLayout.vue
│   ├── AppHeader.vue
│   ├── AppSidebar.vue
│   └── PageHeader.vue
│
├── Page/View Components (Nivel Página)
│   ├── [Entity]View.vue (patrón 12/3)
│   ├── LoginView.vue
│   ├── DashboardView.vue
│   └── ...
│
├── Feature Components (Nivel Funcionalidad)
│   ├── Modales
│   │   ├── BaseModal.vue (contenedor)
│   │   ├── ConfirmModal.vue (confirmación)
│   │   ├── CreateEdit[Entity]Modal.vue (CRUD)
│   │   └── [Feature]Modal.vue
│   │
│   ├── Tablas & Listas
│   │   ├── DataTable.vue (tabla avanzada)
│   │   ├── Filter.vue (filtro de columna)
│   │   └── Paginator.vue (navegación)
│   │
│   ├── Formularios
│   │   ├── FormInput.vue
│   │   ├── FormSelect.vue
│   │   ├── FormTextarea.vue
│   │   └── FormActions.vue (botones submit/cancel)
│   │
│   ├── Notificaciones
│   │   ├── ToastNotification.vue
│   │   ├── AppAlert.vue
│   │   └── Swal (SweetAlert2 helper)
│   │
│   └── Navegación
│       ├── AppBreadcrumb.vue
│       ├── TabNavigation.vue
│       └── AppSidebarNav.vue
│
└── Atomic Components (Nivel Atómico)
    ├── Botones
    │   ├── AppButton.vue (universal)
    │   ├── IconActions.vue (botones de acción compactos)
    │   └── CopyToClipboard.vue
    │
    ├── Inputs
    │   ├── AppInput.vue (nativo, uso interno)
    │   ├── AppSwitch.vue (toggle booleano)
    │   └── [Type]Input (especializados)
    │
    ├── Visualización
    │   ├── AppStatsCard.vue (KPI)
    │   ├── AppChartCard.vue (gráficos)
    │   ├── TotalsSummaryCard.vue (resumen)
    │   └── RequiredFieldsLegend.vue (leyenda)
    │
    └── Utilidad
        ├── AppPageHeader.vue
        ├── ListHeader.vue
        └── CopyToClipboard.vue
```

---

## 📋 Catálogo Estándar (Carpeta: `src/components/shared/`)

### Categoría: Forms (Formularios)

| Componente | Archivo | Uso | Props Clave | Evento Clave |
|---|---|---|---|---|
| **FormInput** | `FormInput.vue` | Campo de entrada unificado | `:type`, `:label`, `:placeholder`, `:error-message` | `@blur`, `@input` |
| **FormSelect** | `FormSelect.vue` | Selector de opciones | `:options`, `:label`, `:multiple` (si aplica) | `@change` |
| **FormTextarea** | `FormTextarea.vue` | Texto multilinea | `:rows`, `:label`, `:max-length` | `@blur`, `@input` |
| **FormActions** | `FormActions.vue` | Botones de acción (Guardar/Cancelar) | `:loading-save`, `:disabled` | `@save`, `@cancel` |

### Categoría: Buttons (Botones)

| Componente | Archivo | Uso | Props Clave | Variantes |
|---|---|---|---|---|
| **AppButton** | `AppButton.vue` | Botón universal | `:variant` (primary/secondary/danger), `:size` (sm/md/lg), `:is-loading`, `:full-width` | primary, secondary, danger, ghost |
| **IconActions** | `IconActions.vue` | Botones de acción compactos (Edit, Delete) | `:actions`, `:entity-id` | edit, delete, view, export |

### Categoría: Modales (Diálogos)

| Componente | Archivo | Uso | Props Clave | Evento Clave |
|---|---|---|---|---|
| **BaseModal** | `BaseModal.vue` | Contenedor modal simple | `:is-open`, `:title` | `@close` |
| **AppModal** | `AppModal.vue` | Modal moderno (alternativa mejorada) | `:is-open`, `:title`, `:size` (sm/md/lg) | `@close` |
| **ConfirmModal** | `ConfirmModal.vue` | Modal de confirmación | `:is-open`, `:title`, `:message`, `:confirm-text`, `:cancel-text` | `@confirm`, `@cancel` |

### Categoría: Tables (Tablas)

| Componente | Archivo | Uso | Props Clave | Evento Clave |
|---|---|---|---|---|
| **DataTable** | `datatable/DataTable.vue` | Tabla avanzada con paginación, filtros y acciones | `:columns`, `:data`, `:loading`, `:pagination` | `@filter`, `@sort`, `@page-change` |
| **Filter** | `Filter.vue` | Filtro individual en header de tabla | `:column-name`, `:column-type` | `@filter` |
| **Paginator** | `Paginator.vue` | Control de paginación | `v-model:currentPage`, `v-model:recordsPerPage`, `:total-records` | `@page-change` |

### Categoría: Layout (Estructura)

| Componente | Archivo | Uso | Props Clave | Ubicación |
|---|---|---|---|---|
| **AppSidebar** | `layout/AppSidebar.vue` | Barra lateral principal | `:is-open`, `:navigation-items` | layout/ |
| **AppHeader** | `layout/AppHeader.vue` | Encabezado superior | `:user-name`, `:is-sidebar-open` | layout/ |
| **PageHeader** | `PageHeader.vue` | Encabezado de página/vista | `:title`, `:subtitle`, `:breadcrumb` | shared/ |
| **AppBreadcrumb** | `AppBreadcrumb.vue` | Migas de navegación | `:items` | shared/ |

### Categoría: Notificaciones

| Componente | Archivo | Uso | Método Clave | Ubicación |
|---|---|---|---|---|
| **ToastNotification** | `ToastNotification.vue` | Notificación emergente | `show(message, type)` | shared/ |
| **SweetAlert2 Helper** | (importar directamente) | Confirmaciones y alertas | `Swal.fire({...})` | composables/ |

### Categoría: Cards & Stats

| Componente | Archivo | Uso | Props Clave | Contexto |
|---|---|---|---|---|
| **AppStatsCard** | `AppStatsCard.vue` | KPI / Métrica | `:label`, `:value`, `:icon`, `:trend` | Dashboard |
| **AppChartCard** | `AppChartCard.vue` | Gráfico envuelto | `:title`, `:chart-data` | Dashboard |
| **TotalsSummaryCard** | `TotalsSummaryCard.vue` | Resumen de totales | `:items` (array de {label, value}) | CRUD |

### Categoría: Especializados

| Componente | Archivo | Uso | Props Clave |
|---|---|---|---|
| **AppSwitch** | `AppSwitch.vue` | Toggle booleano (on/off) | `v-model`, `:label` |
| **CopyToClipboard** | `CopyToClipboard.vue` | Botón copiar al portapapeles | `:text` |
| **DocumentUploadSection** | `DocumentUploadSection.vue` | Carga de archivos | `:accept-types`, `:max-size` |
| **RequiredFieldsLegend** | `RequiredFieldsLegend.vue` | Leyenda de campos requeridos | (sin props, solo slot) |

---

## 🎯 Reglas de Nomenclatura

### Componentes Reutilizables (`src/components/shared/`)

**Formato**: `App[FeatureName].vue` o `[FeatureName].vue`

✅ CORRECTO:
- `AppButton.vue`
- `AppInput.vue`
- `FormInput.vue`
- `FormSelect.vue`
- `DataTable.vue`
- `Filter.vue`
- `Paginator.vue`
- `PageHeader.vue`

❌ EVITAR:
- `Button.vue` (muy genérico)
- `input-field.vue` (kebab-case en componentes)
- `MyCustomButton.vue` (sin prefijo estándar)
- `table-component.vue` (genérico)

### Modales Específicos de Características (`src/views/[entity]/components/`)

**Formato**: `CreateEdit[Entity]Modal.vue` o `[Feature][Entity]Modal.vue`

✅ CORRECTO:
- `CreateEditUserModal.vue`
- `CreateEditProductModal.vue`
- `ImportUsersModal.vue`
- `ConfirmDeleteModal.vue`

❌ EVITAR:
- `UserModal.vue` (ambiguo)
- `user-edit-modal.vue` (kebab-case)
- `CreateModal.vue` (demasiado genérico)

### Vistas / Pages (`src/views/`)

**Formato**: `[Entity]View.vue` o `[Feature]View.vue`

✅ CORRECTO:
- `UsersView.vue`
- `ProductsView.vue`
- `LoginView.vue`
- `DashboardView.vue`

❌ EVITAR:
- `Users.vue` (sin sufijo)
- `user-list.vue` (kebab-case)
- `UserPage.vue` (inconsistente, usar View)

---

## 🔄 Ciclo de Uso: Garantizar Uniformidad

### Escenario: Generar Formulario de Usuario 3 veces

**Primera invocación** (`/H Usuario`):
```vue
<!-- UserCreateModal.vue -->
<template>
  <BaseModal :is-open="isOpen" @close="closeModal">
    <FormInput v-model="form.name" label="Nombre" type="text" />
    <FormInput v-model="form.email" label="Email" type="email" />
    <FormSelect v-model="form.role" label="Rol" :options="roleOptions" />
    <FormActions @save="handleSubmit" @cancel="closeModal" />
  </BaseModal>
</template>
```

**Segunda invocación** (mismo usuario solicita cambios):
```vue
<!-- Misma estructura, mismos componentes, misma nomenclatura -->
```

**Tercera invocación** (otro dev genera el mismo formulario):
```vue
<!-- Idéntico a las anteriores -->
```

✅ **ÉXITO**: Las 3 invocaciones producen la misma estructura, componentes y nomenclatura.

---

## 🚫 Violaciones de Taxonomía

❌ Mezclar `BaseModal` con modal nativo `<dialog>`  
❌ Usar `<input>` nativo en lugar de `FormInput`  
❌ Componente `MyCustomTable` en lugar de extender `DataTable`  
❌ Nomenclatura inconsistente: `UserForm.vue` vs `CreateUserModal.vue`  
❌ Props sin contrato: cada invocación define props diferentes  

---

## ✅ Checklist de Consistencia

Cuando generes un componente, verifica:

- [ ] ¿Usa un componente de la Taxonomía estándar?
- [ ] ¿La nomenclatura sigue el patrón correcto?
- [ ] ¿El componente reside en la carpeta correcta?
- [ ] ¿Las props están documentadas y coinciden con otras invocaciones?
- [ ] ¿Los eventos tienen nombres consistentes (@close, @save, @cancel)?
- [ ] ¿Puede un dev generar el mismo componente 3 veces sin variaciones?

---

## 📚 Referencias
- Componentes reales: `src/components/shared/`
- Modales de entidad: `src/views/[entity]/components/`
- Vistas CRUD: `src/views/[entity]/[Entity]View.vue`
- Skill: `vue-master-crud-standard` (patrón 12/3)
