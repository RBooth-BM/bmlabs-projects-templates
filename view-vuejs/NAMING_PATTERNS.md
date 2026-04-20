# BM: Guía de Nomenclatura y Patrones Uniformes

**Proyecto Agnóstico**: Esta guía aplica a **TODOS** los proyectos de BM. No contiene referencias a clientes específicos.

**Objetivo**: Garantizar uniformidad determinística — cuando generas un componente, formulario o vista 3 veces, las 3 veces deben ser idénticas.

---

## 🎯 Principios Fundamentales

1. **Agnóstico de cliente**: Sin referencias a proyectos externos ni empresas específicas
2. **Determinístico**: Misma solicitud = mismo resultado siempre
3. **Type-safe**: TypeScript estricto, prohibido `any`
4. **Un solo idioma en nomenclatura**: Identificadores, archivos y carpetas en ingles; comentarios y texto visible pueden estar en espanol
5. **No verboso**: Nombrar funciones, variables y componentes de forma clara pero concisa
6. **Predecible**: Quien lee el código sabe dónde buscar cada cosa

---

## 📂 Estructura de Directorios Estándar

```
src/
  ├── assets/                    ← Recursos estáticos (logos, imágenes)
  │   └── logo/                  ← Logos de marca (reemplazable por cliente)
  │
  ├── components/
  │   └── shared/                ← Componentes reutilizables globales (ver vue-component-taxonomy)
  │
  ├── composables/               ← Lógica reactiva (use[Entity].ts)
  │
  ├── config/
  │   └── api.config.ts          ← Configuración de APIs (baseURL, timeouts)
  │
  ├── constants/                 ← Constantes globales
  │   ├── action-keys.ts         ← Claves de acciones / permisos
  │   ├── status.constants.ts    ← Estados (ACTIVE, INACTIVE, etc.)
  │   ├── boolean.constants.ts   ← Booleanos corporativos
  │   └── index.ts               ← Exportación central
  │
  ├── helpers/                   ← Funciones utilidad puras
  │   ├── [feature]-input.ts     ← Helpers de input específicos (tax-id, phone, etc.)
  │   └── validators.ts          ← Utilidades de validación
  │
  ├── layout/                    ← Layouts principales
  │   ├── DefaultLayout.vue
  │   └── components/
  │       ├── AppHeader.vue
  │       ├── AppSidebar.vue
  │       └── UserCard.vue
  │
  ├── router/
  │   ├── index.ts               ← Definición de rutas + guards
  │   └── guards/
  │       └── auth.guard.ts      ← Guard de autenticación
  │
  ├── services/
  │   └── api/
  │       ├── http-client.ts     ← Cliente Axios centralizado
  │       └── services/
  │           └── [entity].service.ts  ← Clases estáticas por entidad
  │
  ├── stores/                    ← Pinia stores
  │   ├── auth.store.ts
  │   └── [entity].store.ts
  │
  ├── style/                     ← CSS global
  │   ├── custom.css
  │   └── style.css
  │
  ├── types/
  │   ├── api.types.ts           ← Tipos genéricos de API
  │   ├── models/
  │   │   └── [entity].types.ts  ← DTOs e interfaces de entidades
  │   └── navigation/            ← Tipos de navegación y rutas
  │
  ├── utils/                     ← Utilidades varias
  │   ├── [feature].utils.ts     ← Helpers de feature
  │   └── validators.ts          ← Funciones de validación (reutilizables sin Zod)
  │
  ├── validators/                ← Schemas Zod
  │   ├── [entity].validator.ts  ← Schema + type inference
  │   └── index.ts               ← Exportación central
  │
  ├── views/
  │   ├── auth/
  │   │   └── LoginView.vue
  │   ├── [entity]/
  │   │   ├── [Entity]View.vue             ← Vista principal (CRUD, patrón 12/3)
  │   │   └── components/
  │   │       └── CreateEdit[Entity]Modal.vue  ← Modal de formulario
  │   └── DashboardView.vue
  │
  ├── App.vue
  └── main.ts
```

---

## 🏷️ Convenciones de Nomenclatura

### 1. Archivos y Carpetas

