<script setup lang="ts">
import { computed } from 'vue'
import { ChevronLeft, ChevronRight } from 'lucide-vue-next'

interface Props {
  totalRecords: number
  recordsPerPage?: number
  currentPage?: number
  showRecordsPerPage?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  recordsPerPage: 10,
  currentPage: 1,
  showRecordsPerPage: true,
})

const emit = defineEmits<{
  'update:currentPage': [page: number]
  'update:recordsPerPage': [pageSize: number]
}>()

const totalPages = computed(() => Math.ceil(props.totalRecords / props.recordsPerPage))

const canGoPrevious = computed(() => props.currentPage > 1)

const canGoNext = computed(() => props.currentPage < totalPages.value)

const goToPrevious = (): void => {
  if (canGoPrevious.value) {
    emit('update:currentPage', props.currentPage - 1)
  }
}

const goToNext = (): void => {
  if (canGoNext.value) {
    emit('update:currentPage', props.currentPage + 1)
  }
}

const handlePageSizeChange = (event: Event): void => {
  const newSize = parseInt((event.target as HTMLSelectElement).value, 10)
  emit('update:recordsPerPage', newSize)
}

const startRecord = computed(() => (props.currentPage - 1) * props.recordsPerPage + 1)

const endRecord = computed(() => Math.min(props.currentPage * props.recordsPerPage, props.totalRecords))
</script>

<template>
  <div class="flex flex-col sm:flex-row items-center justify-between gap-4 px-4 py-3 bg-gray-50 dark:bg-gray-800/50 rounded-lg border border-gray-200 dark:border-gray-700">
    <!-- Info de registros -->
    <div class="text-sm text-gray-600 dark:text-gray-400">
      Mostrando <span class="font-semibold">{{ startRecord }}</span> a
      <span class="font-semibold">{{ endRecord }}</span> de
      <span class="font-semibold">{{ totalRecords }}</span> registros
    </div>

    <!-- Controles de paginación -->
    <div class="flex items-center gap-2">
      <!-- Selector de registros por página -->
      <div v-if="showRecordsPerPage" class="flex items-center gap-2">
        <label for="records-per-page" class="text-xs font-semibold text-gray-600 dark:text-gray-400">
          Mostrar:
        </label>
        <select
          id="records-per-page"
          :value="recordsPerPage"
          class="px-2 py-1 text-xs border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:border-blue-500"
          @change="handlePageSizeChange"
        >
          <option value="5">5</option>
          <option value="10">10</option>
          <option value="20">20</option>
          <option value="50">50</option>
          <option value="100">100</option>
        </select>
      </div>

      <!-- Botones de navegación -->
      <div class="flex items-center gap-1">
        <button
          :disabled="!canGoPrevious"
          class="p-1.5 rounded-md border border-gray-300 dark:border-gray-600 hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition"
          :aria-label="`Ir a página ${currentPage - 1}`"
          @click="goToPrevious()"
        >
          <ChevronLeft class="w-4 h-4 text-gray-600 dark:text-gray-400" />
        </button>

        <!-- Indicador de página -->
        <div class="px-3 py-1 text-xs font-semibold text-gray-600 dark:text-gray-400">
          Página <span>{{ currentPage }}</span> de <span>{{ totalPages }}</span>
        </div>

        <button
          :disabled="!canGoNext"
          class="p-1.5 rounded-md border border-gray-300 dark:border-gray-600 hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition"
          :aria-label="`Ir a página ${currentPage + 1}`"
          @click="goToNext()"
        >
          <ChevronRight class="w-4 h-4 text-gray-600 dark:text-gray-400" />
        </button>
      </div>
    </div>
  </div>
</template>
