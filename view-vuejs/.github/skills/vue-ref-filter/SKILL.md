---
name: vue-ref-filter
description: Instrucciones técnicas para la implementación y posicionamiento del componente Filter corporativo.
license: Proprietary
compatibility: Vue 3, Tailwind CSS
metadata:
  author: Ricardo B.T.
  version: 1.0.0
---

# Skill: Filter Component Reference

## Propósito
Guiar al Agente en la implementación correcta del componente `Filter`, asegurando el manejo de eventos de búsqueda, limpieza de filtros y el posicionamiento inteligente del dropdown en la interfaz.

## Invocación
Se activa automáticamente cuando se genera la SECCIÓN 2 (DATOS) de un CRUD o mediante:
```bash
/A # Para validar la lógica de un filtro existente
```

## Instrucciones / Estándares Aplicados

Al integrar el componente `Filter`, DEBES asegurar:

### 1. Gestión de Estado Base
- Utiliza `modelValue` para el binding y `isActive` para controlar la visibilidad desde el padre si es necesario.
- Define el objeto `filtros` en el Paso 5 del Script Setup del componente padre.

### 2. Posicionamiento Inteligente
- El componente calcula su posición (`dropdownPosition`) basándose en los límites de la ventana (window edges) para evitar desbordamientos horizontales.

### 3. Eventos de Comunicación
- Escucha `@search` para disparar el refresco de datos en el Composable.
- Maneja el cierre del dropdown con eventos de clic fuera del área (`handleClickOutside`).

## Checklist de Calidad

- [ ] ¿El componente recibe `v-model` correctamente?
- [ ] ¿Se especificó el `type` (text, date, select)?
- [ ] ¿Se inyectó la lógica de búsqueda en el evento `@search`?
- [ ] ¿Se maneja el flag `disabled` para estados de carga?
- [ ] ¿El diseño es consistente con el resto de la tabla?

## Que Genera
La integración lógica y visual de filtros por columna o globales dentro del `DataTable`.

## Referencias y Código Reutilizable
Para generar la integración exacta, DEBES consultar el código fuente del componente real alojado aquí:
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/COMPONENTES_COMUNES.md)
