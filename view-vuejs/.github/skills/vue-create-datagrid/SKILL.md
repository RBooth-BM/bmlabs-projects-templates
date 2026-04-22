---
name: vue-create-datagrid
description: Genera grillas de datos (DataTables) avanzadas con paginación, filtrado por columna y acciones de gestión corporativas.
license: Proprietary
compatibility: Vue 3, Lucide Icons, Shared DataTable Component
metadata:
  author: Ricardo B.T.
  version: 1.0.0
---

# Skill: Crear DataGrid (Unified List View)

## Propósito
Implementar la visualización de datos masivos en tablas interactivas, asegurando que se utilice el componente `DataTable` unificado y se orquesten correctamente los eventos de filtrado, paginación y acciones de fila.

## Invocación
Se activa automáticamente cuando se genera la SECCIÓN 2 (DATOS) de un CRUD o mediante:
```bash
/C # Para generar una tabla de datos completa basada en una entidad
```

## Argumentos
- `Entidad`: Nombre del modelo a listar.
- `Columnas`: Definición de campos visibles.

## Instrucciones / Estándares Aplicados

Si el usuario solicita crear una grilla de datos (DataGrid), entonces DEBES seguir este estándar de orquestación:

### 1. Definición de Columnas y Acciones
- Define un array `columns` con el tipado `TableColumn[]` de `@/components/shared/datatable/data-table.types`.
- Define un array `actions` con el tipado `TableAction[]` del mismo archivo.
- Configura las acciones (Editar, Eliminar, Ver) con `iconComponent` usando iconos de `lucide-vue-next`.
- El componente `DataTable` maneja internamente filtros por columna, paginación, botón de refresh interno y limpieza global de filtros.
- **PROHIBIDO** usar tablas HTML (`<table>`) nativas para el listado principal — usar siempre `DataTable`.

### 2. Integración con Composable de Datos
- Vincula el prop `:data` con la ref `items` o `registros` del composable/vista.
- Para paginación server-side: `:total-records`, `:records-per-page`, `:current-page` y escuchar `@update:currentPage`/`@update:recordsPerPage`.
- Para refresh de datos, usa `:show-refresh-button="true"` y escucha `@refresh`.
- Para paginación client-side: usar `:client-side-operations="true"`.

### 3. Orquestación de Eventos
- Escuchar `@action-clicked="(key, row) => ..."` para derivar la lógica a los modales (Paso 10: ACCIONES).
- El `key` del evento corresponde al campo `key` definido en el array `actions`.
- Escuchar `@update:filters` para disparar búsqueda, y soportar limpieza global con el botón del `DataTable`.

### 4. Reglas UX Obligatorias (anti-regresión)
- **Texto visible en español**: labels, placeholders, botones, estados vacíos y mensajes de tabla.
- **Refresh dentro de tabla**: no dejar botón "Actualizar" textual fuera del `DataTable` cuando la vista principal depende de este componente.
- **Filtros con estado visible**: debe existir indicador de filtro activo por columna y limpieza global de filtros.
- **Relaciones por select**: si una entidad depende de otra, usar select/multiselect con datos reales; evitar pedir UUID manual.
- **Paginación válida por defecto**: inicializar `recordsPerPage` con valor presente en el selector (default recomendado: `5`).
- **Opciones de paginación**: priorizar múltiplos de 5 para consistencia.

## Checklist de Calidad

- [ ] ¿Se utiliza el componente `DataTable` corporativo?
- [ ] ¿Las columnas están correctamente tipadas y mapeadas al DTO?
- [ ] ¿Se inyectaron los componentes `Filter` y `Paginator` en los slots o props correspondientes?
- [ ] ¿El botón de refresh está dentro del DataTable y conectado a `@refresh`?
- [ ] ¿Existe limpieza global de filtros?
- [ ] ¿La UI visible de la tabla está en español?
- [ ] ¿`recordsPerPage` inicia con valor válido (sin selector en blanco)?
- [ ] ¿Las relaciones usan selects y no UUID manual?
- [ ] ¿El estado de carga (`loading`) bloquea la interacción?
- [ ] ¿Las acciones de fila disparan los modales correctos (SECCIÓN 3)?
- [ ] ¿La tabla es responsiva y tiene scroll horizontal en móviles?

## Que Genera
La integración del núcleo administrativo en la SECCIÓN 2 del pattern 12/3, conectando la lógica del servidor con la interactividad del usuario.

## Referencias y Código Reutilizable
Para generar el código con el máximo nivel de detalle, DEBES leer los ejemplos reales, validaciones Zod y arquitectura de BM ubicados en la referencia técnica:
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/REAL_DATAGRID.md)
