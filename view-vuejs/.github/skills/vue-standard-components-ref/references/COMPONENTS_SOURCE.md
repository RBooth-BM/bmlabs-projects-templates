# Referencia Real: Componentes Compartidos (BM)

> Componentes en `src/components/shared/`. SIEMPRE usar estos en lugar de HTML nativo.

---

## 1. AppButton.vue — Botón Universal

**Props**: `variant` (`primary|secondary|ghost|danger|link|unstyled`), `size` (`sm|md|lg|icon`), `type`, `icon`, `isLoading`, `disabled`, `fullWidth`, `contentClass`

**Emits**: `click: [event: MouseEvent]`

**Slots**: `default` (texto), `loading` (texto alternativo al cargar, default "Procesando...")

```vue
<AppButton variant="primary" :icon="Plus" @click="nuevo()">Nuevo</AppButton>
<AppButton variant="secondary" :icon="Upload" :disabled="loading">Importar</AppButton>
<AppButton variant="danger" :icon="Trash2" @click="eliminar()">Eliminar</AppButton>
<AppButton type="submit" :full-width="true" :is-loading="isLoading">Guardar</AppButton>
```

---

## 2. FormInput.vue — Input de Formularios

**Model**: `defineModel<string | number>()`

**Props clave**: `id` (requerido), `label`, `type` (`text|password|email|rut|phone|number|amount|date`), `error`, `errorMessage`, `helpText`, `required`, `disabled`, `readonly`, `placeholder`, `icon`, `prefix`, `suffix`, `suffixIcon`, `suffixClickable`, `showPasswordToggle`, `autocomplete`, `restrictInput` (`integer|maintainer-alphanum-code|vehicle-plate-alnum`)

```vue
<FormInput id="nombre" v-model="form.nombre" label="Nombre" required
  :error="!!errors.nombre" :error-message="errors.nombre" />
<FormInput id="password" v-model="form.password" type="password"
  label="Contraseña" :show-password-toggle="true" />
```

---

## 3. FormSelect.vue — Select de Formularios

**Model**: `defineModel<string | number | undefined>()`

**Props clave**: `label`, `options`, `placeholder`, `disabled`, `required`, `error`, `errorMessage`, `valueKey`, `labelKey`, `icon`

```vue
<FormSelect id="perfil" v-model="form.perfilId" label="Perfil"
  :options="perfilOptions" value-key="id" label-key="nombre"
  :error="!!errors.perfilId" :error-message="errors.perfilId" />
```

---

## 4. FormTextarea.vue — Textarea de Formularios

**Model**: `defineModel<string | undefined>()`

**Props clave**: `label`, `rows` (default 3), `placeholder`, `disabled`, `required`, `error`, `errorMessage`, `maxlength`, `resize` (`none|vertical|horizontal|both`)

```vue
<FormTextarea id="descripcion" v-model="form.descripcion" label="Descripción"
  :rows="4" :maxlength="500" :error="!!errors.descripcion" :error-message="errors.descripcion" />
```

---

## 5. AppModal.vue — Modal Moderno (Principal)

**Model**: `v-model` o `show` (boolean)

**Props**: `title`, `maxWidth` (`sm|md|lg|xl|2xl|full`), `hideHeader`, `hideFooter`, `hideCloseButton`, `closeOnBackdrop`, `confirmText`, `cancelText`, `loading`

**Emits**: `update:modelValue`, `update:show`, `close`, `confirm`, `cancel`

**Implementación**: `Teleport to="#modals"` + animaciones + `backdrop-blur-sm` + diseño `rounded-3xl`

**Slots**: `header` (personalizar cabecera), `default` (body), `footer` (personalizar pie)

```vue
<AppModal v-model="showModal" title="Crear Usuario" max-width="lg"
  confirm-text="Guardar" :loading="saving" @confirm="guardar()" @close="showModal = false">
  <!-- formulario aquí -->
</AppModal>
```

---

## 6. BaseModal.vue — Modal Legacy

**Props**: `show`, `title`, `maxWidth` (`sm|md|lg|xl|2xl|3xl|4xl|5xl|6xl|7xl`), `maxHeight`, `confirmText`, `cancelText`, `hideFooter`, `loading`, `closeOnBackdropClick`, `hideCloseButton`

**Emits**: `close`, `confirm`

