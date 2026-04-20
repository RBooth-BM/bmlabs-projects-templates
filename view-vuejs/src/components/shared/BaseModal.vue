<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition-opacity duration-200"
      leave-active-class="transition-opacity duration-200"
      enter-from-class="opacity-0"
      leave-to-class="opacity-0"
    >
      <div
        v-if="show"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4"
        @click="onBackdropClick"
      >
        <div
          class="w-full overflow-hidden rounded-xl bg-white shadow-xl dark:bg-slate-900"
          :class="maxWidthClass"
          @click.stop
        >
          <div class="flex items-center justify-between border-b border-slate-200 px-4 py-3 dark:border-slate-700">
            <h2 class="text-base font-semibold text-slate-800 dark:text-slate-100">{{ title }}</h2>
            <button
              v-if="!hideCloseButton"
              type="button"
              class="rounded p-1 text-slate-500 hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-800"
              aria-label="Cerrar modal"
              @click="emit('close')"
            >
              ✕
            </button>
          </div>

          <div class="max-h-[75vh] overflow-y-auto p-4">
            <slot />
          </div>

          <div
            v-if="!hideFooter"
            class="flex justify-end gap-2 border-t border-slate-200 px-4 py-3 dark:border-slate-700"
          >
            <button
              type="button"
              class="rounded-lg bg-slate-100 px-3 py-2 text-sm text-slate-700 hover:bg-slate-200 dark:bg-slate-700 dark:text-slate-100"
              @click="emit('close')"
            >
              {{ cancelText }}
            </button>
            <button
              type="button"
              class="rounded-lg bg-blue-600 px-3 py-2 text-sm text-white hover:bg-blue-700"
              :disabled="loading"
              @click="emit('confirm')"
            >
              {{ loading ? 'Procesando...' : confirmText }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  show: boolean
  title?: string
  maxWidth?: 'sm' | 'md' | 'lg' | 'xl'
  confirmText?: string
  cancelText?: string
  hideFooter?: boolean
  loading?: boolean
  closeOnBackdropClick?: boolean
  hideCloseButton?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  title: '',
  maxWidth: 'lg',
  confirmText: 'Confirmar',
  cancelText: 'Cancelar',
  hideFooter: false,
  loading: false,
  closeOnBackdropClick: true,
  hideCloseButton: false,
})

const emit = defineEmits<{
  close: []
  confirm: []
}>()

const maxWidthClass = computed(() => {
  if (props.maxWidth === 'sm') return 'max-w-md'
  if (props.maxWidth === 'md') return 'max-w-lg'
  if (props.maxWidth === 'xl') return 'max-w-4xl'
  return 'max-w-2xl'
})

const onBackdropClick = () => {
  if (props.closeOnBackdropClick) {
    emit('close')
  }
}
</script>