| Tipo | Convención | Ejemplo | Ubicación |
|---|---|---|---|
| **Componentes Vue** | **PascalCase** | `AppButton.vue`, `FormInput.vue` | `src/components/shared/` |
| **Vistas / Pages** | **PascalCase + View** | `UsersView.vue`, `LoginView.vue` | `src/views/` |
| **Modales** | **PascalCase + Modal** | `CreateEditUserModal.vue` | `src/views/[entity]/components/` |
| **Servicios API** | **camelCase.service.ts** | `users.service.ts` | `src/services/api/services/` |
| **Stores Pinia** | **camelCase.store.ts** | `auth.store.ts` | `src/stores/` |
| **Composables** | **camelCase con `use`** | `useUsers.ts`, `usePagination.ts` | `src/composables/` |
| **Tipos/DTOs** | **camelCase.types.ts** | `user.types.ts`, `api.types.ts` | `src/types/` o `src/types/models/` |
| **Validadores Zod** | **camelCase.validator.ts** | `user.validator.ts` | `src/validators/` |
| **Constantes** | **SCREAMING_SNAKE_CASE** en código, **camelCase.constants.ts** archivo | `STATUS_ACTIVE`, `ACTION_KEYS` | `src/constants/` |
| **Helpers/Utils** | **camelCase.ts** | `tax-id-input.ts`, `validators.ts` | `src/helpers/` o `src/utils/` |
| **Carpetas** | **kebab-case** | `src/components/shared/`, `src/types/models/` | Cualquier ubicación |

### 2. Variables y Funciones

| Contexto | Convención | Ejemplo | Nota |
|---|---|---|---|
| **Variables** | camelCase | `userName`, `isActive`, `userList` | Predecibles y descriptivas |
| **Funciones** | camelCase | `validateForm()`, `handleSubmit()`, `fetchUsers()` | Verbos claros: fetch, validate, handle |
| **Booleanos** | `is` o `has` + descriptor | `isActive`, `hasPermission`, `isLoading` | Claro que es booleano |
| **Arrays** | plural | `users`, `items`, `errors` | O sufijo `List`: `userList` |
| **Refs (Vue)** | camelCase | `loading`, `items`, `selectedId` | Sin prefijo especial |
| **Objetos reactivos** | camelCase | `form`, `filters`, `pagination` | Claro contexto |
| **Constantes** | SCREAMING_SNAKE_CASE | `BASE_URL`, `DEFAULT_TIMEOUT`, `MAX_RETRIES` | Inmutables en tiempo de ejecución |

### 3. Componentes Vue

#### Props
```typescript
// ✅ CORRECTO
interface Props {
  label: string
  isLoading?: boolean
  errorMessage?: string
  modelValue: string | number
}

// ❌ EVITAR
interface Props {
  lbl: string              // abreviatura confusa
  loading: boolean         // sin prefijo is/has
  err: string             // abreviatura
  value: string           // ambiguo
}
```

#### Emits
```typescript
// ✅ CORRECTO
const emit = defineEmits<{
  'update:modelValue': [value: string]
  'blur': []
  'error': [message: string]
}>()

// ❌ EVITAR
const emit = defineEmits<{
  'change': [value: string]    // genérico
  'on-blur': []                // prefijo on
  '@error': [message: string]  // @ en evento
}>()
```

### 4. HTML IDs y Clases CSS

| Tipo | Convención | Ejemplo |
|---|---|---|
| **IDs** | kebab-case | `id="btn-save-user"`, `id="form-login"` |
| **Classes** | kebab-case | `class="section-header"`, `class="card-container"` |
| **Data Attributes** | kebab-case | `data-test-id="user-list"` |

```vue
<!-- ✅ CORRECTO -->
<button id="btn-submit" class="action-button" data-test-id="submit-btn">Guardar</button>

<!-- ❌ EVITAR -->
<button id="btnSubmit" class="actionButton" data-test-id="submitBtn">Guardar</button>
```

---

## 🧩 Patrones de Uniformidad Garantizada

### Patrón 1: Formularios

**Garantía**: 3 invocaciones = 3 formularios idénticos en estructura y componentes.

**Archivos**:
- `src/types/models/[entity].types.ts` — DTOs
- `src/validators/[entity].validator.ts` — Zod schema
- `src/views/[entity]/components/CreateEdit[Entity]Modal.vue` — Formulario

**Nomenclatura**:
- Variable formulario: `form` (never `userData`, `userForm`, `formData`)
- Variable errores: `errors` (typed como `Partial<Record<keyof typeof form, string>>`)
- Función validación: `validateField(field: keyof typeof form)`
- Función submit: `handleSubmit()`

**Componentes usados** (ver `vue-form-input-patterns`):
- Texto corto → `FormInput type="text"`
- Email → `FormInput type="email"`
- Número → `FormInput type="number"`
- Texto largo → `FormTextarea`
- Select → `FormSelect`
- Toggle → `AppSwitch`

