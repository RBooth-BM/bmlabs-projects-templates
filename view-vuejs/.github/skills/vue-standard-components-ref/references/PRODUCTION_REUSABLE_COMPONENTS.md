# Referencia: Componentes Reutilizables Observados en Proyectos Productivos

## Objetivo
Documentar componentes compartidos repetidos en proyectos productivos de BM para acelerar adopción en nuevos templates sin imponer dependencias inexistentes.

## Fuentes consideradas
- `airexp-backoffice-app`
- `ssa-epp-backoffice-app`
- `polpaico-muestreo-app`

## Componentes detectados (candidatos)

### Core CRUD / Formularios
- `BackButton.vue`: navegación de retorno consistente.
- `FormActions.vue`: acciones de formulario (guardar/cancelar).
- `ListHeader.vue`: encabezado de listados con slots de acciones.
- `RequiredFieldsLegend.vue`: leyenda de campos obligatorios.

### Data / Tabla / Acciones
- `IconActions.vue`: acciones compactas por fila (editar/eliminar/ver/exportar).
- `ColumnSelectorModal.vue`: selección dinámica de columnas.
- `Filter.vue` y `Paginator.vue`: presentes en más de un proyecto.

### Modales y flujos auxiliares
- `BulkImportModal.vue`: importación masiva.
- `DocumentViewerModal.vue`: visualización de documentos.
- `ConfirmModal.vue`: confirmación de acciones críticas.
- `ToastNotification.vue`: feedback no bloqueante.

### Navegación y layout
- `TabNavigation.vue`: navegación por tabs dentro de vista.
- `PageHeader.vue`: encabezado de vistas.

## Regla de adopción en el template
1. Si el componente ya existe en `src/components/shared/`, reutilizarlo.
2. Si no existe, tratarlo como **opcional** y no bloquear generación.
3. Si la funcionalidad es requerida por el módulo, generarlo vía prompt de componente (`A`) manteniendo contrato TS estricto y dark mode.
4. No marcar componentes opcionales como obligatorios en prompts base.

## Recomendación para prompts/skills
- Priorizar componentes base obligatorios del template.
- Referenciar esta lista para mejoras incrementales cuando el proyecto destino ya tenga esos componentes.