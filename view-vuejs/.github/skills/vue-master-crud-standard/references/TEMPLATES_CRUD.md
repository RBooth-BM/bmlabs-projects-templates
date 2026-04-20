# Templates CRUD - Patrón 12/3 (BM)

> Código fuente del proyecto base `test-clean-agentic-app`.
> DEBES replicar esta estructura EXACTA para toda vista administrativa.

---

## Estructura del Template (3 Secciones)

```vue
<template>
  <!-- 1. SECCIÓN ACCIONES -->
  <div class="w-full p-4 md:p-6 bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-100 dark:border-gray-700 mb-4">
    <div class="flex flex-col md:flex-row md:items-end gap-4">
      <div class="w-full md:w-auto">
        <span class="block text-md font-medium text-black dark:text-white mb-2">Acciones</span>
        <div class="flex flex-col sm:flex-row items-stretch sm:items-center gap-3 sm:gap-4">
          <div class="flex-1 flex justify-end items-end gap-2">
            <AppButton
              id="btn-nuevo-registro"
              variant="primary"
              :icon="Plus"
              :disabled="loading"
              @click="nuevoRegistro()"
            >
              Nuevo Registro
            </AppButton>
            <AppButton
              id="btn-importar-registros"
              variant="secondary"
              :icon="Upload"
              :disabled="loading"
              @click="importarRegistros()"
            >
              Importar
            </AppButton>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- 2. SECCIÓN DATOS -->
  <div class="w-full p-4 md:p-6 bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-100 dark:border-gray-700 mb-10">
    <div class="overflow-x-auto mb-10 border border-gray-200 dark:border-gray-700 md:rounded-lg" style="padding-bottom: 60px">
      <table class="w-full divide-y divide-gray-200 dark:divide-gray-700" style="border-collapse: collapse">
        <thead class="bg-gray-50 dark:bg-gray-900">
          <tr>
            <th scope="col" class="py-3 px-3 text-xs sm:text-sm font-bold text-center text-gray-500 dark:text-gray-400 whitespace-nowrap">
              <div class="flex items-center justify-center gap-1">
                <span>Nombre</span>
                <Filter
                  v-model="filtros.nombre"
                  :is-active="activeFilter === 'nombre'"
                  id="nombre-filter"
                  @update:is-active="setActiveFilter('nombre', $event)"
                  @search="buscar()"
                />
              </div>
            </th>
            <th scope="col" class="py-3 px-3 text-xs sm:text-sm font-bold text-center text-gray-500 dark:text-gray-400 whitespace-nowrap">
              <div class="flex items-center justify-center gap-1">
                <span>Estado</span>
                <Filter
                  v-model="filtros.estado"
                  :is-active="activeFilter === 'estado'"
                  :options="estadoOptions"
                  :value-key="'id'"
                  :label-key="'nombre'"
                  id="estado-filter"
                  @update:is-active="setActiveFilter('estado', $event)"
                  @search="buscar()"
                />
              </div>
            </th>
            <th scope="col" class="py-3 px-3 text-xs sm:text-sm font-bold text-center text-gray-500 dark:text-gray-400 whitespace-nowrap">
              Acciones
            </th>
          </tr>
        </thead>
        <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
          <!-- Loading -->
          <tr v-if="loading">
            <td :colspan="3" class="py-8 text-center text-gray-500 dark:text-gray-400">
              <div class="flex flex-col items-center">
                <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-[var(--color-primary)] mb-2" />
                <span>Cargando registros...</span>
              </div>
            </td>
          </tr>
          <!-- Empty -->
          <tr v-else-if="registros.length === 0">
            <td :colspan="3" class="py-8 text-center text-gray-500 dark:text-gray-400">
              No se encontraron registros
            </td>
          </tr>
          <!-- Data -->
          <tr v-else v-for="registro in registros" :key="registro.id" class="hover:bg-gray-50 dark:hover:bg-gray-700/50">
            <td class="py-3 px-3 text-sm text-center text-gray-900 dark:text-gray-200">{{ registro.nombre }}</td>
            <td class="py-3 px-3 text-sm text-center text-gray-900 dark:text-gray-200">{{ registro.estado }}</td>
            <td class="py-3 px-3 text-sm text-center">
              <div class="flex justify-center gap-1">
                <button
                  :id="`btn-edit-${registro.id}`"
                  class="p-1.5 text-[var(--color-primary)] hover:bg-blue-50 dark:hover:bg-blue-900/30 rounded cursor-pointer"
                  title="Editar"
                  @click="editarRegistro(registro)"
                >
                  <Pencil :size="16" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <Paginator
      v-if="registros.length > 0"
      :total-records="totalRegistros"
      :records-per-page="recordsPerPage"
      :show-records-per-page="true"
      :current-page="currentPage"
      @update:currentPage="currentPage = $event"
      @update:recordsPerPage="updateRecordsPerPage"
      class="mt-4"
    />
  </div>

  <!-- 3. SECCIÓN MODALES -->
  <CreateEditRecordModal
    :show="showCreateEditModal"
    :operacion="operacion"
    :registro="registroSeleccionado"
    @close="closeCreateEditModal()"
    @refresh="handleRefresh()"
  />
</template>
```

