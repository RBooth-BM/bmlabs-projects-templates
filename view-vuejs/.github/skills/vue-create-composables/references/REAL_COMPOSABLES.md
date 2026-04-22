# Skill: Crear Composable (Custom Hook)

## Propósito
Generar composables para operaciones CRUD, gestión de estado reactivo y lógica compartida entre componentes Vue.

## Invocación
```
@copilot /vue-create-composables [ComposableName] [options]
```

## Argumentos

- `ComposableName` (requerido): Nombre en camelCase con prefijo 'use'
- `type` (opcional): 'crud', 'form', 'api', 'state' (default: 'crud')
- `entity` (opcional): Entidad a manejar
- `service` (opcional): Servicio a usar

## Ejemplo de Invocación

```
@copilot /vue-create-composables useProduct type:crud entity:Product service:productService
```

## Que Genera

1. **composable.ts**: Función composable completa con refs reactivos
2. **composable.types.ts**: Interfaces y tipos
3. **composable.test.ts**: Tests unitarios

## Composable useCrud

Patrón para CRUD genérico:

```typescript
// composables/useProduct.ts
import { ref } from 'vue'
import { productService } from '@/services'
import type { Product, CreateProductDTO, UpdateProductDTO } from '@/services/product.types'

export interface UseProductResult {
  items: Ref<Product[]>
  item: Ref<Product | null>
  loading: Ref<boolean>
  error: Ref<Error | null>
  create: (data: CreateProductDTO) => Promise<void>
  read: (id: string) => Promise<void>
  readAll: (filters?: unknown) => Promise<void>
  update: (id: string, data: UpdateProductDTO) => Promise<void>
  remove: (id: string) => Promise<void>
  reset: () => void
  clearError: () => void
}

/**
 * Composable para gestionar productos
 * Proporciona métodos CRUD y estado reactivo
 */
export const useProduct = (): UseProductResult => {
  const items = ref<Product[]>([])
  const item = ref<Product | null>(null)
  const loading = ref(false)
  const error = ref<Error | null>(null)

  const handleError = (err: unknown, context: string) => {
    const errorMessage = err instanceof Error ? err.message : 'Error desconocido'
    console.error(`Error in ${context}:`, errorMessage)
    error.value = new Error(errorMessage)
  }

  const create = async (data: CreateProductDTO) => {
    try {
      loading.value = true
      error.value = null
      const newProduct = await productService.create(data)
      items.value = [...items.value, newProduct]
      item.value = newProduct
    } catch (err) {
      handleError(err, 'useProduct.create')
    } finally {
      loading.value = false
    }
  }

  const read = async (id: string) => {
    try {
      loading.value = true
      error.value = null
      item.value = await productService.getById(id)
    } catch (err) {
      handleError(err, 'useProduct.read')
    } finally {
      loading.value = false
    }
  }

  const readAll = async (filters?: unknown) => {
    try {
      loading.value = true
      error.value = null
      items.value = await productService.getAll(filters)
    } catch (err) {
      handleError(err, 'useProduct.readAll')
    } finally {
      loading.value = false
    }
  }

  const update = async (id: string, data: UpdateProductDTO) => {
    try {
      loading.value = true
      error.value = null
      const updated = await productService.update(id, data)
      items.value = items.value.map((p) => (p.id === id ? updated : p))
      item.value = updated
    } catch (err) {
      handleError(err, 'useProduct.update')
    } finally {
      loading.value = false
    }
  }

  const remove = async (id: string) => {
    try {
      loading.value = true
      error.value = null
      await productService.delete(id)
      items.value = items.value.filter((p) => p.id !== id)
      if (item.value?.id === id) item.value = null
    } catch (err) {
      handleError(err, 'useProduct.remove')
    } finally {
      loading.value = false
    }
  }

  const reset = () => {
    items.value = []
    item.value = null
    error.value = null
  }

  const clearError = () => {
    error.value = null
  }

  return { items, item, loading, error, create, read, readAll, update, remove, reset, clearError }
}

export default useProduct
```

## Composable useForm

Para gestión de formularios:

