---
name: vue-form-input-patterns
description: Define patrones uniformes para inputs de formularios basados en tipos de datos. Garantiza consistencia en 3 invocaciones del mismo formulario.
license: Proprietary
compatibility: Vue 3, Zod, FormInput, FormSelect, FormTextarea
metadata:
  author: BM
  version: 1.0.0
  objective: Asegurar que el generador de formularios SIEMPRE produce el mismo tipo de control para el mismo tipo de dato
---

# Skill: Patrones Uniformes de Input en Formularios

## Propósito
Garantizar **uniformidad determinística**: cuando se solicita un formulario 3 veces, los 3 deben tener el mismo tipo de input para cada campo, validadores idénticos y estilos consistentes.

## Filosofía
- **Una fuente de verdad**: Este documento define el mapeo tipo-de-dato → componente + validador
- **No hay variaciones**: Si `email` se genera con `FormInput type="email"` la primera vez, siempre será así
- **Validadores idénticos**: El mismo campo siempre usa el mismo schema Zod
- **Nomenclatura predecible**: Sufijos y prefijos consistentes

---

## 📋 Mapeo: Tipo de Dato → Componente + Validador

### Strings

| Tipo de Dato | Componente | Prop clave | Validador Zod | Ejemplo |
|---|---|---|---|---|
| **Email** | `FormInput` | `type="email"` | `z.string().email('Email inválido')` | `user@company.com` |
| **Teléfono** | `FormInput` | `type="tel"` | `z.string().regex(/^[0-9+\-() ]+$/, 'Teléfono inválido')` | `+56 9 1234 5678` |
| **URL** | `FormInput` | `type="url"` | `z.string().url('URL inválida')` | `https://example.com` |
| **RUT (Chile)** | `FormInput` | `type="text"` + mask | `z.string().regex(/^\d{1,2}\.\d{3}\.\d{3}[-k\d]$/, 'RUT inválido')` | `12.345.678-K` |
| **Texto corto** | `FormInput` | `type="text"` | `z.string().min(1, 'Requerido').min(3, 'Mínimo 3 caracteres')` | Nombre, ciudad |
| **Contraseña** | `FormInput` | `type="password"` | `z.string().min(8, 'Mínimo 8 caracteres')` | (no mostrar) |
| **Búsqueda** | `FormInput` | `type="search"` | `z.string().optional()` | Buscador de tabla |
| **Texto largo** | `FormTextarea` | rows="4" | `z.string().max(500, 'Máximo 500 caracteres')` | Descripción, comentarios |

### Números

| Tipo de Dato | Componente | Prop clave | Validador Zod | Ejemplo |
|---|---|---|---|---|
| **Entero** | `FormInput` | `type="number"` step="1" | `z.number().int('Debe ser entero').positive('Debe ser positivo')` | Cantidad, años |
| **Decimal (dinero)** | `FormInput` | `type="number"` step="0.01" | `z.number().min(0, 'No puede ser negativo').multipleOf(0.01, 'Max 2 decimales')` | Precio, monto |
| **Porcentaje** | `FormInput` | `type="number"` min="0" max="100" step="0.01" | `z.number().min(0).max(100, 'Máximo 100')` | Descuento, comisión |

### Booleanos

| Tipo de Dato | Componente | Prop clave | Validador Zod | Ejemplo |
|---|---|---|---|---|
| **Toggle** | `AppSwitch` | `v-model` | `z.boolean()` | Activar/desactivar, aceptar términos |
| **Checkbox** | `AppSwitch` o `<input type="checkbox">` | `v-model` | `z.boolean()` | Consentimiento |

### Fechas y Tiempos

| Tipo de Dato | Componente | Prop clave | Validador Zod | Ejemplo |
|---|---|---|---|---|
| **Fecha (YYYY-MM-DD)** | `FormInput` | `type="date"` | `z.string().regex(/^\d{4}-\d{2}-\d{2}$/, 'Formato YYYY-MM-DD')` | Fecha de nacimiento, evento |
| **Hora (HH:MM)** | `FormInput` | `type="time"` | `z.string().regex(/^([01][0-9]\|2[0-3]):[0-5][0-9]$/, 'Formato HH:MM')` | Hora de reunión |
| **Datetime** | `FormInput` | `type="datetime-local"` | `z.string().datetime()` | Timestamp |

### Selects (Enumeraciones)

