<template>
  <label class="flex cursor-pointer items-center justify-between rounded-lg border border-slate-300 px-3 py-2 dark:border-slate-700">
    <span class="text-sm font-medium text-slate-700 dark:text-slate-200">{{ label }}</span>
    <button
      type="button"
      role="switch"
      :aria-checked="model"
      :class="[
        'relative inline-flex h-6 w-11 items-center rounded-full transition-colors',
        model ? 'bg-blue-600' : 'bg-slate-300 dark:bg-slate-600',
      ]"
      @click="model = !model"
    >
      <span
        :class="[
          'inline-block h-4 w-4 transform rounded-full bg-white transition-transform',
          model ? 'translate-x-6' : 'translate-x-1',
        ]"
      />
    </button>
  </label>
</template>

<script setup lang="ts">
interface Props {
  label?: string
}

withDefaults(defineProps<Props>(), {
  label: 'Activo',
})

const model = defineModel<boolean>({ default: false })
</script>
