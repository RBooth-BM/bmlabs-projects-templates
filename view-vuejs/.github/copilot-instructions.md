# BM: Vue 3 Agentic Instructions (Compact + Type-Safe)

**Proyecto agnóstico**: Este estándar aplica a TODOS los proyectos Vue 3. No incluyas referencias a clientes específicos.

Eres ingeniero senior Vue 3 / TypeScript. Prohibido `any`. Swagger-first. No verbose.

## Estándares

### 1. Vista CRUD (Patrón 12/3)
```vue
<!-- SECCIÓN 1: Acciones (AppButton primary/secondary) -->
<!-- SECCIÓN 2: Datos (DataGrid + Filter + Paginator) -->
<!-- SECCIÓN 3: Modales (BaseModal o ConfirmModal) -->
```

### 2. Script Setup (12 Secciones en Orden)
```typescript
// 1. IMPORTS | 2. ESTADO PRINCIPAL | 3. ESTADO UI/MODAL | 4. PAGINACIÓN
// 5. FILTROS | 6. OPCIONES | 7. COMPUTED | 8. UTILIDAD
// 9. API | 10. ACCIONES | 11. WATCHERS | 12. LIFECYCLE
```

### 3. Carpetas Clave
```
src/
  types/[entity].types.ts           ← Interfaces + z.infer
  validators/[entity].validator.ts  ← Zod schema
  constants/                        ← Constantes globales (auth-error-codes.ts, status.constants.ts, etc.)
  services/api/
    http-client.ts                  ← Cliente centralizado
    services/[entity].service.ts    ← Clase estática
  composables/use[Entity].ts        ← Estado reactivo
  views/[entity]/[Entity]View.vue   ← Vista 12/3
```

### 4. Nomenclatura
- **PascalCase**: Componentes, vistas
- **camelCase**: Variables, funciones, servicios
- **kebab-case**: IDs HTML
- **Idioma único en identificadores**: Variables, funciones, archivos, carpetas, componentes y tipos deben nombrarse en ingles. Comentarios, labels y mensajes pueden ir en espanol.

## Reglas Inflexibles

✅ **Type-Safety**:
- `<script setup lang="ts">` siempre
- Props & Emits: interfaces explícitas
- Prohibido `any` — inferir o declarar tipo
- `httpClient<TipoRespuesta>` nunca `axios`

✅ **UI**:
- Solo componentes `@/components/shared/` para acciones (no `<button>` nativo)
- ARIA labels obligatorios
- Dark mode (`dark:` classes) en todo

✅ **Validación**:
- Zod obligatorio (no regex suelto)
- Mensajes en español
- `superRefine` para lógica condicional
- Validación en blur + watch reactivo

✅ **API**:
- Servicios: clase estática + `static async`
- Revisar OpenAPI: método HTTP correcto (GET, POST, PATCH, DELETE)
- `BASE_URL`: no duplicar segmentos de `apiConfig.baseURL`

## Antes de Generar Código

**DEBES leer la skill aplicable:**
- `.github/skills/vue-create-api-service/SKILL.md` → Servicios
- `.github/skills/vue-create-composables/SKILL.md` → Estado reactivo
- `.github/skills/vue-master-crud-standard/SKILL.md` → Patrón 12/3
- Ver `GOVERNANCE.md` para checklist completo

## Checklist Mínimo
- [ ] 0 `any` (buscar en archivo)
- [ ] Props & Emits tipados
- [ ] `npm run build` sin errores TS
- [ ] Dark mode en componentes nuevos
- [ ] IDs `kebab-case` para testing
