# Prompts: Generación Guiada by Skills

**Filosofía**: No verbose. Cada prompt referencia una skill que contiene las reglas detalladas.

## Quick Start: Generar un CRUD Completo

```bash
# Invoca el orquestador (6 pasos automáticos)
/L-GenerateFullStackWorkflow <EntityName>

# Ejemplo: Generar CRUD de Usuarios
/L-GenerateFullStackWorkflow Usuario
```

**Flujo** (cada paso tiene su skill):
1. `/A` → Types
2. `/G` → Validator (Zod)
3. `/B` → Service (clase estática)
4. `/D` → Composable (estado reactivo)
5. `/H` → Modal (formulario)
6. `/C` → View (patrón 12/3)

---

## Prompts Disponibles

| ID | Nombre | Propósito |
|----|--------|----------|
| A | CreateComponent | Componentes reutilizables |
| B | CreateApiService | **→ Ver skill** (consolidado) |
| C | CreateDataGrid | Vista CRUD (patrón 12/3) |
| D | CreateComposable | Composables (estado reactivo) |
| E | CreateLayout | Layouts |
| F | CreateView | Vista sin formulario (read-only/dashboard/detalle) |
| G | CreateValidator | Zod schemas |
| H | CreateForm | Formularios modales |
| I | CreateAuthStore | Stores Pinia |
| J | CreateAuthViews | Vistas autenticación |
| K | CreateProtectedRoutes | Guardias Vue Router |
| L | GenerateFullStackWorkflow | **Orquestador** (6 pasos) |
| M | SetupProjectBase | Setup inicial |
| N | CreateExportAction | Exportar datos |
| O | ChangeProjectColors | **Cambiar colores del proyecto** |
| Z | GenerateFromSwagger | **Generar stack completo desde Swagger** |

---

## Regla de Oro

> **No hagas preguntas.** El prompt y su skill contienen toda la información.
> Si algo no está claro, lee la skill del paso.

---

## Consolidación Phase 1 (Abril 2026)

✅ Reintroducido: `F.-CreateView` (vista sin formulario)  
✅ Compactado: `L.-GenerateFullStackWorkflow` (68 → 20 líneas)  
✅ Consolidado: `B.-CreateApiService` (referencia única a skill)  

**Próximo**: Super-prompt `/Z-GenerateFromSwagger` (Swagger JSON → Full Stack en 1 invocación)
