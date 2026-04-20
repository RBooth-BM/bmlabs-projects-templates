<template>
  <!-- HEADER BAR fijo superior -->
  <div
    class="fixed z-40 flex backdrop-blur-md bg-white/70 dark:bg-[#0F172A]/80 items-center h-16 px-4 md:px-8 border-b border-slate-200/50 dark:border-white/5 transition-all duration-500"
    style="width: 100%; max-width: 100vw; left: 0; right: 0; overflow: hidden"
  >
    <!-- Espacio para la sidebar -->
    <div
      class="transition-all duration-500 min-w-0 shrink-0"
      :class="[isDesktop ? (isDesktopRailCollapsed ? 'w-16' : 'w-60') : 'w-0']"
    ></div>

    <div class="flex items-center ml-4">
      <h1 class="text-sm font-black uppercase tracking-widest text-slate-400 dark:text-slate-500">
        {{ route.name || 'Dashboard' }}
      </h1>
    </div>

    <div class="flex-1" />

    <div class="relative group hidden md:block mr-4">
      <button
        type="button"
        class="cursor-pointer flex items-center gap-3 px-4 py-2 rounded-xl text-sm text-slate-500 dark:text-slate-400 bg-slate-100/50 dark:bg-slate-800/40 hover:bg-white dark:hover:bg-slate-800 transition-all border border-slate-200/50 dark:border-white/5 shadow-sm active:scale-95"
        @click="emit('open-search')"
      >
        <Search class="w-4 h-4 text-brand-primary" />
        <span class="text-xs font-semibold opacity-70">Búsqueda rápida...</span>
        <div class="ml-4 flex items-center gap-1 px-1.5 py-0.5 rounded border border-slate-200 dark:border-white/10 bg-white dark:bg-slate-900 shadow-xs text-[10px] font-black opacity-40">
          <span class="text-[12px] leading-none">⌘</span>K
        </div>
      </button>
    </div>

    <div class="flex items-center px-2 py-1 bg-slate-100/50 dark:bg-slate-800/40 rounded-full border border-slate-200/50 dark:border-white/5 shadow-sm">
      <div class="h-8 w-8 rounded-full bg-brand-primary/10 flex items-center justify-center text-[10px] font-black text-brand-primary border border-brand-primary/20">
        {{ userInitials }}
      </div>
    </div>
  </div>

  <!-- ASIDE: Sidebar Azul BM -->
  <aside
    :class="[
      'w-60 fixed h-screen flex flex-col transition-all duration-500 cubic-bezier-sidebar',
      flyoutGroup ? 'z-[58]' : 'z-50',
      isDesktop
        ? isDesktopRailCollapsed
          ? '-translate-x-44'
          : 'translate-x-0'
        : isOpen
          ? 'translate-x-0 shadow-2xl shadow-black/40'
          : '-translate-x-full',
    ]"
    class="sidebar-container"
    @click.stop
  >
    <!-- Logo & Toggle Section -->
    <div class="h-16 flex items-center justify-end px-3 overflow-hidden shrink-0 border-b border-white/10">
      <Transition name="fade">
        <div v-if="!railMode" class="flex-1 flex items-center px-2">
          <span class="text-xl font-black tracking-tighter text-white">BM</span>
          <span class="text-xl font-black tracking-tighter text-blue-300">LABS</span>
        </div>
      </Transition>
      
      <button
        type="button"
        class="w-10 h-10 flex items-center justify-center rounded-xl bg-white/10 text-white hover:bg-white/20 transition-all active:scale-90 shrink-0"
        @click.stop="openNav"
        :title="railMode ? 'Expandir' : 'Colapsar'"
      >
        <ChevronRight v-if="railMode" class="w-5 h-5" />
        <ChevronLeft v-else class="w-5 h-5" />
      </button>
    </div>

    <!-- Navegación -->
    <nav class="flex-1 overflow-y-auto px-2 py-4 space-y-1 custom-scrollbar">
      <div v-for="item in navigationItems" :key="item.id" class="relative group">
        <button
          type="button"
          class="nav-item-btn flex items-center rounded-xl transition-all duration-300 group"
          :class="[
            isActive(item) 
              ? 'bg-white text-[#1e3a8a] shadow-lg shadow-black/20 font-black' 
              : 'text-white/70 hover:bg-white/10 hover:text-white font-bold',
            railMode ? 'w-full justify-end px-0' : 'w-full px-3 py-2.5',
          ]"
          @click="handleItemClick(item, $event)"
        >
          <div :class="[railMode ? 'w-16' : 'mr-3']" class="flex items-center justify-center shrink-0">
            <component :is="item.icon" class="w-5 h-5 transition-transform duration-300 group-hover:scale-110" />
          </div>

          <span v-if="!railMode" class="flex-1 truncate text-sm text-left uppercase tracking-tight">{{ item.label }}</span>
          
          <ChevronRight
            v-if="item.children?.length && !railMode"
            class="w-4 h-4 opacity-40 transition-transform duration-300 ml-1"
            :class="expandedGroup === item.id ? 'rotate-90' : ''"
          />
        </button>

        <Transition name="submenu">
          <div
            v-if="item.children?.length && expandedGroup === item.id && !railMode"
            class="ml-9 mt-1 space-y-1 mb-2 border-l border-white/20 pl-4 overflow-hidden"
          >
            <button
              v-for="child in item.children"
              :key="child.id"
              @click="navigate(child.path)"
              class="flex w-full items-center rounded-lg px-3 py-2 text-xs font-bold transition-all duration-200"
              :class="isActive(child) ? 'text-white bg-white/10' : 'text-white/50 hover:text-white hover:bg-white/5'"
            >
              {{ child.label }}
            </button>
          </div>
        </Transition>
      </div>
    </nav>

    <!-- Flyout para Riel -->
    <Teleport to="body">
      <template v-if="flyoutGroup && railMode">
        <div class="fixed inset-0 z-[55]" @click="flyoutGroup = null" />
        <div
          class="fixed z-[59] w-56 rounded-2xl border border-white/10 shadow-2xl backdrop-blur-3xl bg-[#1e3a8a]/95 dark:bg-[#0F172A]/98 py-3 animate-in fade-in slide-in-from-left-4 duration-300"
          :style="{ top: `${flyoutTop}px`, left: '72px' }"
        >
          <p class="px-5 pb-2 mb-2 border-b border-white/10 text-[10px] font-black uppercase tracking-widest text-blue-200 dark:text-slate-500">
            {{ flyoutGroup.label }}
          </p>
          <div class="px-2 space-y-1">
            <button
              v-for="child in flyoutGroup.children"
              :key="child.id"
              class="flex w-full px-4 py-2 text-xs font-bold rounded-xl transition-all"
              :class="isActive(child) ? 'bg-white text-[#1e3a8a] dark:text-slate-900' : 'text-white/70 hover:bg-white/10 hover:text-white dark:text-slate-400 dark:hover:text-white'"
              @click="navigate(child.path)"
            >
              {{ child.label }}
            </button>
          </div>
        </div>
      </template>
    </Teleport>

    <!-- Footer: Sidebar Actions -->
    <div 
      class="shrink-0 border-t border-white/10 bg-black/5 flex flex-col transition-all duration-300"
      :class="railMode ? 'px-0 py-3 space-y-4' : 'p-3 space-y-3'"
    >
      <!-- Switch de Tema estilo Slider -->
      <div 
        class="flex items-center bg-black/30 rounded-xl p-1 relative h-10 transition-all"
        :class="railMode ? 'w-full justify-end' : 'w-full'"
      >
        <div v-if="railMode" class="w-16 flex items-center justify-center">
          <button
            @click="toggleDark"
            class="w-10 h-10 flex items-center justify-center rounded-xl text-white hover:bg-white/10 transition-all"
          >
            <Sun v-if="!isDarkMode" class="w-5 h-5" />
            <Moon v-else class="w-5 h-5" />
          </button>
        </div>

        <template v-else>
          <button
            @click="setTheme('light')"
            class="flex-1 flex items-center justify-center h-full rounded-lg transition-all relative z-10"
          >
            <Sun :class="[!isDarkMode ? 'text-blue-900 scale-110' : 'text-white/40']" class="w-4 h-4 transition-all duration-300" />
          </button>
          <button
            @click="setTheme('dark')"
            class="flex-1 flex items-center justify-center h-full rounded-lg transition-all relative z-10"
          >
            <Moon :class="[isDarkMode ? 'text-blue-500 scale-110' : 'text-white/40']" class="w-4 h-4 transition-all duration-300" />
          </button>
        </template>

        <!-- Indicador deslizante -->
        <div 
          v-if="!railMode"
          class="absolute top-1 bottom-1 left-1 bg-white rounded-lg transition-all duration-500 shadow-lg"
          :style="{ width: 'calc(50% - 4px)', transform: isDarkMode ? 'translateX(100%)' : 'translateX(0)' }"
        ></div>
      </div>

      <!-- Avatar Section -->
      <div 
        class="flex items-center transition-all duration-300 bg-white/10 rounded-xl group"
        :class="railMode ? 'px-0 py-1 bg-transparent justify-end' : 'p-2'"
      >
        <div :class="[railMode ? 'w-16 h-10' : '']" class="flex items-center justify-center shrink-0">
          <div class="h-9 w-9 rounded-lg flex items-center justify-center text-xs font-black bg-white text-[#1e3a8a] shadow-sm">
            {{ userInitials }}
          </div>
        </div>
        
        <div v-if="!railMode" class="ml-3 min-w-0 flex-1">
          <p class="truncate text-xs font-black text-white leading-none uppercase tracking-tight">{{ props.userName || 'Admin' }}</p>
          <p class="text-[9px] text-blue-300 font-bold uppercase tracking-widest mt-1 opacity-60">Super Admin</p>
        </div>

        <button
          v-if="!railMode"
          @click="emit('logout')"
          class="h-9 w-9 shrink-0 rounded-lg bg-red-500/20 hover:bg-red-500 text-white transition-all flex items-center justify-center active:scale-90 ml-2"
        >
          <LogOut class="w-4 h-4" />
        </button>
      </div>

      <!-- Logout Rail -->
      <div v-if="railMode" class="flex justify-end">
        <button
          @click="emit('logout')"
          class="w-16 h-10 flex items-center justify-center text-red-300 hover:text-white hover:bg-red-500/20 transition-all"
        >
          <LogOut class="w-5 h-5" />
        </button>
      </div>
    </div>
  </aside>

  <!-- Overlay Mobile -->
  <Transition name="fade">
    <div
      v-if="!isDesktop && isOpen"
      class="fixed inset-0 z-[35] bg-slate-900/60 backdrop-blur-sm shadow-inner"
      @click="isOpen = false; emitState()"
    />
  </Transition>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { LogOut, Search, ChevronLeft, ChevronRight, Sun, Moon } from 'lucide-vue-next'

