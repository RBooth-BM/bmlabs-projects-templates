# Referencia Real: DataTable (Patrón BM)

> Componente en `src/components/shared/datatable/DataTable.vue`.
> Tipos en `src/components/shared/datatable/data-table.types.ts`.
> DEBES usar este componente para TODA tabla de datos en las vistas administrativas.

---

## 1. Tipos (importar desde `@/components/shared/datatable/data-table.types`)

```typescript
import type { TableColumn, TableAction, DataTableRow } from '@/components/shared/datatable/data-table.types'

// TableColumn — define columnas visibles
interface TableColumn {
  key: string                 // campo del objeto de datos
  label: string               // texto del header
  hidden?: string             // clase responsive ('md:table-cell', 'lg:table-cell')
  sortable?: boolean          // habilita ordenamiento client-side
  filterable?: boolean        // muestra ícono de filtro en el header
  filterType?: 'text' | 'date' | 'dateRange' | 'select' | 'multiselect' | 'number'
  filterOptions?: { value: unknown; label: string }[]
  price?: boolean             // formatea como moneda ($1.000)
  type?: 'text' | 'select' | 'custom' | 'date' | 'rejectedAlert'
  badge?: boolean             // renderiza con badge de color
  badgeColor?: string
  options?: { value: unknown; label: string }[]  // para type='select'
  render?: Component          // componente Vue personalizado para la celda
  sticky?: boolean
  minWidth?: string
  maxWidth?: string
  clickable?: boolean
  textColor?: string          // clase tailwind de color ('text-blue-600')
}

// TableAction — define acciones de fila
interface TableAction {
  key: string                 // identificador del emit
  title?: string              // tooltip
  iconComponent?: Component   // ícono de lucide-vue-next
  iconClass?: string          // clase de color ('text-blue-500', 'text-red-500')
  class?: string
  show?: (row: unknown) => boolean  // visibilidad condicional
}
```

---

## 2. Props del componente DataTable

| Prop | Tipo | Default | Descripción |
|---|---|---|---|
| `data` | `DataTableRow[]` | requerido | Datos a mostrar |
| `columns` | `TableColumn[]` | requerido | Definición de columnas |
| `actions` | `TableAction[]` | `[]` | Acciones de fila |
| `totalRecords` | `number` | requerido | Total para paginación |
| `recordsPerPage` | `number` | requerido | Registros por página |
| `currentPage` | `number` | `1` | Página actual |
| `clientSideOperations` | `boolean` | `false` | Paginación/filtros locales |
| `showCheckbox` | `boolean` | `false` | Selección múltiple |
| `showSelectAllCheckbox` | `boolean` | `false` | Checkbox "seleccionar todos" |
| `compact` | `boolean` | `false` | Modo compacto |
| `headerColor` | `string` | CSS var primary | Color del header |
| `selectedRows` | `DataTableRow[]` | `[]` | Filas seleccionadas (v-model) |
| `showPagination` | `boolean` | `true` | Mostrar paginador |
| `getRowClass` | `(row) => string` | — | Clase CSS dinámica por fila |

**Emits**: `action-clicked: [key: string, row: DataTableRow]`, `update:currentPage`, `update:recordsPerPage`, `update:selectedRows`, `update:filters`, `column-clicked: [key: string, row: DataTableRow]`

---

## 3. Ejemplo completo en una vista CRUD

