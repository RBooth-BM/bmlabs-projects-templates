---
name: vue-create-composables
description: Genera composables Vue 3 (Composition API) para la gestión del estado reactivo, lógica de negocio encapsulada y orquestación de servicios.
license: Proprietary
compatibility: Vue 3, Composition API, Pinia (opcional)
metadata:
  author: Ricardo B.T.
  version: 1.3.0
---

# Skill: Crear Composables Vue (Logic & State)

## Propósito
Encapsular la lógica reactiva y la gestión del estado en funciones reutilizables, separando la lógica de negocio de la capa de presentación (componentes) para facilitar el testing y la mantenibilidad.

## Invocación
```bash
/D [Entidad] # Genera un composable para la entidad especificada
```

## Argumentos
- `Entidad`: Nombre de la entidad (ej: Usuario, Producto).

## Instrucciones / Estándares Aplicados

Si el usuario solicita crear lógica reactiva o un Composable, entonces DEBES seguir este estándar de implementación:

### 1. Estado Unificado y Reactivo
- Todo composable debe exponer como mínimo las variables base: `loading` (ref booleana), `error` (ref string/null) e `items` (ref tipada).
- Utiliza `ref` para valores primitivos y `reactive` para objetos complejos de estado si es necesario.
- Para payloads grandes o listas extensas donde no se necesita reactividad profunda, preferir `shallowRef`.

### 2. Encapsulamiento Asíncrono
- Implementa métodos de acción (ej: `fetchItems`, `saveItem`, `deleteItem`) utilizando bloques `try-catch-finally`.
- La comunicación con el backend DEBE delegarse a una instancia de un **API Service** (inyectado o importado).
- Si hay watchers con llamadas asíncronas, usar cleanup (`onWatcherCleanup`) para abortar peticiones previas.

### 2.1 Entradas Reactivas Flexibles
- Cuando el composable acepta parámetros externos, soportar `MaybeRefOrGetter<T>` y normalizar con `toValue()`.
- Evitar lógica duplicada para manejar `value`, getter o primitivo.

### 2.2 Seguridad de Contexto (SSR-Ready)
- No invocar composables dependientes de contexto a nivel de módulo.
- No definir estado global mutable con `ref` fuera de funciones exportadas del composable.
- Inicializar dependencias dentro de `useXxx()` para evitar fugas de estado entre ejecuciones.

### 3. Notificaciones Integradas
- Utiliza el sistema de notificaciones del proyecto (ej: `useNotify` o `Swal.fire`) para informar al usuario sobre el resultado de las operaciones asíncronas.

### 3.1 Integración con VueUse (Cuando Aplique)
- Si `@vueuse/core` ya está instalado, preferir composables probados frente a utilidades manuales.
- Casos sugeridos:
  - Persistencia reactiva: `useStorage` / `useLocalStorage` / `useSessionStorage`
  - Estado async: `useAsyncState`
  - Eventos del navegador: `useEventListener`
  - Watchers con control de frecuencia: `watchDebounced` / `watchThrottled`
- No agregar nuevas dependencias automáticamente solo por preferencia técnica; respetar stack existente del proyecto.

### 4. Interfaz de Retorno
- Devuelve un objeto plano que contenga solo las propiedades y métodos necesarios, permitiendo la desestructuración limpia en el componente.

## Checklist de Calidad

- [ ] ¿El nombre sigue el patrón `use[Entidad]`?
- [ ] ¿Expone estados de `loading` y `error`?
- [ ] ¿Utiliza `try-catch` en todas las funciones asíncronas?
- [ ] ¿Maneja el tipado estricto en las refs de datos?
- [ ] ¿Delega las llamadas HTTP al servicio correspondiente (`.api.ts`)?
- [ ] ¿Incluye notificaciones visuales tras completar acciones?
- [ ] ¿Usa `shallowRef` cuando aplica por performance?
- [ ] ¿Usa `toValue()` en parámetros reactivos?
- [ ] ¿Cancela efectos asíncronos obsoletos en watchers?
- [ ] ¿Evita estado y composables a nivel de módulo?
- [ ] ¿Reutiliza VueUse cuando ya está disponible en el proyecto?

## Que Genera
- `use[Entidad].ts`: Archivo TypeScript con la lógica reactiva encapsulada y lista para ser consumida por múltiples componentes.

## Referencias y Código Reutilizable
Para generar el código con el máximo nivel de detalle, DEBES leer los ejemplos reales, validaciones Zod y arquitectura de BM ubicados en la referencia técnica:
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/REAL_COMPOSABLES.md)
