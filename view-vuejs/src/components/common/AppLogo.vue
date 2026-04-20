<template>
  <div class="app-logo-container" :style="{ width: size + 'px', height: size + 'px' }">
    <!-- Si es SVG, podemos inyectarlo o usar img. Para permitir edición fácil, usamos img o una prop de src -->
    <img 
      :src="logoSrc" 
      :alt="BRAND_CONFIG.logo.alt"
      class="w-full h-full object-contain"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { BRAND_CONFIG } from '@/config/brand.config';
import logoUrl from '@/assets/logo/logo.svg';

const props = defineProps<{
  size?: number | string;
  src?: string;
}>();

const logoSrc = computed(() => props.src || logoUrl);
</script>

<style scoped>
.app-logo-container {
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.3s ease;
}

.app-logo-container:hover {
  transform: scale(1.05);
}
</style>
