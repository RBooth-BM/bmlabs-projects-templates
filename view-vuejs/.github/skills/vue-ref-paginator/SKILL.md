---
name: vue-ref-paginator
description: Implementa la lógica de paginación reactiva y navegación de registros para listas de datos extensas.
license: Proprietary
compatibility: Vue 3, Lucide Icons, Tailwind CSS
metadata:
  author: Ricardo B. T.
  version: 1.0.0
---

# Skill: Paginator Component Reference

## Propósito
Asegurar que la navegación entre páginas de datos sea fluida, accesible y consistente, permitiendo al usuario controlar la cantidad de registros por página y navegar rápidamente mediante controles visuales estándar.

## Invocación
Se activa automáticamente cuando se genera la SECCIÓN 2 (DATOS) de un CRUD o mediante:
```bash
/A # Para validar la configuración de una paginación existente
```

## Argumentos
- `totalRecords`: Número total de registros en la base de datos.
- `recordsPerPage`: Cantidad de registros por vista.
- `currentPage`: Página actual activa.

## Instrucciones / Estándares Aplicados

Al integrar el componente `Paginator`, DEBES asegurar:

### 1. Sincronización de Estado
- Utiliza `v-model:currentPage` y `v-model:recordsPerPage` para mantener la reactividad bidireccional entre el componente y el Composable.
- `recordsPerPage` debe inicializarse con un valor válido del selector para evitar estado en blanco.

### 2. Cálculos de Rango
- El componente debe calcular dinámicamente las `visiblePages` incluyendo elipsis (`...`) para no saturar la interfaz en listas largas.
- Si no hay registros, el rango inicial debe manejarse sin valores inválidos.

### 3. Eventos de Navegación
- Los botones de navegación (Primero, Anterior, Siguiente, Último) DEBEN emitir los eventos correspondientes y deshabilitarse (`:disabled`) cuando los límites sean alcanzados.

### 4. Estética e Iconografía
- Usa exclusivamente los iconos de `lucide-vue-next` (ChevronFirst, ChevronLeft, etc.) alineados con el sistema de diseño.

### 5. Convención de Tamaños por Página
- Valor por defecto recomendado: `5`.
- Opciones recomendadas: múltiplos de 5 (`5, 10, 15, 20, 25, 50, 100`).
- Si `recordsPerPage` llega con un valor no estándar, debe mostrarse igualmente en el selector y no quedar en blanco.

## Checklist de Calidad

- [ ] ¿Se vinculó correctamente el `modelValue` para la página actual?
- [ ] ¿Los botones de navegación tienen estados `:disabled` correctos?
- [ ] ¿Se muestra el selector de registros por página (opcional)?
- [ ] ¿El selector nunca queda en blanco?
- [ ] ¿Default y opciones siguen la convención acordada del proyecto?
- [ ] ¿El diseño es responsivo y se adapta a móviles?
- [ ] ¿Se utiliza Tailwind CSS para todos los estados visuales?
- [ ] ¿Es compatible con Dark Mode?

## Que Genera
La integración lógica de la barra de navegación de páginas al pie de las tablas de datos (SECCIÓN 2).

## Referencias y Código Reutilizable
Para generar la integración exacta, DEBES consultar el código fuente del componente real alojado aquí:
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/COMPONENTES_COMUNES.md)