---

## Script Setup - 12 Pasos Obligatorios

```typescript
<script setup lang="ts">
// ─── 1. IMPORTS ─────────────────────────────────────────────
import { ref, onMounted, watch } from 'vue'
import { Plus, Pencil, Upload } from 'lucide-vue-next'
import AppButton from '@/components/shared/AppButton.vue'
import Paginator from '@/components/shared/Paginator.vue'
import Filter from '@/components/shared/Filter.vue'
import CreateEditRecordModal from '../components/CreateEditRecordModal.vue'
import { RegistroService } from '@/services/api/services/registros.service'
import type { Registro } from '@/types'

// ─── 2. ESTADO PRINCIPAL ────────────────────────────────────
const registros = ref<Registro[]>([])
const totalRegistros = ref<number>(0)
const loading = ref<boolean>(false)

// ─── 3. ESTADO UI Y MODALES ────────────────────────────────
const showCreateEditModal = ref<boolean>(false)
const registroSeleccionado = ref<Partial<Registro>>({})
const operacion = ref<'crear' | 'editar'>('crear')

// ─── 4. PAGINACIÓN ─────────────────────────────────────────
const currentPage = ref<number>(1)
const recordsPerPage = ref<number>(10)

// ─── 5. FILTROS ─────────────────────────────────────────────
const activeFilter = ref<string | null>(null)
const filtros = ref<{
  nombre: string | null
  estado: string | null
}>({
  nombre: null,
  estado: null,
})

// ─── 6. OPCIONES PARA SELECTS ──────────────────────────────
const estadoOptions = ref<{ id: string; nombre: string }[]>([])

// ─── 7. COMPUTED PROPERTIES ────────────────────────────────
// (Agregar computados si hay transformaciones de datos)

// ─── 8. FUNCIONES DE UTILIDAD ──────────────────────────────
const setActiveFilter = (filterKey: string, isActive: boolean): void => {
  activeFilter.value = isActive ? filterKey : null
}

const updateRecordsPerPage = (newRpp: number): void => {
  recordsPerPage.value = newRpp
  currentPage.value = 1
}

// ─── 9. FUNCIONES DE API ────────────────────────────────────
const fetchRecords = async (): Promise<void> => {
  try {
    loading.value = true
    const response = await RegistroService.getPaged(
      currentPage.value,
      recordsPerPage.value,
      filtros.value.nombre ?? undefined,
      filtros.value.estado ?? undefined,
    )
    registros.value = response.items
    totalRegistros.value = response.totalCount
  } catch (error) {
    console.error('Error cargando registros:', error)
  } finally {
    loading.value = false
  }
}

// ─── 10. ACCIONES DE USUARIO ────────────────────────────────
const buscar = async (): Promise<void> => {
  currentPage.value = 1
  await fetchRecords()
}

const nuevoRegistro = (): void => {
  operacion.value = 'crear'
  registroSeleccionado.value = {}
  showCreateEditModal.value = true
}

const editarRegistro = (registro: Registro): void => {
  operacion.value = 'editar'
  registroSeleccionado.value = registro
  showCreateEditModal.value = true
}

const importarRegistros = (): void => {
  // Implementar lógica de importación
}

const closeCreateEditModal = (): void => {
  showCreateEditModal.value = false
}

const handleRefresh = async (): Promise<void> => {
  await fetchRecords()
}

// ─── 11. WATCHERS ───────────────────────────────────────────
watch(currentPage, async () => { await fetchRecords() })
watch(recordsPerPage, async () => { currentPage.value = 1; await fetchRecords() })

// ─── 12. LIFECYCLE ──────────────────────────────────────────
onMounted(async () => { await fetchRecords() })
</script>
```