| Tipo de Dato | Componente | Prop clave | Validador Zod | Ejemplo |
|---|---|---|---|---|
| **Select simple** | `FormSelect` | `:options="[]"` | `z.enum(['A', 'B', 'C'], ...)` o `z.string().refine(...)` | País, estado, tipo |
| **Multiselect** | `@vueform/multiselect` | `:options="[]"` `:multiple="true"` | `z.array(z.string()).min(1, 'Selecciona al menos uno')` | Tags, permisos, departamentos |

### Archivos

| Tipo de Dato | Componente | Prop clave | Validador Zod | Ejemplo |
|---|---|---|---|---|
| **Archivo (File)** | `<input type="file">` + composable | `:accept=".pdf,.xlsx"` | `z.instanceof(File).refine(f => f.size < 5 * 1024 * 1024, 'Max 5MB')` | Documentos, facturas |

---

## 🎨 Nomenclatura Consistente

### Variables de Formulario (Forma Reactiva)
```typescript
// ✅ CORRECTO
const form = reactive({
  email: '',
  fullName: '',
  birthDate: '',
  isActive: true,
  roles: [], // multiselect
  avatar: null, // File
})

// ❌ EVITAR
const data = reactive({ ...form }) // confuso
const formData = { ...form } // no reactivo
const user = reactive({...form}) // nombre genérico
```

### Errores
```typescript
// ✅ CORRECTO
const errors = reactive<Partial<Record<keyof typeof form, string>>>({})
errors.email = 'Email inválido'
errors.fullName = '' // limpiar (delete errors.fullName better)

// ❌ EVITAR
const fieldErrors = {} // no reactivo
const errorsData = {} // nombre genérico
```

### Funciones de Validación
```typescript
// ✅ CORRECTO
const validateField = (field: keyof typeof form) => { ... }
const validateForm = () => { ... }

// ❌ EVITAR
const check = () => {...} // demasiado genérico
const validateUserForm = () => {...} // redundante con el componente mismo
```

### Props de Componentes
```typescript
// ✅ CORRECTO (FormInput)
<FormInput
  v-model="form.email"
  label="Correo electrónico"
  type="email"
  placeholder="usuario@empresa.com"
  :error="!!errors.email"
  :error-message="errors.email"
  @blur="validateField('email')"
/>

// ✅ CORRECTO (FormSelect)
<FormSelect
  v-model="form.status"
  label="Estado"
  :options="STATUS_OPTIONS"
  :error="!!errors.status"
  :error-message="errors.status"
/>

// ❌ EVITAR
<FormInput type="invalid-type" /> <!-- solo types predefinidos -->
<FormInput @change="validateField(...)" /> <!-- usar @blur + watch-->
```

---

## ✅ Checklist para Uniformidad

Cuando generes un formulario, verifica:

- [ ] ¿Cada tipo de dato usa el componente correcto de la tabla anterior?
- [ ] ¿Los validadores Zod coinciden exactamente?
- [ ] ¿Las variables de formulario siguen nomenclatura `form.fieldName`?
- [ ] ¿Los errores usan `reactive<Partial<Record<keyof typeof form, string>>>`?
- [ ] ¿Existen funciones `validateField()` y `validateForm()`?
- [ ] ¿Se usa `@blur` + `watch` reactivo para validación en tiempo real?
- [ ] ¿Props de componentes siguen estructura `label`, `type`, `placeholder`, `:error`, `:error-message`?
- [ ] ¿Nomenclatura 100% consistente si generas 3 veces el mismo formulario?

---

## 🔄 Ejemplo Completo: Formulario de Usuario

### DTO (types/user.types.ts)
```typescript
export interface UserForm {
  email: string
  fullName: string
  birthDate: string // YYYY-MM-DD
  phone: string
  status: 'active' | 'inactive'
  roles: string[] // multiselect
  bio: string // textarea
  isNewsletterSubscribed: boolean
}
```

### Validador (validators/user.validator.ts)
```typescript
import { z } from 'zod'

export const userFormSchema = z.object({
  email: z.string().email('Email inválido'),
  fullName: z.string().min(3, 'Mínimo 3 caracteres'),
  birthDate: z.string().regex(/^\d{4}-\d{2}-\d{2}$/, 'Formato YYYY-MM-DD'),
  phone: z.string().regex(/^[0-9+\-() ]+$/, 'Teléfono inválido').optional(),
  status: z.enum(['active', 'inactive']),
  roles: z.array(z.string()).min(1, 'Selecciona al menos un rol'),
  bio: z.string().max(500, 'Máximo 500 caracteres').optional(),
  isNewsletterSubscribed: z.boolean(),
})

export type UserFormType = z.infer<typeof userFormSchema>
```

