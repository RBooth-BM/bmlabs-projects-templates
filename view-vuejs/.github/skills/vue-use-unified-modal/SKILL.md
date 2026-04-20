---
name: vue-use-unified-modal
description: Gestiona la orquestación y el estado de diálogos modales (AppModal y ConfirmModal) con flujos de confirmación asíncronos corporativos.
license: Proprietary
compatibility: Vue 3, Tailwind CSS, Shared Component Library
metadata:
  author: Ricardo B. T.
  version: 1.0.0
---

# Skill: Unified Modal Management

## Propósito
Estandarizar la forma en que el Agente implementa diálogos en la aplicación, asegurando que se utilicen los componentes base (`AppModal` y `ConfirmModal`) con una gestión de estado reactiva y asíncrona consistente.

## Invocación
```bash
/M # Para añadir un modal de formulario o confirmación a una vista
```

## Argumentos
- `tipo`: 'form' | 'confirm' | 'danger'.
- `entidad`: Nombre de la entidad afectada.

## Instrucciones / Estándares Aplicados

Si el usuario requiere implementar un diálogo de interacción, entonces DEBES seguir este patrón de orquestación:

### 1. Gestión de Visibilidad
- Todo modal debe controlarse mediante una constante ref booleana exclusiva: `const isModalOpen = ref(false)`.
- El método para abrir el modal debe resetear estados previos si es necesario.

### 2. Selección de Componente Unificado
- **Formularios (`BaseModal` con `hide-footer="true"`)**: Para procesos de Crear/Editar. Debe ocultar el footer del BaseModal y usar `AppButton` para acciones personalizadas.
- **Confirmaciones (`ConfirmModal`)**: Para acciones simples de confirmación o cancelación.
- **Acciones Riesgosas (`variant="danger"`)**: Para borrado de datos o procesos irreversibles.

### 3. Estructura de Botones en Formularios
- **NO usar footer del BaseModal**: Siempre usar `:hide-footer="true"` en BaseModal para formularios
- **Botones personalizados**: Usar `AppButton` con `variant="secondary"` para cancelar y `variant="primary"` para confirmar
- **Posicionamiento**: Botones al final del contenido con `flex justify-end gap-2 border-t pt-3`
- **Estados**: Incluir `loading` y `disabled` según el estado de la operación

### 3. Flujo Asíncrono & Feedback
- Implementa un estado `loading` local para el botón de confirmación.
- Lanza la acción en el evento `@confirm` o `@submit`.
- Cierra el modal y notifica éxito/error tras completar la operación usando `Swal.fire` o Toasts.

## Checklist de Calidad

- [ ] ¿Se usa `isModalOpen` como ref para visibilidad?
- [ ] ¿Se utiliza `BaseModal` del directorio `@/components/shared/`?
- [ ] ¿Para formularios: se usa `:hide-footer="true"` y botones `AppButton` personalizados?
- [ ] ¿Se implementó la variante `danger` para acciones destructivas?
- [ ] ¿El modal se cierra automáticamente tras un éxito asíncrono?
- [ ] ¿Se incluye el manejo de `loading` para prevenir clics dobles?
- [ ] ¿Prohibido el uso de `window.confirm`?

## Que Genera
La integración de un componente modal en el bloque `<template>` (SECCIÓN 3) y la lógica de control en el `script setup`, siguiendo el patrón de orquestación de BM.

## Referencias y Código Reutilizable
Para generar la integración exacta, DEBES consultar el código fuente del patrón real alojado aquí:
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/COMPONENTES_COMUNES.md)