export interface NavigationItem {
  id: string
  label: string
  path?: string
  icon?: any
  children?: NavigationItem[]
}

const props = defineProps<{
  navigationItems: NavigationItem[]
  userName?: string | null
}>()

const emit = defineEmits<{
  'sidebar-state-change': [state: { isDesktop: boolean; isOpen: boolean; isDesktopRailCollapsed: boolean }]
  navigate: [path: string]
  logout: []
  'open-search': []
}>()

const route = useRoute()

// Estado
const isOpen = ref(false)
const isDesktop = ref(window.innerWidth >= 768)
const isDesktopRailCollapsed = ref(true)
const flyoutGroup = ref<NavigationItem | null>(null)
const flyoutTop = ref(88)
const expandedGroup = ref<string | null>(null)
const isDarkMode = ref(false)

const railMode = computed(() => isDesktop.value && isDesktopRailCollapsed.value)

const userInitials = computed(() => {
  const name = props.userName?.trim() || 'Admin'
  const parts = name.split(' ')
  if (parts.length >= 2) return (parts[0]![0]! + parts[1]![0]!).toUpperCase()
  return name.slice(0, 2).toUpperCase()
})

const emitState = () => {
  emit('sidebar-state-change', {
    isDesktop: isDesktop.value,
    isOpen: isOpen.value,
    isDesktopRailCollapsed: isDesktopRailCollapsed.value,
  })
}