---

## Checklist

- [ ] ¿Template tiene exactamente 3 secciones comentadas?
- [ ] ¿Script setup sigue los 12 pasos en orden?
- [ ] ¿Usa `AppButton` en vez de `<button>` nativo para acciones principales?
- [ ] ¿Todas las variables están tipadas (`ref<Tipo>(...)`)?
- [ ] ¿Se usa `<script setup lang="ts">`?
- [ ] ¿Soporte dark mode en todas las clases?
- [ ] ¿IDs kebab-case para testing?

---

## Template: CreateEditModal (Child Component)

> Ubicar en `src/views/[module]/components/CreateEdit[Entity]Modal.vue`
> Usa `AppModal` como modal base (NO `BaseModal` para nuevos desarrollos).

```vue
<!-- src/views/records/components/CreateEditRecordModal.vue -->
<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import AppModal from '@/components/shared/AppModal.vue'
import FormInput from '@/components/shared/FormInput.vue'
import FormSelect from '@/components/shared/FormSelect.vue'
import { RegistroService, type RegistroDto } from '@/services/api/services/registros.service'
import { registroSchema, type RegistroForm } from '@/validators/registro.validator'
import type { Registro } from '@/types'

const props = defineProps<{
  show: boolean
  operacion: 'crear' | 'editar'
  registro: Partial<Registro>
}>()

const emit = defineEmits<{
  close: []
  refresh: []
}>()

const saving = ref(false)

const form = reactive<RegistroForm>({
  nombre: '',
  estado: '',
})

const errors = reactive<Partial<Record<keyof RegistroForm, string>>>({})

const resetForm = () => {
  form.nombre = props.registro.nombre ?? ''
  form.estado = props.registro.estado ?? ''
  errors.nombre = undefined
  errors.estado = undefined
}

const validate = (): boolean => {
  const result = registroSchema.safeParse(form)
  errors.nombre = undefined
  errors.estado = undefined
  if (!result.success) {
    for (const issue of result.error.issues) {
      const field = issue.path[0] as keyof RegistroForm
      if (!errors[field]) errors[field] = issue.message
    }
    return false
  }
  return true
}

const guardar = async () => {
  if (!validate()) return
  saving.value = true
  try {
    const dto: RegistroDto = {
      id: props.operacion === 'editar' ? props.registro.id : undefined,
      nombre: form.nombre,
      estado: form.estado,
    }
    await RegistroService.createOrUpdate(dto)
    emit('refresh')
    emit('close')
  } catch (error) {
    console.error('Error guardando registro:', error)
  } finally {
    saving.value = false
  }
}

watch(() => props.show, (newVal) => {
  if (newVal) resetForm()
})
</script>

<template>
  <AppModal
    :show="show"
    :title="operacion === 'crear' ? 'Nuevo Registro' : 'Editar Registro'"
    max-width="lg"
    confirm-text="Guardar"
    :loading="saving"
    @confirm="guardar()"
    @close="emit('close')"
  >
    <div class="space-y-4 p-1">
      <FormInput
        id="modal-nombre"
        v-model="form.nombre"
        label="Nombre"
        required
        :error="!!errors.nombre"
        :error-message="errors.nombre"
      />
      <FormSelect
        id="modal-estado"
        v-model="form.estado"
        label="Estado"
        required
        :options="[{ id: 'activo', nombre: 'Activo' }, { id: 'inactivo', nombre: 'Inactivo' }]"
        value-key="id"
        label-key="nombre"
        :error="!!errors.estado"
        :error-message="errors.estado"
      />
    </div>
  </AppModal>
</template>
```

