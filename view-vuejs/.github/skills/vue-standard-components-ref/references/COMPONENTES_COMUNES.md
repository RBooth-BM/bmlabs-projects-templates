# Catalogo Comun Compacto (BM)

## Objetivo
Definir un set minimo de componentes compartidos para reducir ambiguedad y tokens.

## Componentes Base
- AppButton: acciones primarias/secundarias.
- FormInput: campos de formulario con validación.
- FormSelect: select de formulario con opciones flexibles.
- FormTextarea: textarea de formulario.
- FormActions: wrapper de formulario con acciones Guardar/Volver.
- AppBreadcrumb: breadcrumb en vistas. Obligatorio en todas las vistas de `src/views/`.
- Filter: filtro por columna en tablas HTML manuales.
- Paginator: paginación de listados (usado junto a tablas HTML manuales o como complemento).
- **DataTable**: `@/components/shared/datatable/DataTable.vue` — tabla avanzada con filtros por columna, sorting, paginación y acciones. USAR para listados CRUD principales.
- **AppModal**: modal moderno principal (`Teleport to="#modals"`, `rounded-3xl`).
- BaseModal: modal legacy (disponible, pero AppModal es el estándar actual).
- ConfirmModal: confirmaciones y acciones de riesgo. Prop `variant` (info|success|warning|danger|question).
- PageHeader: encabezado de vista con título, subtítulo y slot `#extra`.
- AppSwitch: toggle booleano para estados activo/inactivo.
- ShowcaseView: Vista de referencia canónica (ubicada en `src/views/ShowcaseView.vue` y accesible en `/showcase`). REVISAR SIEMPRE PARA IMPLEMENTACIONES DE ALTA FIDELIDAD.

## Contrato de Sidebar Estandar
- Archivo: `src/layout/components/AppSidebar.vue`
- Referencias reales disponibles (ejemplos):
	- `references/AppSidebar_PRODUCTION_REFERENCE.vue`
- Requisitos mínimos comunes:
	- Rail mode (expandido/colapsado)
	- Navegación principal con estado activo
	- Trigger de búsqueda global (`open-search` o equivalente)
	- Bloque de usuario + cerrar sesión
	- Overlay mobile

## Contrato de DefaultLayout Estandar
- Archivo: `src/layout/DefaultLayout.vue`
- Referencias reales disponibles (ejemplos):
	- `references/DefaultLayout_PRODUCTION_REFERENCE.vue`
- Requisitos mínimos comunes:
	- Integración con `AppSidebar`
	- `RouterView` como contenido principal
	- Soporte responsive (móvil/desktop)
	- Integración con búsqueda global si existe componente (`GlobalSearch`)

## Regla de Reutilizacion
- Si el proyecto ya contiene AppSidebar/DefaultLayout/GlobalSearch, reutilizar y ajustar.
- Evitar crear variantes nuevas sin requerimiento explicito.
- El resultado debe mantenerse como estándar corporativo único y reusable.

## Componentes opcionales de adopcion incremental
- Existen componentes reutilizables observados en proyectos productivos que pueden incorporarse por necesidad de módulo.
- Ver referencia: `references/PRODUCTION_REUSABLE_COMPONENTS.md`.
- Regla: si no existen en `src/components/shared/`, no hacerlos obligatorios en prompts base.

## Regla de Tokens
- No leer catalogos extensos si la tarea es puntual.
- Leer solo archivos necesarios para la tarea actual.
