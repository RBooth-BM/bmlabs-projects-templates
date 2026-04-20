# BM: Estándar de Gobernanza de Proyecto

## 1️⃣ Principios Rectores

### No Verboso, Pero Type-Safe
- ✅ **Prohibido `any`** en todo el código (TypeScript estricto)
- ✅ **Prompts compactos** — referencia a skills, no repetición
- ✅ **Swagger-first** — servicios validados contra OpenAPI
- ✅ **0 vulnerabilidades críticas** — `pnpm audit` antes de merge

---

## 2️⃣ Arquitectura: Swagger JSON → Full Stack

### Flujo Estándar (6 Pasos)

```mermaid
Swagger.json
    ↓
[1] Types (interfaces + Zod infer)
    ↓
[2] Validator (Zod schema, mensajes ES)
    ↓
[3] Service (clase estática, httpClient)
    ↓
[4] Composable (estado reactivo: loading, data, error)
    ↓
[5] Modal (form + BaseModal + validación)
    ↓
[6] View (patrón 12/3: Acciones + Datos + Modales)
```

**Ejecución**: Usar prompt `/L-GenerateFullStackWorkflow` (orquestador automático).

---

## 3️⃣ Estructura de Directorios

```
src/
  assets/                       ← Logos, imágenes
  components/
    shared/                     ← Componentes reutilizables (AppButton, BaseModal, Filter, etc.)
  composables/                  ← Lógica reactiva
    useGlobalSearch.ts
    use[Entity].ts
  config/
    api.config.ts               ← Configuración httpClient
  constants/                    ← Constantes globales
    action-keys.ts
    auth-error-codes.ts
    boolean.constants.ts
    status.constants.ts
    ...
  helpers/                      ← Funciones utilidad
  layout/
    DefaultLayout.vue
    components/
      AppHeader.vue
      AppSidebar.vue
  router/
    index.ts                    ← Rutas + guardias
  services/
    api/
      http-client.ts            ← Cliente Axios centralizado
      services/
        [entity].service.ts     ← Clase estática
  stores/                       ← Pinia stores
    auth.store.ts
    [entity].store.ts
  style/                        ← CSS global + Tailwind
  types/
    [entity].types.ts           ← DTOs + Interfaces
    models/                     ← Modelos de datos
    navigation/                 ← Tipos de navegación
  utils/                        ← Utilidades (date, validators, etc.)
  validators/                   ← Zod schemas
    [entity].validator.ts
  views/
    [entity]/
      [Entity]View.vue          ← Vista CRUD (patrón 12/3)
      components/
        CreateEdit[Entity]Modal.vue
```

**Ref**: Basado en estructura estándar de proyectos BM (agnóstico de cliente)

---

## 4️⃣ Nomenclatura

| Tipo | Convención | Ejemplo |
|------|-----------|---------|
| Componentes | **PascalCase** | `AppButton.vue`, `UserProfileCard.vue` |
| Variables, funciones | **camelCase** | `usersData`, `handleSubmit()` |
| Servicios | **camelCase** (clase estática) | `UserService.getAll()` |
| IDs HTML | **kebab-case** | `btn-save`, `user-name-input` |
| Archivos tipos | **[entity].types.ts** | `user.types.ts` |
| Archivos validador | **[entity].validator.ts** | `user.validator.ts` |

---

## 5️⃣ Reglas Inflexibles

### En `<script setup lang="ts">`
1. **Tipado explícito**: Todos los refs, props, emits con tipos
2. **Imports organizados** en 12 secciones (ver copilot-instructions.md)
3. **Prohibido `any`**: Siempre inferir tipo o declarar interfaz

### En API Services
1. **Clase estática** con método `private static readonly BASE_URL`
2. **httpClient centralizado** (nunca `import axios`)
3. **Métodos HTTP validados** contra OpenAPI (POST, PATCH, GET, DELETE)
4. **Respuesta tipada**: `httpClient.get<TipoRespuesta>(...)`

### En Componentes Compartidos
1. **Props interface** exportada
2. **Emits interface** exportada
3. **No elementos nativos** para interacciones (usar `AppButton`, `FormInput`, etc.)
4. **Dark mode**: Clases `dark:` en Tailwind obligatorias

### En Validadores
1. **Zod obligatorio** (no regex suelto)
2. **Mensajes en español**
3. **`superRefine`** para lógica condicional (ej: email si contiene `@`)
4. **`z.infer<typeof schema>`** para inferir tipo automático

---

## 6️⃣ Checklist de Calidad

Antes de mergear código nuevo:

- [ ] Correr `npm run build` (0 errores TS, 0 warnings)
- [ ] Correr `pnpm audit` (0 vulnerabilidades críticas)
- [ ] Revisar que no haya `any` (buscar: `any` en archivo)
- [ ] Validar tipos con `vue-tsc`
- [ ] ARIA labels en UI (accesibilidad WCAG)
- [ ] Dark mode en componentes nuevos (`dark:` classes)
- [ ] IDs `kebab-case` en elementos interactivos
- [ ] JSDoc en funciones públicas
- [ ] Componentes `@/components/shared/` usados (no `<button>` nativo)

---

## 7️⃣ Stack de Dependencias

### Frontend ✅
- **Vue 3** (^3.5.22) — Framework
- **TypeScript** (~5.9.0) — Type checking
- **Vite** (^7.1.7) — Build tool
- **Tailwind CSS** (^4.1.14) — Styling + Dark mode
- **Pinia** (^3.0.3) — State management
- **Vue Router** (^4.5.1) — Routing
- **Axios** (^1.13.2) — HTTP client
- **Zod** (^3.24.1) — Validation
- **Lucide Vue Next** (^0.546.0) — Icons
- **JWT Decode** (^4.0.0) — Auth token parsing

### Nota sobre exportación Excel
- La dependencia `xlsx` no viene preinstalada en el template base.
- Si un módulo la requiere, instalar desde un registro de paquetes verificado (o mirror interno), nunca desde URLs externas directas.
- Mantener la verificación de seguridad con `pnpm audit` antes de merge.

---

## 8️⃣ Consolidación

### Cambios Realizados

| Artefacto | Antes | Después | Ahorro |
|-----------|-------|---------|--------|
| B.-CreateApiService.prompt | 95 líneas (repite skill) | Restaurado con XML + tools | Sin cambio neto |
| L.-GenerateFullStackWorkflow | 68 líneas (repite reglas) | 20 líneas (solo flujo) | ~1.5K tokens |
| F.-CreateView.prompt | DEPRECATED | ✅ Reintroducido para vistas sin formulario | Claridad funcional |
| Referencias a clientes | Nombres de proyectos externos | Agnóstico | Claridad |
| Skills de uniformidad | No existían | ✨ 2 nuevos skills | Determinismo |

### Nuevos Skills (Phase 1 Mejorada)

✅ **vue-form-input-patterns** — Mapeo tipo-dato → componente (garantiza uniformidad)  
✅ **vue-component-taxonomy** — Clasificación jerárquica de componentes  
✅ **NAMING_PATTERNS.md** — Guía única de nomenclatura (agnóstica de cliente)

## 9️⃣ Invariantes

- **Type-safety**
- **No-verbose**
- **Swagger-first**
- **0-critical-vulns**