```vue
<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { Pencil, Trash2 } from 'lucide-vue-next'
import DataTable from '@/components/shared/datatable/DataTable.vue'
import AppButton from '@/components/shared/AppButton.vue'
import ConfirmModal from '@/components/shared/ConfirmModal.vue'
import CreateEditRecordModal from '../components/CreateEditRecordModal.vue'
import { RegistroService } from '@/services/api/services/registros.service'
import type { TableColumn, TableAction } from '@/components/shared/datatable/data-table.types'
import type { Registro } from '@/types'

// ─── 2. ESTADO PRINCIPAL ────────────────────────────────────
const registros = ref<Registro[]>([])
const totalRegistros = ref(0)
const loading = ref(false)

// ─── 3. ESTADO UI ───────────────────────────────────────────
const showCreateEdit = ref(false)
const showConfirmDelete = ref(false)
const registroSeleccionado = ref<Partial<Registro>>({})
const operacion = ref<'crear' | 'editar'>('crear')
const deleting = ref(false)

// ─── 4. PAGINACIÓN ─────────────────────────────────────────
const currentPage = ref(1)
const recordsPerPage = ref(10)

// ─── DEFINICIÓN DE TABLA ────────────────────────────────────
const columns: TableColumn[] = [
  { key: 'nombre', label: 'Nombre', sortable: true, filterable: true },
  { key: 'estado', label: 'Estado', filterable: true, filterType: 'select',
    filterOptions: [
      { value: 'activo', label: 'Activo' },
      { value: 'inactivo', label: 'Inactivo' },
    ],
    badge: true,
  },
  { key: 'createdAt', label: 'Creado', type: 'date', hidden: 'md:table-cell' },
]

const actions: TableAction[] = [
  { key: 'edit', title: 'Editar', iconComponent: Pencil, iconClass: 'text-blue-500 hover:text-blue-700' },
  { key: 'delete', title: 'Eliminar', iconComponent: Trash2, iconClass: 'text-red-500 hover:text-red-700' },
]

// ─── 9. API ─────────────────────────────────────────────────
const fetchRecords = async () => {
  loading.value = true
  try {
    const res = await RegistroService.getPaged(currentPage.value, recordsPerPage.value)
    registros.value = res.items
    totalRegistros.value = res.totalCount
  } finally {
    loading.value = false
  }
}

// ─── 10. ACCIONES ───────────────────────────────────────────
const handleAction = (key: string, row: unknown) => {
  const registro = row as Registro
  if (key === 'edit') {
    operacion.value = 'editar'
    registroSeleccionado.value = registro
    showCreateEdit.value = true
  }
  if (key === 'delete') {
    registroSeleccionado.value = registro
    showConfirmDelete.value = true
  }
}

const confirmarEliminar = async () => {
  deleting.value = true
  try {
    await RegistroService.delete(registroSeleccionado.value.id!)
    await fetchRecords()
  } finally {
    deleting.value = false
    showConfirmDelete.value = false
  }
}

// ─── 11. WATCHERS ───────────────────────────────────────────
watch([currentPage, recordsPerPage], () => fetchRecords())

// ─── 12. LIFECYCLE ──────────────────────────────────────────
onMounted(() => fetchRecords())
</script>

<template>
  <!-- SECCIÓN ACCIONES -->
  <div class="w-full p-4 md:p-6 bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-100 dark:border-gray-700 mb-4">
    <div class="flex justify-end">
      <AppButton id="btn-nuevo" variant="primary" :icon="Plus" :disabled="loading" @click="nuevoRegistro()">
        Nuevo
      </AppButton>
    </div>
  </div>

  <!-- SECCIÓN DATOS -->
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

  <!-- SECCIÓN MODALES -->
  <CreateEditRecordModal
    :show="showCreateEdit"
    :operacion="operacion"
    :registro="registroSeleccionado"
    @close="showCreateEdit = false"
    @refresh="fetchRecords()"
  />

  <ConfirmModal
    v-model="showConfirmDelete"
    title="Eliminar registro"
    message="¿Estás seguro? Esta acción no se puede deshacer."
    variant="danger"
    confirm-text="Sí, eliminar"
    :loading="deleting"
    @confirm="confirmarEliminar()"
    @cancel="showConfirmDelete = false"
  />
</template>
```

---

## 4. Cuándo usar DataTable vs tabla HTML manual

| Situación | Usar |
|---|---|
| Lista principal de entidad CRUD | `DataTable` |
| Tabla con filtros, sorting, selección múltiple | `DataTable` |
| Tabla simple de 2-3 columnas en un modal | Tabla HTML manual con Tailwind |
| Tabla de resumen/totales dentro de un formulario | Tabla HTML manual |
