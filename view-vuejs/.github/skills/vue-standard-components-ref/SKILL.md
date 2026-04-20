---
name: vue-standard-components-ref
description: Referencia compacta de componentes compartidos y contrato estándar de Sidebar/Layout BM.
license: Proprietary
compatibility: Vue 3, TypeScript, Tailwind CSS
metadata:
  author: Ricardo B.T.
  version: 3.0.0
  standard: BM Shared Components
---

# Skill: Componentes Compartidos y Layout Estándar

## Propósito
Proveer una guía mínima y estable para reutilizar componentes compartidos y generar Sidebar/Layout consistentes entre proyectos.

## Invocación
Esta skill se activa automáticamente cuando cualquier otro prompt necesita componentes de UI.

## Catálogo mínimo permitido

| Componente | Archivo | Propósito |
|---|---|---|
| `AppButton` | `@/components/shared/AppButton.vue` | Botón universal |
| `FormInput` | `@/components/shared/FormInput.vue` | Input de formularios |
| `FormSelect` | `@/components/shared/FormSelect.vue` | Select de formularios |
| `FormTextarea` | `@/components/shared/FormTextarea.vue` | Textarea de formularios |
| `FormActions` | `@/components/shared/FormActions.vue` | Wrapper con Guardar/Volver |
| `AppBreadcrumb` | `@/components/shared/AppBreadcrumb.vue` | Breadcrumb de vistas |
| `Filter` | `@/components/shared/Filter.vue` | Filtro en tablas |
| `Paginator` | `@/components/shared/Paginator.vue` | Paginación |
| `AppModal` | `@/components/shared/AppModal.vue` | Modal principal (moderno) |
| `BaseModal` | `@/components/shared/BaseModal.vue` | Modal legacy |
| `ConfirmModal` | `@/components/shared/ConfirmModal.vue` | Confirmaciones |
| `PageHeader` | `@/components/shared/PageHeader.vue` | Encabezado de vista |
| `AppSwitch` | `@/components/shared/AppSwitch.vue` | Toggle booleano |
| `DataTable` | `@/components/shared/datatable/DataTable.vue` | Tabla avanzada con filtros y acciones |
| `AppExportActions` | `@/components/shared/datatable/AppExportActions.vue` | Exportación a Excel |

## Sidebar Mobile Contract

### Estructura Obligatoria
- **Botón Hamburguesa**: Visible solo en mobile (`md:hidden`) en header bar
- **Z-Index Dinámico**: Header bar `z-[60]` cuando sidebar abierto en mobile, `z-40` normalmente
- **Logo Siempre Visible**: Logo BM LABS visible en todos los modos del sidebar (expandido y colapsado) y en el header de desktop
- **Carpeta de Logo Predeterminada**: `src/assets/logo/` debe existir para logos de imagen y ser reemplazable por el deploy o el desarrollador
- **Overlay**: Backdrop oscuro cuando sidebar está abierto en mobile
- **Auto-cierre**: Sidebar se cierra automáticamente al navegar en mobile
- **Estado Controlado**: Layout padre controla `isOpen` del sidebar

### Props Contract
```typescript
interface AppSidebarProps {
  navigationItems: NavigationItem[]
  userName?: string
  isOpen?: boolean  // Controlado por layout padre
}
```

### Mobile Behavior
- Sidebar oculto por defecto (`-translate-x-full`)
- Se muestra con `translate-x-0` cuando `isOpen = true`
- Overlay click cierra sidebar
- Navegación cierra sidebar automáticamente
- **Botón colapsar oculto en mobile** (`hidden md:flex`)
- Solo botón hamburguesa controla apertura en mobile

### Checklist Mobile
- [ ] Botón hamburguesa visible solo en mobile
- [ ] Overlay backdrop cuando sidebar abierto
- [ ] Auto-cierre al navegar
- [ ] Estado `isOpen` controlado por layout
- [ ] Sidebar responsive con `isDesktop` detection
- [ ] Botón colapsar oculto en mobile (`hidden md:flex`)
- [ ] Z-index dinámico del header bar (`z-[60]` cuando sidebar abierto)
- [ ] Logo siempre visible en todos los modos

## Checklist Operativo de Layout
- [ ] `DefaultLayout` queda montado en una ruta real de `src/router/index.ts`.
- [ ] Sidebar incluye botón hamburguesa para mobile.
- [ ] Sidebar tiene overlay backdrop en mobile.
- [ ] Navegación cierra sidebar automáticamente en mobile.
- [ ] Estado sidebar controlado por layout padre.
- [ ] Existe una vista navegable para validar layout (`HomeView` fallback si no hay otra).
- [ ] El sidebar incluye una entrada que navega a esa vista.
- [ ] La implementación compila con `npm run build`.
- [ ] Sidebar/Layout alineados con referencia oficial corporativa, sin cambios arbitrarios de diseño.

## Modo Low-Token
- Leer solo referencias necesarias para la tarea.
- Si la tarea es layout/sidebar: leer solo `AppSidebar_PRODUCTION_REFERENCE.vue` y `DefaultLayout_PRODUCTION_REFERENCE.vue`.
- Evitar copiar código completo de catálogos largos cuando se pueda reutilizar archivos existentes del proyecto.

## Referencias y Código Reutilizable
**Referencias compactas obligatorias (orden de prioridad):**
- [Sidebar referencia oficial actual](./references/AppSidebar_PRODUCTION_REFERENCE.vue)
- [DefaultLayout referencia oficial actual](./references/DefaultLayout_PRODUCTION_REFERENCE.vue)
- [Catálogo común compacto](./references/COMPONENTES_COMUNES.md)
- [Candidatos reutilizables observados en producción](./references/PRODUCTION_REUSABLE_COMPONENTS.md)

Nota: estas referencias son ejemplos de implementación real para extraer patrones comunes. No deben convertirse en selector de perfil por cliente.