> **NOTA**: `AppModal` es el modal moderno. `BaseModal` sigue disponible como legacy. Usar `AppModal` para nuevos desarrollos.

---

## 7. ConfirmModal.vue — Diálogos de Confirmación

**Props**: `modelValue` o `show`, `title`, `message`, `variant` (`info|success|warning|danger|question`), `confirmText`, `cancelText`, `loading`, `maxWidth`

**Emits**: `update:modelValue`, `update:show`, `confirm`, `cancel`

**Implementación**: Usa `AppModal` internamente con `hide-header`. Renderiza ícono + título + mensaje + botones.

```vue
<ConfirmModal
  v-model="showConfirm"
  title="Eliminar registro"
  message="¿Estás seguro? Esta acción no se puede deshacer."
  variant="danger"
  confirm-text="Sí, eliminar"
  :loading="deleting"
  @confirm="eliminar()"
  @cancel="showConfirm = false"
/>
```

---

## 8. Filter.vue — Filtro en Tablas

**Props**: `modelValue: string | null | undefined`, `isActive: boolean`, `id: string`, `placeholder?: string`

**Emits**: `update:modelValue`, `update:isActive`, `search`

**Uso en tabla** (dentro de `<th>`):
```vue
<Filter
  v-model="filtros.nombre"
  :is-active="activeFilter === 'nombre'"
  id="nombre-filter"
  @update:is-active="setActiveFilter('nombre', $event)"
  @search="buscar()"
/>
```

**Función auxiliar requerida**:
```typescript
const setActiveFilter = (filterKey: string, isActive: boolean): void => {
  activeFilter.value = isActive ? filterKey : null
}
```

---

## 9. Paginator.vue — Paginación

**Props**: `totalRecords`, `recordsPerPage?` (default 10), `currentPage?` (default 1), `showRecordsPerPage?` (default true)

**Emits**: `update:currentPage`, `update:recordsPerPage`

```vue
<Paginator
  v-if="registros.length > 0"
  :total-records="totalRegistros"
  :records-per-page="recordsPerPage"
  :current-page="currentPage"
  :show-records-per-page="true"
  @update:currentPage="currentPage = $event"
  @update:recordsPerPage="updateRecordsPerPage"
  class="mt-4"
/>
```

---

## 10. AppBreadcrumb.vue — Navegación Automática

Sin props. Calcula el path automáticamente desde la URL actual y el router.

**Obligatorio**: al inicio de CADA vista en `src/views/`.

```vue
<AppBreadcrumb />
```

---

## 11. FormActions.vue — Acciones de Formulario (Guardar/Volver)

**Props**: `backRoute`, `saveButtonText` (default `'Guardar'`), `showBackButton` (default true), `fullWidth` (default true), `disabled`

**Emits**: `save`

**Slot**: `default` (contenido del formulario)

```vue
<FormActions back-route="/usuarios" @save="guardar()">
  <FormInput ... />
  <FormSelect ... />
</FormActions>
```

---

## 12. PageHeader.vue — Encabezado de Vista

**Props**: `title: string`, `subtitle?: string`

**Slot**: `#extra` para acciones adicionales a la derecha.

```vue
<PageHeader title="Usuarios" subtitle="Gestión de usuarios del sistema">
  <template #extra>
    <AppButton variant="primary" :icon="Plus" @click="nuevo()">Nuevo</AppButton>
  </template>
</PageHeader>
```

---

## 13. AppSwitch.vue — Toggle Activo/Inactivo

**Model**: `v-model` (boolean)

**Props**: `label`

```vue
<AppSwitch v-model="form.isActive" label="Activo" />
```

---

## 14. DataTable.vue — Tabla Avanzada con Filtros y Paginación

**Ubicación**: `@/components/shared/datatable/DataTable.vue`

**Props principales**:

| Prop | Tipo | Descripción |
|---|---|---|
| `data` | `DataTableRow[]` | Datos a mostrar |
| `columns` | `TableColumn[]` | Definición de columnas |
| `actions` | `TableAction[]` | Acciones de fila (iconos) |
| `totalRecords` | `number` | Total para paginación server-side |
| `recordsPerPage` | `number` | Registros por página |
| `currentPage` | `number` | Página actual |
| `clientSideOperations` | `boolean` | `true` si la paginación/filtros son locales |
| `showCheckbox` | `boolean` | Habilita selección múltiple |
| `compact` | `boolean` | Modo compacto (menos padding) |
| `headerColor` | `string` | Color CSS del header de la tabla |

