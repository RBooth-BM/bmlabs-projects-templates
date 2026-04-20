<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { X } from 'lucide-vue-next'

interface Props {
  id?: string
  modelValue: string | null
  isActive?: boolean
  type?: 'text' | 'date' | 'select'
  options?: Array<{ id: string; name: string }>
  valueKey?: string
  labelKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: null,
  isActive: false,
  type: 'text',
})

const emit = defineEmits<{
  'update:modelValue': [value: string | null]
  'update:isActive': [value: boolean]
  search: []
}>()

const showDropdown = ref(false)

const filterValue = computed<string>({
  get: () => props.modelValue ?? '',
  set: (value) => {
    emit('update:modelValue', value === '' ? null : value)
  },
})

watch(
  () => props.isActive,
  (newVal) => {
    showDropdown.value = newVal
  },
)

const handleSearch = (): void => {
  emit('search')
}

const handleClear = (): void => {
  emit('update:modelValue', null)
  emit('update:isActive', false)
  showDropdown.value = false
}

const handleClose = (): void => {
  emit('update:isActive', false)
  showDropdown.value = false
}

const handleClickOutside = (event: MouseEvent): void => {
  const target = event.target as HTMLElement
  if (!target.closest('[data-filter-container]')) {
    handleClose()
  }
}
</script>

<template>
  <div data-filter-container class="relative" @click.stop>
    <!-- Botón de filtro -->
    <button
      :id="id"
      class="p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
      :class="{ 'text-blue-600 dark:text-blue-400': modelValue }"
      @click="emit('update:isActive', !isActive)"
    >
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="w-4 h-4">
        <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"></polygon>
      </svg>
    </button>

    <!-- Dropdown -->
    <transition name="fade">
      <div
        v-if="showDropdown"
        class="absolute top-full right-0 mt-2 w-48 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 z-10 p-3"
        @click.stop
      >
        <!-- Input de búsqueda -->
        <input
          v-if="type === 'text' || type === 'date'"
          v-model="filterValue"
          :type="type"
          :placeholder="`Filtrar por ${type}...`"
          class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm focus:outline-none focus:border-blue-500"
          @input="handleSearch()"
        />

        <!-- Select para opciones -->
        <select
          v-else-if="type === 'select' && options"
          v-model="filterValue"
          class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm focus:outline-none focus:border-blue-500"
          @change="handleSearch()"
        >
          <option value="">Seleccionar...</option>
          <option v-for="option in options" :key="option[valueKey || 'id']" :value="option[valueKey || 'id']">
            {{ option[labelKey || 'name'] }}
          </option>
        </select>

        <!-- Botones de acción -->
        <div class="mt-3 flex gap-2 justify-end">
          <button
            v-if="modelValue"
            class="px-2 py-1 text-xs rounded bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 transition"
            @click="handleClear()"
          >
            Limpiar
          </button>
          <button
            class="px-2 py-1 text-xs rounded bg-blue-500 hover:bg-blue-600 text-white transition"
            @click="handleClose()"
          >
            Cerrar
          </button>
        </div>
      </div>
    </transition>

    <!-- Overlay -->
    <transition name="fade">
      <div
        v-if="showDropdown"
        class="fixed inset-0 z-9"
        @click="handleClickOutside"
      />
    </transition>
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