### Componente (UserFormModal.vue)
```vue
<template>
  <BaseModal :is-open="isOpen" @close="closeModal">
    <form @submit.prevent="handleSubmit" class="space-y-4">
      <!-- Email -->
      <FormInput
        v-model="form.email"
        label="Correo Electrónico"
        type="email"
        placeholder="usuario@empresa.com"
        :error="!!errors.email"
        :error-message="errors.email"
        @blur="validateField('email')"
      />

      <!-- Full Name -->
      <FormInput
        v-model="form.fullName"
        label="Nombre Completo"
        type="text"
        :error="!!errors.fullName"
        :error-message="errors.fullName"
        @blur="validateField('fullName')"
      />

      <!-- Birth Date -->
      <FormInput
        v-model="form.birthDate"
        label="Fecha de Nacimiento"
        type="date"
        :error="!!errors.birthDate"
        :error-message="errors.birthDate"
        @blur="validateField('birthDate')"
      />

      <!-- Phone -->
      <FormInput
        v-model="form.phone"
        label="Teléfono"
        type="tel"
        placeholder="+56 9 1234 5678"
        :error="!!errors.phone"
        :error-message="errors.phone"
        @blur="validateField('phone')"
      />

      <!-- Status -->
      <FormSelect
        v-model="form.status"
        label="Estado"
        :options="STATUS_OPTIONS"
        :error="!!errors.status"
        :error-message="errors.status"
      />

      <!-- Roles (Multiselect) -->
      <div>
        <label>Roles</label>
        <Multiselect
          v-model="form.roles"
          :options="ROLE_OPTIONS"
          multiple
          @blur="validateField('roles')"
        />
        <p v-if="errors.roles" class="text-red-500 text-sm">{{ errors.roles }}</p>
      </div>

      <!-- Bio -->
      <FormTextarea
        v-model="form.bio"
        label="Biografía"
        rows="4"
        placeholder="Cuéntanos sobre ti..."
        :error="!!errors.bio"
        :error-message="errors.bio"
        @blur="validateField('bio')"
      />

      <!-- Newsletter -->
      <AppSwitch
        v-model="form.isNewsletterSubscribed"
        label="Suscribirse al newsletter"
      />

      <!-- Buttons -->
      <FormActions @save="handleSubmit" @cancel="closeModal" />
    </form>
  </BaseModal>
</template>

<script setup lang="ts">
import { reactive, watch } from 'vue'
import { userFormSchema, type UserFormType } from '@/validators/user.validator'

const form = reactive<UserFormType>({
  email: '',
  fullName: '',
  birthDate: '',
  phone: '',
  status: 'active',
  roles: [],
  bio: '',
  isNewsletterSubscribed: false,
})

const errors = reactive<Partial<Record<keyof UserFormType, string>>>({})

const validateField = (field: keyof UserFormType) => {
  const result = userFormSchema.shape[field].safeParse(form[field])
  if (!result.success) {
    errors[field] = result.error.issues[0]?.message
  } else {
    delete errors[field]
  }
}

// Watch para validación reactiva
watch(() => form.email, () => validateField('email'))
watch(() => form.fullName, () => validateField('fullName'))
// ... etc para todos los campos

const handleSubmit = async () => {
  const result = userFormSchema.safeParse(form)
  if (!result.success) {
    Object.keys(result.error.flatten().fieldErrors).forEach(field => {
      errors[field as keyof UserFormType] = result.error.flatten().fieldErrors[field as keyof UserFormType]?.[0] || ''
    })
    return
  }
  
  // Guardar (llamada API)
  await saveUser(result.data)
  closeModal()
}
</script>
```

---

## 🚫 Violaciones a Evitar

❌ **Inconsistencia tipo-componente**: Email con `<input type="text">` en vez de `FormInput type="email"`  
❌ **Validadores diferentes**: Mismo campo con diferentes reglas en diferentes vistas  
❌ **Nomenclatura heterogénea**: `userData`, `userForm`, `user_form` en el mismo proyecto  
❌ **Props olvidadas**: `label`, `:error-message` incompletos  
❌ **Sin validación blur**: Solo validación on-submit sin feedback en tiempo real  

---

## 📚 Referencias
- Componentes compartidos: `src/components/shared/`
- Validadores centrales: `src/validators/`
- Tipos: `src/types/models/`
