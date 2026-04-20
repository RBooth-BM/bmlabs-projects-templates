# Prompts, Skills y Agentes

Guía para usar los prompts, skills y agentes disponibles en este template Vue 3.

## Quick Start

### 1. Acceso a Prompts en Copilot

Todos los prompts están disponibles en `@prompt` dentro de GitHub Copilot Chat:

```
@prompt M.-SetupProjectBase
@prompt A.-CreateComponent
@prompt B.-CreateApiService
...
```

Consulta `prompts/README.md` para la lista completa y propósito de cada uno.

### 2. Uso de Skills

Los skills contienen estándares, patrones y referencias. Están diseñados para ser leídos automáticamente por los agentes antes de generar código:

- **`vue-component-taxonomy`**: Clasificación de componentes por tipo.
- **`vue-create-api-service`**: Patrón para servicios API estáticos tipados.
- **`vue-master-crud-standard`**: Estándar 12/3 para vistas CRUD.
- **`vue-form-input-patterns`**: Mapeo tipo de dato → componente de input.
- **`vue-protected-routes`**: Guardias de navegación con autenticación.

Cada skill tiene referencias en `skills/[skill-name]/references/` con ejemplos reales.

### 3. Agentes Autónomos

Los agentes orquestan la ejecución de múltiples skills y prompts:

- **`BM Builder`**: Orquesta generación full-stack Vue 3 con validación y cierre end-to-end.
  - Input: Entidad, flujo o tarea (ej: "CRUD de Invoices desde swagger.json")
  - Output: Stack completo (types → validator → service → composable → view)

- **`Explore`**: Exploración rápida de codebase.
  - Input: Descripción de qué buscar + (quick/medium/thorough)
  - Output: Archivos relevantes y snippets

## Estándares Clave

### Nomenclatura
- **Archivos y carpetas**: `kebab-case` para archivos; `camelCase` para funciones/variables
- **Identificadores**: Únicamente en **inglés** (variables, funciones, tipos, clases, componentes, archivos, carpetas)
- **Comentarios y UI**: Español permitido

Ver [`NAMING_PATTERNS.md`](../NAMING_PATTERNS.md)

### Type-Safety
- Prohibido `any`
- Props & Emits tipados explícitamente
- Validación con Zod obligatoria

Ver [`copilot-instructions.md`](./copilot-instructions.md)

### Componentes
- Componentes compartidos en `src/components/shared/`
- Usar `<script setup lang="ts">` siempre
- Soportar dark mode: clases `dark:`

### Servicios API
- Clase estática con métodos `static async`
- Tipado: `httpClient<ResponseType>()`
- Nunca `axios` directo

Ver [`skills/vue-create-api-service/SKILL.md`](./skills/vue-create-api-service/SKILL.md)

### CRUD (Patrón 12/3)
- **SECCIÓN 1**: Acciones (botones)
- **SECCIÓN 2**: Datos (tabla, filtros, paginador)
- **SECCIÓN 3**: Modales (formulario, confirmación)

Script setup organizado en 12 bloques estándar.

Ver [`skills/vue-master-crud-standard/SKILL.md`](./skills/vue-master-crud-standard/SKILL.md)

## Flujo Recomendado: Swagger → Full Stack

1. Proporcionar especificación OpenAPI 3.0
2. Ejecutar prompt `@prompt Z.-GenerateFromSwagger`
3. Resultado: 6 archivos listos:
   - `types/[entity].types.ts`
   - `validators/[entity].validator.ts`
   - `services/api/services/[entity].service.ts`
   - `composables/use[Entity].ts`
   - `views/.../Create[Entity]Modal.vue`
   - `views/.../[Entity]View.vue`

Ver [`prompts/Z.-GenerateFromSwagger.prompt.md`](./prompts/Z.-GenerateFromSwagger.prompt.md)

## Referencia Rápida de Archivos

### Prompts
```
prompts/
  A.-CreateComponent.prompt.md       → Componente Vue reutilizable
  B.-CreateApiService.prompt.md      → Servicio API estático
  C.-CreateDataGrid.prompt.md        → Tabla de datos avanzada
  D.-CreateComposable.prompt.md      → Composable (estado reactivo)
  E.-CreateLayout.prompt.md          → Layout wrapper
  F.-CreateView.prompt.md            → Vista sin formulario
  G.-CreateValidator.prompt.md       → Schema Zod
  H.-CreateForm.prompt.md            → Formulario Modal
  I.-CreateAuthStore.prompt.md       → Pinia store de autenticación
  J.-CreateAuthViews.prompt.md       → Vistas de login/registro
  K.-CreateProtectedRoutes.prompt.md → Guardias y rutas protegidas
  L.-GenerateFullStackWorkflow.prompt.md → Orquesta 6 prompts en orden
  M.-SetupProjectBase.prompt.md      → Bootstrap completo del proyecto
  N.-CreateExportAction.prompt.md    → Exportación a Excel/CSV
  Z.-GenerateFromSwagger.prompt.md   → Stack completo desde OpenAPI
```

### Skills
```
skills/
  vue-component-taxonomy/            → Clasificación de componentes
  vue-create-api-service/            → Patrón servicios API
  vue-create-composables/            → Composables y estado
  vue-create-datagrid/               → DataTable avanzada
  vue-master-crud-standard/          → Patrón CRUD 12/3
  vue-form-input-patterns/           → Tipo dato → componente
  vue-protected-routes/              → Guardias + autenticación
  vue-standard-components-ref/       → Componentes compartidos
  ...y más
```

### Documentación Central
```
.github/
  GOVERNANCE.md           → Estándares del proyecto
  NAMING_PATTERNS.md      → Convenciones de naming
  AGENTS.md               → Definición de agentes
  copilot-instructions.md → Instrucciones para Copilot
  prompts/README.md       → Guía de prompts detallada
```

## Checklist Antes de Generar Código

- [ ] He leído la skill aplicable
- [ ] Tengo la fuente de verdad (Swagger, referencia, etc.)
- [ ] Entiendo el patrón esperado
- [ ] 0 `any` en el código generado
- [ ] Naming en inglés (identificadores)
- [ ] Dark mode soportado (si es UI)
- [ ] Sin referencias de cliente externo

## Troubleshooting

**Q: Un prompt no genera el código que espero**
A: Verifica que el prompt esté leyendo la skill correcta. Abre `skills/[nombre]/SKILL.md` y revisa referencias.

**Q: ¿Por qué se prohibe `any`?**
A: TypeScript estricto garantiza seguridad en refactores masivos y mejor DX.

**Q: ¿Cómo fuerzo dark mode en un componente?**
A: Añade clases `dark:` en el template. Ejemplo: `class="bg-white dark:bg-gray-900"`.

**Q: ¿Puedo usar Axios directamente?**
A: No. Usa `httpClient` desde `src/services/api/http-client.ts`. Es centralizado y tipado.

## Contacto & Mejoras

Para sugerir cambios a skills, prompts o estándares:
1. Abre issue describiendo el cambio
2. Proporciona referencia real o patrón mejorado
3. Valida con `npm run build` (0 errores TS)

---

**Última actualización**: Abril 2026
**Versión**: 1.0 (Consolidada)
