<template>
  <AppSidebar
    :navigation-items="navigationItems"
    :user-name="userName"
    @sidebar-state-change="handleSidebarStateChange"
    @navigate="navigateTo"
    @logout="handleLogout"
    @open-search="openSearch"
  />

  <div
    :class="[
      'content transition-[margin-left] duration-700 ease-out pt-20 px-2 md:px-5 pb-4 bg-gray-50 dark:bg-gray-800 min-h-screen',
      contentOffsetClass,
    ]"
    style="overflow-x: hidden; position: relative"
  >
    <div class="mx-auto w-full" style="max-width: 100%">
      <RouterView :key="route.fullPath" />
    </div>
  </div>

  <button
    type="button"
    class="fixed bottom-5 right-4 z-40 flex h-10 w-10 items-center justify-center rounded-full bg-[var(--color-primary,#2563eb)] text-white shadow-md md:hidden"
    aria-label="Abrir búsqueda"
    @click="openSearch"
  >
    <Search class="h-4 w-4" />
  </button>

  <Teleport to="body">
    <Transition
      enter-active-class="transition-opacity duration-200"
      leave-active-class="transition-opacity duration-200"
      enter-from-class="opacity-0"
      leave-to-class="opacity-0"
    >
      <div
        v-if="searchOpen"
        class="fixed inset-0 z-[90] bg-black/50"
        @click="searchOpen = false"
      >
        <div class="mx-auto mt-24 w-[92%] max-w-2xl rounded-xl bg-white p-4 shadow-xl dark:bg-slate-900" @click.stop>
          <label for="global-search" class="sr-only">Buscar en navegación</label>
          <input
            id="global-search"
            ref="searchInput"
            v-model="searchQuery"
            type="text"
            placeholder="Buscar vistas..."
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-[var(--color-primary,#2563eb)] dark:border-slate-700 dark:bg-slate-800 dark:text-slate-100"
            @keydown.esc="searchOpen = false"
          />

          <div class="mt-3 max-h-72 overflow-auto">
            <button
              v-for="item in filteredSearchItems"
              :key="item.id"
              type="button"
              class="mt-1 flex w-full rounded-md px-3 py-2 text-left text-sm"
              :class="route.path === item.path ? 'bg-blue-50 text-blue-700 dark:bg-blue-900/30 dark:text-blue-200' : 'text-slate-600 hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-800'"
              @click="selectSearchItem(item.path)"
            >
              {{ item.label }}
            </button>

            <p v-if="filteredSearchItems.length === 0" class="px-2 py-5 text-center text-sm text-slate-500">
              Sin resultados
            </p>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { BarChart3, FolderKanban, Search, Settings } from 'lucide-vue-next'
import AppSidebar, { type NavigationItem } from './components/AppSidebar.vue'

const route = useRoute()
const router = useRouter()

const sidebarRailCollapsed = ref(true)
const isDesktop = ref(window.innerWidth >= 768)

const handleSidebarStateChange = (state: { isDesktop: boolean; isOpen: boolean; isDesktopRailCollapsed: boolean }) => {
  isDesktop.value = state.isDesktop
  sidebarRailCollapsed.value = state.isDesktopRailCollapsed
}

const userName = ref('Usuario Demo')

const navigationItems: NavigationItem[] = [
  { id: 'nav-home', label: 'Home', path: '/home', icon: BarChart3 },
  { id: 'nav-dashboard', label: 'Dashboard', path: '/dashboard', icon: BarChart3 },
  {
    id: 'nav-maintainers',
    label: 'Mantenedores',
    icon: FolderKanban,
    children: [
      { id: 'nav-clients', label: 'Clientes', path: '/clientes' },
      { id: 'nav-contacts', label: 'Contactos', path: '/contactos' },
      { id: 'nav-sales', label: 'Ventas', path: '/ventas' },
    ],
  },
  {
    id: 'nav-settings',
    label: 'Configuración',
    icon: Settings,
    children: [
      { id: 'nav-general', label: 'General', path: '/configuracion/general' },
    ],
  },
]

const flatSearchItems = computed(() => {
  return navigationItems.flatMap((item) => {
    if (!item.children?.length) return item.path ? [item] : []
    return item.children.filter((child) => !!child.path)
  })
})

const contentOffsetClass = computed(() => {
  if (!isDesktop.value) return 'ml-0'
  return sidebarRailCollapsed.value ? 'ml-12' : 'ml-60'
})

const navigateTo = async (path: string) => {
  await router.push(path)
}

const handleLogout = () => {
  sessionStorage.removeItem('auth_token')
  localStorage.removeItem('auth_user')
  void router.push('/auth/login')
}

const searchOpen = ref(false)
const searchQuery = ref('')
const searchInput = ref<HTMLInputElement | null>(null)

const filteredSearchItems = computed(() => {
  const q = searchQuery.value.trim().toLowerCase()
  if (!q) return flatSearchItems.value
  return flatSearchItems.value.filter((item) => item.label.toLowerCase().includes(q) || item.path?.toLowerCase().includes(q))
})

const openSearch = async () => {
  searchOpen.value = true
  await nextTick()
  searchInput.value?.focus()
}

const selectSearchItem = async (path?: string) => {
  if (!path) return
  searchOpen.value = false
  await navigateTo(path)
}

const handleShortcut = (event: KeyboardEvent) => {
  if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === 'k') {
    event.preventDefault()
    void openSearch()
  }
}

onMounted(() => {
  window.addEventListener('keydown', handleShortcut)
})

onBeforeUnmount(() => {
  window.removeEventListener('keydown', handleShortcut)
})
</script>