**Ejemplo**:
```vue
<!-- Primera, segunda y tercera invocación = idéntico -->
<FormInput
  v-model="form.email"
  label="Correo"
  type="email"
  :error="!!errors.email"
  :error-message="errors.email"
  @blur="validateField('email')"
/>
```

### Patrón 2: Servicios API

**Garantía**: 3 servicios de entidades diferentes = 3 clases idénticas en estructura.

**Archivo**: `src/services/api/services/[entity].service.ts`

**Estructura**:
```typescript
export class [Entity]Service {
  private static readonly BASE_URL = '/[endpoint]'
  
  static async get(id: string): Promise<[Entity]> {
    return httpClient.get<[Entity]>(`${this.BASE_URL}/${id}`)
  }
  
  static async list(filters?: Filters): Promise<[Entity][]> {
    return httpClient.get<[Entity][]>(this.BASE_URL, { params: filters })
  }
  
  static async create(data: Create[Entity]DTO): Promise<[Entity]> {
    return httpClient.post<[Entity]>(this.BASE_URL, data)
  }
  
  static async update(id: string, data: Update[Entity]DTO): Promise<[Entity]> {
    return httpClient.patch<[Entity]>(`${this.BASE_URL}/${id}`, data)
  }
  
  static async delete(id: string): Promise<void> {
    return httpClient.delete(`${this.BASE_URL}/${id}`)
  }
}
```

### Patrón 3: Composables

**Garantía**: 3 composables = 3 estructuras idénticas con `loading`, `error`, `items`.

**Archivo**: `src/composables/use[Entity].ts`

**Estructura**:
```typescript
export function use[Entity]() {
  // Estado base
  const loading = ref(false)
  const error = ref<string | null>(null)
  const items = ref<[Entity][]>([])
  
  // Acciones
  const fetchItems = async () => { /* ... */ }
  const create = async (data: Create[Entity]DTO) => { /* ... */ }
  const update = async (id: string, data: Update[Entity]DTO) => { /* ... */ }
  const delete = async (id: string) => { /* ... */ }
  
  // Retorno
  return {
    loading: readonly(loading),
    error: readonly(error),
    items: readonly(items),
    fetchItems,
    create,
    update,
    delete,
  }
}
```

---

## 🚫 Violaciones Comunes

| Violación | ❌ Evitar | ✅ Hacer |
|---|---|---|
| Referencias de clientes | Nombres de proyectos externos o de clientes | Código agnóstico, comentarios genéricos |
| Idioma mezclado en nombres | Combinaciones híbridas entre ingles y espanol en un mismo identificador | Identificadores completos en ingles, por ejemplo `CreateEditUserModal`, `userList`, `btn-save-user` |
| Variabilidad en formularios | Email con `<input>` vs `FormInput` | Siempre `FormInput type="email"` |
| Nombres genéricos | `Component.vue`, `data`, `handler` | `FormInput.vue`, `userData`, `handleSubmit()` |
| CamelCase en IDs HTML | `id="btnSave"` | `id="btn-save"` |
| Mezcla de patrones | Service estático + objeto const | Siempre clase estática con `static async` |
| Sin validadores centralizados | Regex suelto en componente | Zod schema en `validators/` |
| Props inconsistentes | `label` en un componente, `title` en otro | `label` siempre (ver `vue-component-taxonomy`) |
| `any` en tipos | `const data: any = {...}` | `const data: EntityDTO = {...}` |

---

## 📋 Checklist de Nomenclatura Antes de Mergear

- [ ] ¿Cero referencias a clientes específicos o nombres de proyectos externos?
- [ ] ¿Variables, archivos y carpetas están en ingles sin mezclar idiomas?
- [ ] ¿Componentes Vue en PascalCase?
- [ ] ¿Servicios en camelCase.service.ts?
- [ ] ¿Composables con prefijo `use`?
- [ ] ¿Variables booleanas con `is`/`has`?
- [ ] ¿IDs HTML en kebab-case?
- [ ] ¿Cero `any` en TypeScript?
- [ ] ¿Funciones con verbos claros (fetch, validate, handle)?
- [ ] ¿Arrays en plural?
- [ ] ¿Constantes en SCREAMING_SNAKE_CASE?

---

## 🔗 Referencias Relacionadas

- **Skill**: `vue-form-input-patterns` — Mapeo tipo-dato → componente
- **Skill**: `vue-component-taxonomy` — Clasificación de componentes
- **Skill**: `vue-project-naming-structure` — Estructura de carpetas
- **Doc**: `GOVERNANCE.md` — Estándares del proyecto
