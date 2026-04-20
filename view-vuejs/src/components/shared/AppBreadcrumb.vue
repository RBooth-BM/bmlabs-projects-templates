<template>
  <nav aria-label="Breadcrumb" class="mb-4 text-sm text-slate-500 dark:text-slate-400">
    <ol class="flex flex-wrap items-center gap-2">
      <li class="font-medium text-slate-700 dark:text-slate-200">Inicio</li>
      <li v-for="(crumb, index) in crumbs" :key="crumb.path" class="flex items-center gap-2">
        <span>/</span>
        <span :class="index === crumbs.length - 1 ? 'font-semibold text-slate-800 dark:text-slate-100' : ''">
          {{ crumb.label }}
        </span>
      </li>
    </ol>
  </nav>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const crumbs = computed(() => {
  return route.path
    .split('/')
    .filter(Boolean)
    .map((segment, index, arr) => ({
      label: segment.charAt(0).toUpperCase() + segment.slice(1),
      path: `/${arr.slice(0, index + 1).join('/')}`,
    }))
})
</script>