```typescript
// composables/useForm.ts
import { ref, reactive } from 'vue'

export interface UseFormResult<T> {
  values: T
  errors: Partial<Record<keyof T, string>>
  touched: Partial<Record<keyof T, boolean>>
  isSubmitting: Ref<boolean>
  setFieldValue: (field: keyof T, value: unknown) => void
  setFieldError: (field: keyof T, error: string) => void
  setFieldTouched: (field: keyof T, touched: boolean) => void
  handleSubmit: (onSubmit: (values: T) => Promise<void>) => (event: Event) => Promise<void>
  reset: () => void
}

export const useForm = <T extends Record<string, unknown>>(
  initialValues: T,
  validate?: (values: T) => Partial<Record<keyof T, string>>
): UseFormResult<T> => {
  const values = reactive<T>({ ...initialValues }) as T
  const errors = reactive<Partial<Record<keyof T, string>>>({})
  const touched = reactive<Partial<Record<keyof T, boolean>>>({})
  const isSubmitting = ref(false)

  const setFieldValue = (field: keyof T, value: unknown) => {
    (values as Record<string, unknown>)[field as string] = value
    if (validate) {
      const newErrors = validate(values)
      errors[field] = newErrors[field]
    }
  }

  const setFieldError = (field: keyof T, error: string) => {
    errors[field] = error
  }

  const setFieldTouched = (field: keyof T, isTouched: boolean) => {
    touched[field] = isTouched
  }

  const handleSubmit =
    (onSubmit: (values: T) => Promise<void>) =>
    async (event: Event) => {
      event.preventDefault()

      if (validate) {
        const newErrors = validate(values)
        Object.assign(errors, newErrors)
        if (Object.keys(newErrors).length > 0) return
      }

      try {
        isSubmitting.value = true
        await onSubmit(values)
      } finally {
        isSubmitting.value = false
      }
    }

  const reset = () => {
    Object.assign(values, initialValues)
    Object.keys(errors).forEach((k) => delete (errors as Record<string, unknown>)[k])
    Object.keys(touched).forEach((k) => delete (touched as Record<string, unknown>)[k])
  }

  return { values, errors, touched, isSubmitting, setFieldValue, setFieldError, setFieldTouched, handleSubmit, reset }
}
```

## Composable useApi

Para peticiones HTTP genéricas:

```typescript
// composables/useApi.ts
import { ref, onMounted } from 'vue'

export interface UseApiResult<T> {
  data: Ref<T | null>
  loading: Ref<boolean>
  error: Ref<Error | null>
  refetch: () => Promise<void>
}

export const useApi = <T>(
  fetchFn: () => Promise<T>,
  immediate = true
): UseApiResult<T> => {
  const data = ref<T | null>(null) as Ref<T | null>
  const loading = ref(true)
  const error = ref<Error | null>(null)

  const refetch = async () => {
    try {
      loading.value = true
      error.value = null
      data.value = await fetchFn()
    } catch (err) {
      error.value = err instanceof Error ? err : new Error('Error desconocido')
    } finally {
      loading.value = false
    }
  }

  if (immediate) {
    onMounted(refetch)
  }

  return { data, loading, error, refetch }
}
```

## Uso en Vistas

```vue
<!-- views/catalog/ProductListView.vue -->
<script setup lang="ts">
import { onMounted } from 'vue'
import { useProduct } from '@/composables/useProduct'
import ProductGrid from '@/components/tables/ProductGrid.vue'

const { items, loading, error, readAll, remove } = useProduct()

onMounted(readAll)
</script>

<template>
  <div v-if="error" class="text-red-600">Error: {{ error.message }}</div>

  <ProductGrid
    :data="items"
    :loading="loading"
    @delete="remove"
  />
</template>
```

## Variantes Comunes

### Composable para autenticación
```
@copilot /vue-create-composables useAuth type:state service:authService
```

### Composable para filtros
```
@copilot /vue-create-composables useFilters type:state
```

### Composable para paginación
```
@copilot /vue-create-composables usePagination type:state
```

## Checklist de Calidad

- [ ] Nombre con prefijo `use`
- [ ] Tipos bien definidos con interfaces exportadas
- [ ] Retorna objeto destructurable con refs
- [ ] Manejo de errores robusto
- [ ] `onUnmounted` si hay listeners o timers activos
- [ ] Documentación JSDoc
- [ ] Tests unitarios
- [ ] Reutilizable y genérico
- [ ] Exportado en index.ts
