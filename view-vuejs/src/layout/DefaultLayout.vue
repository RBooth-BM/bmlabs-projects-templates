<template>
  <header class="fixed inset-x-0 top-0 z-[80] flex items-center justify-between gap-3 bg-white/90 dark:bg-slate-900/90 border-b border-slate-200/50 dark:border-slate-700/70 px-3 py-3 backdrop-blur md:hidden pointer-events-auto">
    <button
      type="button"
      class="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200/70 bg-slate-100 dark:border-slate-700/70 dark:bg-slate-800 text-slate-700 dark:text-slate-100 transition-all hover:bg-slate-200 dark:hover:bg-slate-700"
      @click="sidebarOpen = !sidebarOpen"
      aria-label="Abrir menú"
    >
      <Menu class="h-5 w-5" />
    </button>

    <div class="flex items-center gap-3 truncate">
      <AppLogo :size="48" />

      <div class="flex flex-col">
        <div class="flex items-center leading-none">
          <span class="text-[12px] font-black tracking-tighter text-slate-900 dark:text-white uppercase">{{ BRAND_CONFIG.name.first }}</span>
          <span class="text-[12px] font-black tracking-tighter text-blue-600 dark:text-blue-400 uppercase ml-1">{{ BRAND_CONFIG.name.last }}</span>
        </div>
        <span class="truncate text-xs font-bold text-slate-500 dark:text-slate-400">{{ route.name || 'Dashboard' }}</span>
      </div>
    </div>

    <button
      type="button"
      class="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200/70 bg-slate-100 dark:border-slate-700/70 dark:bg-slate-800 text-slate-700 dark:text-slate-100 transition-all hover:bg-slate-200 dark:hover:bg-slate-700"
      @click="openSearch"
      aria-label="Abrir búsqueda"
    >
      <Search class="h-4 w-4" />
    </button>
  </header>

  <AppSidebar
    :navigation-items="navigationItems"
    :user-name="userName"
    :is-open="sidebarOpen"
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
        class="fixed inset-0 z-[90] flex items-start justify-center bg-slate-900/60 backdrop-blur-sm pt-[10vh]"
        @click="searchOpen = false"
      >
        <div
          class="w-[92%] max-w-xl overflow-hidden rounded-2xl bg-white shadow-2xl ring-1 ring-black/5 dark:bg-slate-900 dark:ring-white/10"
          @click.stop
        >
          <!-- Input row -->
          <div class="flex items-center gap-3 border-b border-slate-100 px-4 py-3 dark:border-slate-800">
            <Search class="h-4 w-4 shrink-0 text-slate-400 dark:text-slate-500" aria-hidden="true" />
            <label for="global-search" class="sr-only">Buscar en navegación</label>
            <input
              id="global-search"
              ref="searchInput"
              v-model="searchQuery"
              type="text"
              placeholder="Buscar vistas..."
              class="flex-1 bg-transparent text-sm text-slate-900 outline-none placeholder:text-slate-400 dark:text-slate-100 dark:placeholder:text-slate-500"
              @keydown.esc="searchOpen = false"
              @keydown="handleSearchKeydown"
            />
            <kbd class="hidden items-center rounded border border-slate-200 px-1.5 py-0.5 font-mono text-[10px] text-slate-400 sm:inline-flex dark:border-slate-700 dark:text-slate-500">
              ESC
            </kbd>
          </div>

          <!-- Results -->
          <div class="max-h-72 overflow-y-auto py-2">
            <div v-if="filteredSearchItems.length > 0" class="space-y-0.5 px-2">
              <button
                v-for="item in filteredSearchItems"
                :key="item.id"
                type="button"
                class="group flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-left transition-all"
                :class="
                  filteredSearchItems.indexOf(item) === searchActiveIndex
                    ? 'bg-[var(--brand-primary)]/10 text-[var(--brand-primary)]'
                    : route.path === item.path
                      ? 'bg-slate-100 text-slate-900 dark:bg-slate-800 dark:text-slate-100'
                      : 'text-slate-700 hover:bg-slate-50 dark:text-slate-300 dark:hover:bg-slate-800/60'
                "
                @mouseenter="searchActiveIndex = filteredSearchItems.indexOf(item)"
                @click="selectSearchItem(item.path)"
              >
                <div
                  class="flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-slate-100 transition-colors group-hover:bg-[var(--brand-primary)]/10 dark:bg-slate-800"
                >
                  <LayoutDashboard
                    class="h-3.5 w-3.5 text-slate-400 transition-colors group-hover:text-[var(--brand-primary)] dark:text-slate-500"
                  />
                </div>
                <span class="flex-1 truncate text-sm font-medium">{{ item.label }}</span>
                <ChevronRight
                  class="h-3.5 w-3.5 text-slate-300 transition-colors group-hover:text-[var(--brand-primary)] dark:text-slate-600"
                />
              </button>
            </div>

            <div v-else class="px-4 py-10 text-center">
              <p class="text-sm text-slate-500 dark:text-slate-400">
                Sin resultados para <span class="font-semibold">"{{ searchQuery }}"</span>
              </p>
            </div>
          </div>

          <!-- Footer hints -->
          <div class="flex items-center gap-4 border-t border-slate-100 bg-slate-50/80 px-4 py-2 dark:border-slate-800 dark:bg-slate-800/40">
            <span class="text-[10px] text-slate-400 dark:text-slate-500">
              Seleccionar
              <kbd class="mx-1 rounded border border-slate-200 bg-white px-1 py-0.5 font-mono dark:border-slate-700 dark:bg-slate-900">↵</kbd>
            </span>
            <span class="text-[10px] text-slate-400 dark:text-slate-500">
              Cerrar
              <kbd class="mx-1 rounded border border-slate-200 bg-white px-1 py-0.5 font-mono dark:border-slate-700 dark:bg-slate-900">ESC</kbd>
            </span>
            <span class="ml-auto text-[10px] text-slate-400 dark:text-slate-500">
              Abrir con
              <kbd class="mx-1 rounded border border-slate-200 bg-white px-1 py-0.5 font-mono dark:border-slate-700 dark:bg-slate-900">⌘K</kbd>
            </span>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { BarChart3, FolderKanban, Search, Settings, Menu, ChevronRight, LayoutDashboard } from 'lucide-vue-next'