const setTheme = (theme: 'light' | 'dark') => {
  if (theme === 'dark') {
    document.documentElement.classList.add('dark')
    localStorage.setItem('theme', 'dark')
  } else {
    document.documentElement.classList.remove('dark')
    localStorage.setItem('theme', 'light')
  }
  isDarkMode.value = theme === 'dark'
}

const toggleDark = () => {
  const newTheme = isDarkMode.value ? 'light' : 'dark'
  setTheme(newTheme)
}

const openNav = () => {
  if (!isDesktop.value) {
    isOpen.value = !isOpen.value
    emitState()
    return
  }
  isDesktopRailCollapsed.value = !isDesktopRailCollapsed.value
  flyoutGroup.value = null
  updateExpandedFromRoute()
  emitState()
}

const isActive = (item: NavigationItem) => {
  if (item.path === route.path) return true
  return item.children?.some(child => child.path === route.path)
}

const updateExpandedFromRoute = () => {
  if (railMode.value) return
  const activeParent = props.navigationItems.find(item => 
    item.children?.some(child => child.path === route.path)
  )
  if (activeParent) {
    expandedGroup.value = activeParent.id
  }
}

const navigate = (path?: string) => {
  if (!path) return
  flyoutGroup.value = null
  emit('navigate', path)
  if (!isDesktop.value) {
    isOpen.value = false
    emitState()
  }
}

