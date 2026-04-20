<template>
  <button
    :type="type"
    :disabled="disabled || isLoading"
    :class="[
      'inline-flex items-center justify-center rounded-lg px-4 py-2 text-sm font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2',
      variantClass,
      (disabled || isLoading) ? 'opacity-70 cursor-not-allowed' : '',
      fullWidth ? 'w-full' : '',
    ]"
    @click="onClick"
  >
    <span v-if="isLoading">Procesando...</span>
    <span v-else>
      <slot />
    </span>
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  variant?: 'primary' | 'secondary' | 'danger'
  type?: 'button' | 'submit' | 'reset'
  isLoading?: boolean
  disabled?: boolean
  fullWidth?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  type: 'button',
  isLoading: false,
  disabled: false,
  fullWidth: false,
})

const emit = defineEmits<{
  click: [event: MouseEvent]
}>()

const variantClass = computed(() => {
  if (props.variant === 'secondary') {
    return 'bg-slate-100 text-slate-700 hover:bg-slate-200 focus:ring-slate-300 dark:bg-slate-700 dark:text-slate-100 dark:hover:bg-slate-600'
  }

  if (props.variant === 'danger') {
    return 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-400'
  }

  return 'bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-400'
})

const onClick = (event: MouseEvent) => {
  if (props.isLoading || props.disabled) {
    event.preventDefault()
    return
  }
  emit('click', event)
}
</script>
