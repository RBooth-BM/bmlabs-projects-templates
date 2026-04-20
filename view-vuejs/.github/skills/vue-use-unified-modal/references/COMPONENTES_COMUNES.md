# Catalogo Comun Compacto (BM)

## Objetivo
Definir un set minimo de componentes compartidos para reducir ambiguedad y tokens.

## Componentes Base
- AppButton: acciones primarias/secundarias.
- FormInput: campos de formulario.
- AppBreadcrumb: breadcrumb en vistas.
- Filter: filtro por columna en tablas.
- Paginator: paginacion de listados.
- **AppModal**: modal moderno (USAR ESTE para nuevos desarrollos). `Teleport to="#modals"`, `rounded-3xl`, `backdrop-blur-sm`.
- BaseModal: modal legacy (sigue disponible pero AppModal es el estándar actual).
- ConfirmModal: confirmaciones. Usa `AppModal` internamente con `hide-header`. Prop `variant` (info|success|warning|danger|question).

## Contrato de Sidebar Estandar
- Archivo: src/layout/components/AppSidebar.vue
- Props minimas:
  - open: boolean
  - collapsed: boolean
  - navigationItems: NavigationItem[]
- Emits minimos:
  - update:open
  - update:collapsed
  - open-search
  - logout
- Estructura minima:
  - Header/brand
  - Trigger de busqueda global (Cmd/Ctrl+K)
  - Navegacion principal (estado activo)
  - UserCard y logout al pie
  - Overlay mobile

## Contrato de DefaultLayout Estandar
- Archivo: src/layout/DefaultLayout.vue
- Debe incluir:
  - AppSidebar
  - RouterView
  - AppBreadcrumb (en las vistas)
  - Integracion con GlobalSearch
- Reglas:
  - Ajustar margen/padding segun estado rail del sidebar
  - Mantener responsive y dark mode

## Regla de Reutilizacion
- Si el proyecto ya contiene AppSidebar/DefaultLayout/GlobalSearch, reutilizar y ajustar.
- Evitar crear variantes nuevas sin requerimiento explicito.

## Regla de Tokens
- No leer catalogos extensos si la tarea es puntual.
- Leer solo archivos necesarios para la tarea actual.