const handleItemClick = (item: NavigationItem, event: MouseEvent) => {
  if (!item.children?.length) {
    navigate(item.path)
    return
  }
  if (railMode.value) {
    const target = event.currentTarget as HTMLElement
    const rect = target.getBoundingClientRect()
    flyoutTop.value = rect.top
    flyoutGroup.value = flyoutGroup.value?.id === item.id ? null : item
    return
  }
  expandedGroup.value = expandedGroup.value === item.id ? null : item.id
}

const handleResize = () => {
  isDesktop.value = window.innerWidth >= 768
  if (!isDesktop.value) {
    isDesktopRailCollapsed.value = false
  }
  emitState()
}

watch(() => route.path, () => {
  updateExpandedFromRoute()
})

onMounted(() => {
  window.addEventListener('resize', handleResize)
  handleResize()
  updateExpandedFromRoute()

  // Inicializar tema persistente
  const savedTheme = localStorage.getItem('theme') as 'light' | 'dark' | null
  if (savedTheme) {
    setTheme(savedTheme)
  } else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
    // Si no hay guardado, opcionalmente seguir preferencia del sistema
    setTheme('dark')
  }
})
</script>

<style scoped>
.sidebar-container {
  background: #1e3a8a;
  border-right: 1px solid rgba(255, 255, 255, 0.1);
}

.dark .sidebar-container {
  background: #0f172a;
}

.cubic-bezier-sidebar {
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}

.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 10px;
}

/* Transiciones */
.submenu-enter-active, .submenu-leave-active {
  transition: all 0.3s ease-in-out;
  max-height: 200px;
}
.submenu-enter-from, .submenu-leave-to {
  max-height: 0;
  opacity: 0;
  transform: translateX(-10px);
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>