**Emits**: `action-clicked: [key: string, row: DataTableRow]`, `update:filters`, `update:currentPage`, `update:recordsPerPage`, `update:selectedRows`

**Tipos clave** (importar desde `@/components/shared/datatable/data-table.types.ts`):

```typescript
interface TableColumn {
  key: string
  label: string
  sortable?: boolean
  filterable?: boolean
  filterType?: 'text' | 'date' | 'dateRange' | 'select' | 'multiselect' | 'number'
  filterOptions?: { value: unknown; label: string }[]
  hidden?: string         // clase responsive (ej: 'md:table-cell')
  sticky?: boolean
  minWidth?: string
}

interface TableAction {
  key: string             // identificador del evento
  title?: string          // tooltip
  iconComponent?: Component
  iconClass?: string
  class?: string
}
```

**Ejemplo de uso**:

```vue
<script setup lang="ts">
import DataTable from '@/components/shared/datatable/DataTable.vue'
import type { TableColumn, TableAction } from '@/components/shared/datatable/data-table.types.ts'
import { Pencil, Trash2 } from 'lucide-vue-next'

const columns: TableColumn[] = [
  { key: 'nombre', label: 'Nombre', sortable: true, filterable: true },
  { key: 'estado', label: 'Estado', filterable: true, filterType: 'select',
    filterOptions: [{ value: 'activo', label: 'Activo' }, { value: 'inactivo', label: 'Inactivo' }] },
]

const actions: TableAction[] = [
  { key: 'edit', title: 'Editar', iconComponent: Pencil, iconClass: 'text-blue-500' },
  { key: 'delete', title: 'Eliminar', iconComponent: Trash2, iconClass: 'text-red-500' },
]

const handleAction = (key: string, row: unknown) => {
  if (key === 'edit') editarRegistro(row)
  if (key === 'delete') confirmarEliminacion(row)
}
</script>

<template>
  <DataTable
    :data="registros"
    :columns="columns"
    :actions="actions"
    :total-records="totalRegistros"
    :records-per-page="recordsPerPage"
    :current-page="currentPage"
    @action-clicked="handleAction"
    @update:currentPage="currentPage = $event"
    @update:recordsPerPage="recordsPerPage = $event"
  />
</template>
```

---

## 15. AppExportActions.vue — Exportación a Excel

**Ubicación**: `@/components/shared/datatable/AppExportActions.vue`

**Props**: `data: any[]`, `filename?: string`, `sheetName?: string`, `columns?: { key: string; label: string }[]`

```vue
<AppExportActions
  :data="registros"
  filename="reporte_registros"
  sheet-name="Registros"
  :columns="[{ key: 'nombre', label: 'Nombre' }, { key: 'estado', label: 'Estado' }]"
/>
```

---

## Reglas de Importación

```typescript
import AppButton from '@/components/shared/AppButton.vue'
import AppModal from '@/components/shared/AppModal.vue'
import BaseModal from '@/components/shared/BaseModal.vue'
import ConfirmModal from '@/components/shared/ConfirmModal.vue'
import FormInput from '@/components/shared/FormInput.vue'
import FormSelect from '@/components/shared/FormSelect.vue'
import FormTextarea from '@/components/shared/FormTextarea.vue'
import FormActions from '@/components/shared/FormActions.vue'
import Filter from '@/components/shared/Filter.vue'
import Paginator from '@/components/shared/Paginator.vue'
import AppBreadcrumb from '@/components/shared/AppBreadcrumb.vue'
import PageHeader from '@/components/shared/PageHeader.vue'
import AppSwitch from '@/components/shared/AppSwitch.vue'
// DataTable stack (para listados avanzados):
import DataTable from '@/components/shared/datatable/DataTable.vue'
import AppExportActions from '@/components/shared/datatable/AppExportActions.vue'
import type { TableColumn, TableAction } from '@/components/shared/datatable/data-table.types'
```

## ShowcaseView — Referencia Visual Canónica

`src/views/ShowcaseView.vue` → accesible en `/showcase`. **REVISAR SIEMPRE** para verificar estilos y componentes antes de implementaciones de alta fidelidad.

