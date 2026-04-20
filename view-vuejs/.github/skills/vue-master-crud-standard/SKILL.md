---
name: vue-master-crud-standard
description: Garantiza que todas las vistas CRUD sigan el estándar 12/3 con componentes compartidos reales y TypeScript estricto.
license: Proprietary
compatibility: Vue 3, TypeScript, Vite, Tailwind CSS
metadata:
  author: Ricardo B.T.
  version: 2.0.0
  standard: BM 12/3
---

# Skill: Master CRUD Standard (Patrón BM 12/3)

## Propósito
Asegurar que toda vista administrativa (CRUD) tenga una estructura predecible, tipada y que use exclusivamente componentes de `@/components/shared/`.

## Invocación
```bash
/C [EntityName] # Genera vista CRUD completa con patrón 12/3
```

## Instrucciones / Estándares Aplicados

### 1. Estructura del Template (3 Bloques Obligatorios)
- **SECCIÓN 1 (ACCIONES)**: `AppButton` con `variant="primary"` y `variant="secondary"`. IDs `kebab-case`.
- **SECCIÓN 2 (DATOS)**: Tabla con `Filter` en headers, estados loading/empty, y `Paginator`.
- **SECCIÓN 3 (MODALES)**: `CreateEdit[Entity]Modal` basado en `BaseModal`.

### 2. Orden del Script Setup (12 Pasos)
```
1. IMPORTS        — Vue, Lucide icons, componentes compartidos, servicios
2. ESTADO PRINCIPAL — ref<Entity[]>([]), ref<number>(0), ref<boolean>(false)
3. ESTADO UI      — showModal, selectedItem, operacion
4. PAGINACIÓN     — currentPage, recordsPerPage
5. FILTROS        — activeFilter, filtros reactive
6. OPCIONES       — listas para Selects
7. COMPUTED       — transformaciones de datos
8. UTILIDAD       — setActiveFilter, updateRecordsPerPage
9. API            — fetchRecords (async + try-catch)
10. ACCIONES      — buscar, nuevo, editar, importar, close, refresh
11. WATCHERS      — watch currentPage y recordsPerPage
12. LIFECYCLE     — onMounted con carga inicial
```

### 3. Reglas de UI
- **AppButton**: Para acciones principales (Nuevo, Importar). Botones de acción en tabla pueden ser `<button>`.
- **Filter**: Componente compartido en cada header de columna filtrable.
- **Paginator**: Componente compartido bajo la tabla.
- **Dark Mode**: Todas las clases con variantes `dark:`.
- **TypeScript estricto**: `<script setup lang="ts">`, todas las refs tipadas.

## Checklist de Calidad

- [ ] ¿Template tiene 3 secciones comentadas?
- [ ] ¿Script setup sigue los 12 pasos en orden?
- [ ] ¿Usa `AppButton` para acciones?
- [ ] ¿Usa `Filter` y `Paginator`?
- [ ] ¿Variables tipadas?
- [ ] ¿`<script setup lang="ts">`?
- [ ] ¿Soporte Dark Mode?
- [ ] ¿IDs `kebab-case`?

## Que Genera
Una vista `.vue` que orquesta componentes compartidos siguiendo el patrón 12/3.

## Referencias y Código Reutilizable
**DEBES leer estas referencias antes de generar código:**
- [VER PLANTILLA CRUD](./references/TEMPLATES_CRUD.md)
