<template>
  <div class="space-y-1">
    <label v-if="label" :for="id" class="block text-sm font-medium text-gray-700">
      {{ label }}
    </label>

    <div class="relative">
      <input
        :id="id"
        v-model="model"
        :type="inputType"
        :placeholder="placeholder"
        :autocomplete="autocomplete"
        :disabled="disabled"
        :readonly="readonly"
        :class="[
          'w-full rounded-lg border px-3 py-2.5 text-sm outline-none transition-colors',
          error ? 'border-red-500 focus:border-red-500' : 'border-gray-300 focus:border-blue-500',
          disabled ? 'bg-gray-100 cursor-not-allowed' : 'bg-white',
        ]"
        @blur="emit('blur')"
      />

      <button
        v-if="type === 'password' && showPasswordToggle"
        type="button"
        class="absolute inset-y-0 right-2 my-auto h-8 px-2 text-xs text-gray-500"
        @click="showPassword = !showPassword"
      >
        {{ showPassword ? 'Ocultar' : 'Mostrar' }}
      </button>
    </div>

    <p v-if="error && errorMessage" class="text-xs text-red-600">{{ errorMessage }}</p>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'

interface Props {
  id: string
  label?: string
  type?: 'text' | 'password' | 'email'
  placeholder?: string
  disabled?: boolean
  readonly?: boolean
  error?: boolean
  errorMessage?: string
  autocomplete?: string
  showPasswordToggle?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  type: 'text',
  disabled: false,
  readonly: false,
  error: false,
  showPasswordToggle: false,
})

const emit = defineEmits<{
  blur: []
}>()

const model = defineModel<string>()
const showPassword = ref(false)

const inputType = computed(() => {
  if (props.type !== 'password') return props.type
  return showPassword.value ? 'text' : 'password'
})
</script>
