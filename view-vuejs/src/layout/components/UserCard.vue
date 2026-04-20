<template>
  <div ref="cardRef" class="relative w-full min-w-[140px]" @click.stop>
    <button
      type="button"
      class="w-full flex items-center p-2 text-sm text-gray-600 transition-colors duration-200 transform dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 dark:hover:text-white rounded"
      @click="isOpen = !isOpen"
    >
      <div
        class="w-10 h-10 rounded-full flex items-center justify-center text-white font-semibold text-lg relative"
      >
        <div class="rounded absolute inset-0"></div>
        <div
          style="background-color: var(--sidebar-bg)"
          class="relative z-10 w-8 h-8 rounded-full text-[var(--sidebar-text-color)] dark:bg-blue-600 flex items-center justify-center font-semibold text-sm"
        >
          {{ getUserInitials(userName) }}
        </div>
      </div>
      <div class="mx-1 flex-1 text-left min-w-0 overflow-hidden hidden sm:block">
        <span
          class="text-sm font-semibold truncate text-gray-700 dark:text-gray-200"
          :title="userName"
        >
          {{ userName || '—' }}
        </span>
      </div>
    </button>

    <Teleport to="body">
      <div
        v-if="isOpen"
        ref="dropdownRef"
        class="fixed z-[100] w-full min-w-max p-2 overflow-hidden bg-white rounded-md shadow-xl dark:bg-gray-800 border border-gray-100 dark:border-gray-700"
        :style="dropdownStyle"
        @click.stop
      >
        <button
          type="button"
          class="flex items-center w-full px-4 py-3 text-sm text-red-600 dark:text-red-400 capitalize rounded-lg cursor-pointer transition-colors duration-200 hover:bg-red-50 dark:hover:bg-red-900/20"
          @click="handleLogout"
        >
          <LogOut class="w-4 h-4 mr-3 shrink-0" />
          <span>Cerrar Sesión</span>
        </button>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick, watch, onUnmounted } from 'vue'
import { LogOut } from 'lucide-vue-next'

interface Props {
  userName?: string | null
}

const props = withDefaults(defineProps<Props>(), { userName: '' })
const emit = defineEmits<{ logout: [] }>()

const isOpen = ref(false)
const cardRef = ref<HTMLElement | null>(null)
const dropdownRef = ref<HTMLElement | null>(null)
const dropdownStyle = ref<Record<string, string>>({})

const getUserInitials = (name?: string | null): string => {
  if (!name) return '?'
  const words = name.trim().split(/\s+/).filter(Boolean)
  if (words.length === 0) return '?'
  const [first, second] = words
  if (!second) return first.charAt(0).toUpperCase()
  return `${first.charAt(0)}${second.charAt(0)}`.toUpperCase()
}

const updateDropdownPosition = () => {
  if (!cardRef.value || !isOpen.value) {
    dropdownStyle.value = {}
    return
  }
  const rect = cardRef.value.getBoundingClientRect()
  dropdownStyle.value = {
    top: `${rect.bottom + 4}px`,
    right: `${window.innerWidth - rect.right}px`,
    width: `${Math.max(rect.width, 200)}px`,
  }
}

const handleClickOutside = (event: MouseEvent) => {
  if (
    isOpen.value &&
    cardRef.value &&
    dropdownRef.value &&
    !cardRef.value.contains(event.target as Node) &&
    !dropdownRef.value.contains(event.target as Node)
  ) {
    isOpen.value = false
  }
}

watch(isOpen, async (val) => {
  if (val) {
    await nextTick()
    updateDropdownPosition()
    window.addEventListener('resize', updateDropdownPosition)
    window.addEventListener('scroll', updateDropdownPosition, true)
    document.addEventListener('click', handleClickOutside)
  } else {
    dropdownStyle.value = {}
    window.removeEventListener('resize', updateDropdownPosition)
    window.removeEventListener('scroll', updateDropdownPosition, true)
    document.removeEventListener('click', handleClickOutside)
  }
})

onUnmounted(() => {
  window.removeEventListener('resize', updateDropdownPosition)
  window.removeEventListener('scroll', updateDropdownPosition, true)
  document.removeEventListener('click', handleClickOutside)
})

const handleLogout = () => {
  isOpen.value = false
  emit('logout')
}
</script>
