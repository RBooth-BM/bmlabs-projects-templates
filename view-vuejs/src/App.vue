<template>
  <RouterView />
  <div id="modals"></div>

  <!-- Filtro SVG para remover fondo blanco de imágenes (transparencia simulada) -->
  <svg width="0" height="0" class="absolute pointer-events-none">
    <defs>
      <filter id="remove-white">
        <feColorMatrix type="matrix" values="1 0 0 0 0, 0 1 0 0 0, 0 0 1 0 0, -1.0 -1.0 -1.0 1.0 2.9" />
      </filter>
    </defs>
  </svg>
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>
