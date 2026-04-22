# Skill: Crear Componente Vue

## Propósito
Generar Single File Components (SFC) Vue 3 funcionales, tipados y reutilizables siguiendo las mejores prácticas del proyecto.

## Invocación
```
@copilot /vue-create-component [ComponentName] [options]
```

## Argumentos

- `ComponentName` (requerido): Nombre del componente en PascalCase
- `type` (opcional): 'ui', 'form', 'layout', 'feature' (default: 'ui')
- `props` (opcional): Lista de props (ej: "label, onClick, disabled")
- `styles` (opcional): 'tailwind' o 'scoped' (default: 'tailwind')

## Ejemplo de Invocación

```
@copilot /vue-create-component Button type:ui props:"label, disabled, variant" styles:tailwind
```

## Que Genera

1. **ComponentName.vue**: SFC con `<script setup lang="ts">`, template y estilos
2. **ComponentName.types.ts**: Interfaces de props
3. **ComponentName.test.ts**: Tests básicos con Vue Test Utils

## Estándares Aplicados

- `<script setup lang="ts">` requerido
- Props tipadas con `defineProps<{}>()`
- Emits tipados con `defineEmits<{}>()`
- Accesibilidad WCAG 2.1
- Tailwind CSS para estilos
- Barrel exports en index.ts
- JSDoc comments

## Ejemplo de Salida

```typescript
// Button.types.ts
export interface ButtonProps {
  label: string
  disabled?: boolean
  variant?: 'primary' | 'secondary' | 'danger'
  size?: 'sm' | 'md' | 'lg'
  type?: 'button' | 'submit' | 'reset'
}
```

```vue
<!-- Button.vue -->
<script setup lang="ts">
import type { ButtonProps } from './Button.types'

/**
 * Componente Button reutilizable
 */
const props = withDefaults(defineProps<ButtonProps>(), {
  disabled: false,
  variant: 'primary',
  size: 'md',
  type: 'button',
})

const emit = defineEmits<{
  click: [event: MouseEvent]
}>()

const baseStyles = 'font-semibold rounded transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2'

const variantStyles = {
  primary: 'bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500',
  secondary: 'bg-gray-200 text-gray-900 hover:bg-gray-300 focus:ring-gray-500',
  danger: 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-500',
}

const sizeStyles = {
  sm: 'px-3 py-1 text-sm',
  md: 'px-4 py-2 text-base',
  lg: 'px-6 py-3 text-lg',
}
</script>

<template>
  <button
    :type="props.type"
    :disabled="props.disabled"
    :class="[baseStyles, variantStyles[props.variant], sizeStyles[props.size], props.disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer']"
    :aria-disabled="props.disabled"
    @click="emit('click', $event)"
  >
    {{ props.label }}
  </button>
</template>
```

## Uso en Componentes Padres

```vue
<script setup lang="ts">
import Button from '@/components/ui/Button.vue'

const handleClick = () => {
  console.log('Clicked!')
}
</script>

<template>
  <Button
    label="Click me"
    variant="primary"
    size="md"
    @click="handleClick"
  />
</template>
```

## Variantes Comunes

### Para formularios
```
@copilot /vue-create-component Input type:form props:"label, modelValue, type, error" styles:tailwind
```

### Para secciones de página
```
@copilot /vue-create-component ProductCard type:feature props:"product, onEdit, onDelete" styles:tailwind
```

### Para layouts
```
@copilot /vue-create-component Container type:layout props:"maxWidth" styles:tailwind
```

## Checklist de Calidad

- [ ] `<script setup lang="ts">` usado
- [ ] Props interface definida con `defineProps<{}>()`
- [ ] Emits tipados con `defineEmits<{}>()`
- [ ] ARIA labels para accesibilidad
- [ ] Tailwind CSS utilizado correctamente
- [ ] Tests unitarios incluidos
- [ ] Documentación JSDoc
- [ ] Exportado en index.ts
- [ ] Responsive design considerado
- [ ] Dark mode compatible
