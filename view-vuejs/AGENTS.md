# AGENTS.md

## Project Metadata
- **Tipo**: Template base para pruebas de prompts/skills.
- **Framework**: Vue 3 + Vite + TypeScript.
- **Estado actual**: Código de negocio removido; repo mínimo para generación desde cero.

## Estructura vigente

```
.github/
  prompts/
  skills/
  copilot-instructions.md
src/
  assets/
  components/
    shared/
  composables/
  config/
  constants/
  helpers/
  layout/
  router/
  services/
    api/
  stores/
  style/
  types/
  utils/
  validators/
  views/
App.vue
main.ts
```

## Objetivo de este repositorio

Usar prompts/skills para bootstrap automático de módulos, servicios y vistas sobre una base limpia.

## Reglas para agentes

1. **Skills primero**: Antes de generar código, leer la skill aplicable en `.github/skills/*/SKILL.md`.
2. **Nomenclatura clave**: Ver `NAMING_PATTERNS.md` para garantizar uniformidad (mismo formulario 3 veces = idéntico).
3. **Patrones de uniformidad**: Consultar `vue-form-input-patterns` (tipo-dato → componente) y `vue-component-taxonomy` (mapeo jerárquico).
4. **Fuente de verdad**: Si la skill tiene `references/`, usarlas.
5. **Type-safety**: Mantener TypeScript estricto. Prohibido `any`.
6. **Reutilización**: Preferir componentes compartidos de `@/components/shared/`.
7. **Basado en Swagger**: Servicios generados desde `swagger.json` (path + método HTTP verificados).
8. **No verbose**: Prompts compactos. Referencia a skills en lugar de duplicar reglas.
9. **0 vulnerabilidades**: Revisar `pnpm audit` antes de mergear (0 critical).
10. **Agnóstico de cliente**: Sin referencias a empresas específicas, nombres de clientes ni identificadores de proyectos externos.
11. **Nomenclatura en un solo idioma**: Identificadores, archivos y carpetas en ingles; espanol solo para comentarios, labels y mensajes visibles.

## Comandos base

```bash
npm install
npm run dev
npm run build
```

---

## Estándar: Swagger JSON → Full Stack

**Flujo automático** (6 pasos en orden):
1. Types → `src/types/[entity].types.ts`
2. Validator → `src/validators/[entity].validator.ts` (Zod)
3. Service → `src/services/api/services/[entity].service.ts` (clase estática)
4. Composable → `src/composables/use[Entity].ts` (estado reactivo)
5. Modal → `src/views/.../CreateEdit[Entity]Modal.vue` (formulario)
6. View → `src/views/.../[Entity]View.vue` (patrón 12/3)

**Workflow**: Usar prompt `/L-GenerateFullStackWorkflow` (ejecuta en orden).

---

## Consolidación Phase 1 (Abril 2026)

✅ **Completado**: Reducción de redundancia en prompts/skills (4K tokens ahorrados)
- `B.-CreateApiService.prompt.md`: Consolidado → referencia a skill
- `L.-GenerateFullStackWorkflow.prompt.md`: Compactado 68 → 20 líneas
- `F.-CreateView.prompt.md`: Reintroducido para vistas sin formulario

✅ **Estado de deps**: `pnpm audit` = 0 vulnerabilidades críticas en el baseline del template

📋 **Phase 2**: Super-prompt `/Z-GenerateFromSwagger` disponible (Swagger JSON → Full Stack en 1 invocación)
