---
name: vue-create-pinia-store
description: Estándar Pinia para Setup Stores tipados, reactividad segura y uso correcto dentro y fuera de componentes.
license: Proprietary
compatibility: Vue 3, Pinia 3, TypeScript
metadata:
  author: Ricardo B.T.
  version: 1.0.0
  based_on: antfu/skills pinia + BM conventions
---

# Skill: Crear Store Pinia (Setup Store First)

## Propósito
Generar stores Pinia robustos, tipados y predecibles, priorizando Setup Stores, reactividad correcta y uso seguro en guards/servicios.

## Invocación
```bash
/I [StoreName] # Genera o mejora un store Pinia
```

## Reglas Inflexibles

### 1. Forma del Store
- Preferir SIEMPRE Setup Store: `defineStore('name', () => { ... })`.
- Exponer estado, computed y acciones explícitamente en el `return`.
- Prohibido `any`.

### 2. Reactividad y Desestructuración
- Si se desestructura estado/getters en componentes, usar `storeToRefs(store)`.
- Las acciones se pueden desestructurar directamente desde el store.

### 3. Uso fuera de Componentes
- No llamar `useStore()` a nivel de módulo en guards/plugins.
- Llamar `useStore()` dentro de funciones (`beforeEach`, handlers, actions).

### 4. Mutaciones y Reset
- Preferir acciones tipadas para cambios de negocio.
- Para updates múltiples, considerar `$patch`.
- Definir reset explícito (`$reset` o función equivalente) en Setup Stores.

### 5. Composición entre Stores
- Se permite `store A` usando `store B`, evitando lecturas cruzadas en setup que creen circularidad.
- Leer estado de otros stores dentro de actions/computed, no en inicialización circular.

### 6. Recomendación Dev
- Agregar soporte HMR cuando aplique:
```ts
if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useXStore, import.meta.hot))
}
```

## Plantilla Base
```ts
import { computed, ref } from 'vue'
import { defineStore } from 'pinia'

export const useExampleStore = defineStore('example', () => {
  const loading = ref(false)
  const items = ref<string[]>([])

  const hasItems = computed(() => items.value.length > 0)

  function $reset() {
    loading.value = false
    items.value = []
  }

  async function fetchItems() {
    try {
      loading.value = true
      // ...
    } finally {
      loading.value = false
    }
  }

  return {
    loading,
    items,
    hasItems,
    fetchItems,
    $reset,
  }
})
```

## Checklist
- [ ] Setup Store (no Options Store)
- [ ] Estado/getters/actions bien tipados
- [ ] Sin `any`
- [ ] `useStore()` fuera de módulo en guards/plugins
- [ ] Reset implementado
- [ ] HMR opcional agregado en desarrollo
