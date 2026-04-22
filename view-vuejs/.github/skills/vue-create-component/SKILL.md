---
name: vue-create-component
description: Genera componentes Vue 3 tipados y consistentes, priorizando reutilización y bajo consumo de tokens.
license: Proprietary
compatibility: Vue 3, TypeScript, Tailwind CSS, Heroicons
metadata:
author: Ricardo B.T.
version: 1.3.0
---

# Skill: Crear Componente Vue (High Seniority)

## Propósito
Generar componentes Vue 3 funcionales, accesibles y reutilizables siguiendo estándares del proyecto y evitando sobre-generación.

## Invocación
```bash
/vue-create-component [Nombre] # Genera un componente base
```

## Argumentos
- `Nombre` (requerido): Nombre del componente en PascalCase.
- `props`: Lista de propiedades esperadas.

## Instrucciones / Estándares Aplicados

Si el usuario solicita crear un componente Vue 3, entonces DEBES seguir este proceso de ensamblaje senior:

### 1. Definición de Contrato (TS)
- Utiliza `defineProps<{}>()` y `defineEmits<{}>()` con tipos TypeScript explícitos.
- Prohibido el uso de `any`; define interfaces precisas para todas las Props.
- Usa `withDefaults` para definir valores por defecto.
- Si el componente implementa `v-model`, usar `defineModel<T>()`.
- Evitar destructuring directo de props para no perder claridad/reactividad; preferir `props.x` o `toRefs(props)` cuando sea necesario.

### 1.1 Reactividad y Rendimiento
- Para estructuras grandes que no requieren deep reactivity, preferir `shallowRef` sobre `ref`.
- Usar `computed` para derivaciones de estado, evitando duplicar estado mutable.
- Si hay watchers asíncronos (fetch por cambios), contemplar cleanup (`onWatcherCleanup`) para evitar condiciones de carrera.

### 2. Ensamblaje de UI de Bloques
- Queda **PROHIBIDO** el uso de etiquetas HTML nativas (`input`, `button`, `select`) si existe un componente equivalente en `@/components/shared/`.
- Inyecta componentes corporativos: `FormInput`, `AppButton`, `AppSelect`, `AppIcon`.
- Utiliza exclusivamente **Tailwind CSS** para el diseño responsivo y estados (`hover:`, `focus:`, `dark:`).

### 2.1 Modo Low-Token
- Reutiliza componentes y composición existente del proyecto antes de crear variantes nuevas.
- Si el componente solicitado es de layout/sidebar, reutiliza patrón de `AppSidebar` y `DefaultLayout` de referencias en vez de rediseñar.
- Evita incluir features no solicitadas.

### 2.2 Boundary de Componentes (Best Practices)
- Mantener componentes de ruta y layout como superficies de composición, no como contenedores de toda la lógica de negocio.
- Si el componente tiene múltiples responsabilidades claras (por ejemplo: orquestación + formulario + listado), dividir en subcomponentes y composables.
- Mantener contratos explícitos con `defineProps` y `defineEmits` tipados.

### 3. Documentación & Accesibilidad
- Incluye comentarios JSDoc para props y lógica compleja.
- Asegura que el componente sea accesible utilizando roles ARIA cuando sea necesario.

## Que Genera
- `[Nombre].vue`: Componente SFC con la lógica y el template estandarizado.
- Estructura de código limpia, autodocumentada y libre de CSS personalizado.

## Referencias y Código Reutilizable
Para generar código de forma consistente, DEBES leer solo lo necesario:
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/REAL_COMPONENT.md)