import AppSidebar, { type NavigationItem } from './components/AppSidebar.vue'
import AppLogo from '@/components/common/AppLogo.vue'
import { BRAND_CONFIG } from '@/config/brand.config'
import { APP_ROUTES, AUTH_STORAGE_KEYS } from '@/constants'

const route = useRoute()
const router = useRouter()

const sidebarRailCollapsed = ref(true)
const isDesktop = ref(window.innerWidth >= 768)
const sidebarOpen = ref(false)

const handleSidebarStateChange = (state: { isDesktop: boolean; isOpen: boolean; isDesktopRailCollapsed: boolean }) => {
  isDesktop.value = state.isDesktop
  sidebarRailCollapsed.value = state.isDesktopRailCollapsed
  sidebarOpen.value = state.isOpen
}

const userName = ref('Usuario Demo')

const navigationItems: NavigationItem[] = [
  { id: 'nav-home', label: 'Home', path: APP_ROUTES.HOME, icon: BarChart3 },
  { id: 'nav-dashboard', label: 'Dashboard', path: APP_ROUTES.DASHBOARD, icon: BarChart3 },
  // Agregar nuevas secciones en templates derivados según entidad/proyecto.
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
  // Cerrar sidebar en mobile después de navegar
  if (!isDesktop.value) {
    sidebarOpen.value = false
  }
}

const handleLogout = () => {
  sessionStorage.removeItem(AUTH_STORAGE_KEYS.TOKEN)
  localStorage.removeItem(AUTH_STORAGE_KEYS.USER)
  void router.push(APP_ROUTES.LOGIN)
}

const searchOpen = ref(false)
const searchQuery = ref('')
const searchInput = ref<HTMLInputElement | null>(null)
const searchActiveIndex = ref(0)

const filteredSearchItems = computed(() => {
  const q = searchQuery.value.trim().toLowerCase()
  if (!q) return flatSearchItems.value
  return flatSearchItems.value.filter((item) => item.label.toLowerCase().includes(q) || item.path?.toLowerCase().includes(q))
})

const openSearch = async () => {
  searchOpen.value = true
  searchActiveIndex.value = 0
  await nextTick()
  searchInput.value?.focus()
}

const selectSearchItem = async (path?: string) => {
  if (!path) return
  searchOpen.value = false
  searchQuery.value = ''
  await navigateTo(path)
}

const handleSearchKeydown = (event: KeyboardEvent) => {
  const items = filteredSearchItems.value
  if (!items.length) return

  if (event.key === 'ArrowDown') {
    event.preventDefault()
    searchActiveIndex.value = (searchActiveIndex.value + 1) % items.length
  } else if (event.key === 'ArrowUp') {
    event.preventDefault()
    searchActiveIndex.value = (searchActiveIndex.value - 1 + items.length) % items.length
  } else if (event.key === 'Enter') {
    event.preventDefault()
    const selected = items[searchActiveIndex.value]
    if (selected) void selectSearchItem(selected.path)
  }
}

const handleShortcut = (event: KeyboardEvent) => {
  if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === 'k') {
    event.preventDefault()
    void openSearch()
  }
}

watch(searchQuery, () => {
  searchActiveIndex.value = 0
})

onMounted(() => {
  window.addEventListener('keydown', handleShortcut)
})

onBeforeUnmount(() => {
  window.removeEventListener('keydown', handleShortcut)
})
</script>
