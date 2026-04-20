# Referencia Real: Autenticación (Patrón BM)

> Código fuente extraído del proyecto base `test-clean-agentic-app`. DEBES replicar estos patrones exactos.

---

## 1. Auth Service (`auth.service.ts`)

```typescript
// src/services/api/services/auth.service.ts
import httpClient from '../http-client'

export interface LoginCredentials {
  usernameOrEmail: string
  password: string
}

export interface AuthUser {
  id: string
  name: string
  email: string
  role?: string
  profileId?: string
  permissions?: {
    modules: string[]
    grantedActionKeys?: string[]
  }
}

export interface LoginResponse {
  user: AuthUser
}

export interface CheckAuthResponse {
  user: AuthUser | null
}

/**
 * Servicio de autenticación — los tokens viajan en cookies HttpOnly.
 * El frontend NO almacena tokens; solo persiste la info del usuario.
 */
export class AuthService {
  private static readonly BASE_URL = '/auth'

  static async login(credentials: LoginCredentials): Promise<LoginResponse> {
    const response = await httpClient.post<LoginResponse>(
      `${this.BASE_URL}/login`, credentials, { skipErrorHandler: true },
    )
    return response.data
  }

  static async loginWithGoogle(idToken: string): Promise<LoginResponse> {
    const response = await httpClient.post<LoginResponse>(
      `${this.BASE_URL}/google`, { idToken }, { skipErrorHandler: true },
    )
    return response.data
  }

  static async logout(): Promise<void> {
    await httpClient.post(`${this.BASE_URL}/logout`, null, { skipErrorHandler: true })
  }

  static async checkAuth(): Promise<CheckAuthResponse> {
    const response = await httpClient.get<CheckAuthResponse>(`${this.BASE_URL}/me`)
    return response.data
  }

  static async forgotPassword(email: string): Promise<{ message: string }> {
    const response = await httpClient.post<{ message: string }>(
      `${this.BASE_URL}/forgot-password`, { email }, { skipErrorHandler: true },
    )
    return response.data
  }

  static async resetPassword(data: {
    token: string; newPassword: string; confirmPassword: string
  }): Promise<{ message: string }> {
    const response = await httpClient.post<{ message: string }>(
      `${this.BASE_URL}/reset-password`, data, { skipErrorHandler: true },
    )
    return response.data
  }
}
```

---

## 2. Auth Store (`auth.store.ts`) — Setup Store con Pinia

```typescript
// src/stores/auth.store.ts
import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { AuthService } from '@/services/api/services/auth.service'
import type { AuthUser, LoginRequestDto } from '@/types/auth.types'

const USER_STORAGE_KEY = 'auth_user'
const TOKEN_STORAGE_KEY = 'auth_token'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AuthUser | null>(null)

  // isAuthenticated verifica el token en sessionStorage (Bearer token)
  const isAuthenticated = computed(() => !!sessionStorage.getItem(TOKEN_STORAGE_KEY))

  const setSession = (payload: { user: AuthUser; token?: string }) => {
    user.value = payload.user
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(payload.user))
    sessionStorage.setItem(TOKEN_STORAGE_KEY, payload.token || 'authenticated')
  }

  const clearSession = () => {
    user.value = null
    localStorage.removeItem(USER_STORAGE_KEY)
    sessionStorage.removeItem(TOKEN_STORAGE_KEY)
  }

  const login = async (credentials: LoginRequestDto) => {
    const response = await AuthService.login(credentials)
    setSession({ user: response.user, token: response.accessToken || response.token })
  }

  const loadFromStorage = () => {
    const savedUser = localStorage.getItem(USER_STORAGE_KEY)
    if (!savedUser) return
    try {
      user.value = JSON.parse(savedUser) as AuthUser
    } catch {
      clearSession()
    }
  }

  const checkAuth = async (): Promise<boolean> => {
    const token = sessionStorage.getItem(TOKEN_STORAGE_KEY)
    if (!token) {
      clearSession()
      return false
    }
    try {
      const response = await AuthService.me()
      if (!response.user) {
        clearSession()
        return false
      }
      user.value = response.user
      localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(response.user))
      return true
    } catch {
      clearSession()
      return false
    }
  }

  const logout = async () => {
    try {
      await AuthService.logout()
    } finally {
      clearSession()
    }
  }

  return {
    user,
    isAuthenticated,
    login,
    logout,
    loadFromStorage,
    checkAuth,
  }
})
```

**Token Strategy**: Tokens Bearer almacenados en `sessionStorage('auth_token')`. Info del usuario en `localStorage('auth_user')`. El `httpClient` inyecta el token automáticamente. En 401 redirige directamente a `/auth/login`.

---

## 3. Login View con Zod y componentes corporativos

```vue
<!-- src/views/auth/LoginView.vue -->
<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AppBreadcrumb from '@/components/shared/AppBreadcrumb.vue'
import AppButton from '@/components/shared/AppButton.vue'
import FormInput from '@/components/shared/FormInput.vue'
import { useAuthStore } from '@/stores/auth.store'
import { loginSchema, type LoginForm } from '@/validators/auth.validator'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const form = reactive<LoginForm>({
  usernameOrEmail: '',
  password: '',
})

const errors = reactive<Partial<Record<keyof LoginForm, string>>>({})
const apiError = ref('')
const isLoading = ref(false)

const validate = () => {
  const result = loginSchema.safeParse(form)
  errors.usernameOrEmail = undefined
  errors.password = undefined

  if (!result.success) {
    for (const issue of result.error.issues) {
      const field = issue.path[0] as keyof LoginForm
      if (!errors[field]) errors[field] = issue.message
    }
    return false
  }
  return true
}

const handleSubmit = async () => {
  apiError.value = ''
  if (!validate()) return

  isLoading.value = true
  try {
    await authStore.login(form)
    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/dashboard'
    await router.push(redirect)
  } catch {
    apiError.value = 'Credenciales inválidas. Verifica tus datos e intenta nuevamente.'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-100 px-4 py-10 dark:bg-gray-950">
    <div class="mx-auto w-full max-w-md">
      <AppBreadcrumb />

      <section
        class="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-800 dark:bg-gray-900"
      >
        <header class="mb-6 space-y-1">
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Iniciar sesión</h1>
          <p class="text-sm text-gray-600 dark:text-gray-400">
            Accede al panel administrativo.
          </p>
        </header>

        <form class="space-y-4" @submit.prevent="handleSubmit">
          <FormInput
            id="login-username"
            v-model="form.usernameOrEmail"
            label="Usuario o correo"
            autocomplete="username"
            :error="!!errors.usernameOrEmail"
            :error-message="errors.usernameOrEmail"
          />

          <FormInput
            id="login-password"
            v-model="form.password"
            type="password"
            label="Contraseña"
            autocomplete="current-password"
            :show-password-toggle="true"
            :error="!!errors.password"
            :error-message="errors.password"
          />

          <p v-if="apiError" class="text-sm font-medium text-red-600 dark:text-red-400">{{ apiError }}</p>

          <AppButton id="btn-login" type="submit" full-width :is-loading="isLoading">
            Entrar al sistema
          </AppButton>
        </form>
      </section>
    </div>
  </div>
</template>
```

**Reglas clave**:
- Importar schema Zod desde `@/validators/auth.validator` (no inline)
- Usar `FormInput` (no `<input>` nativo) con props `:error` y `:error-message`
- Usar `AppButton` con `:is-loading` para feedback visual
- `AppBreadcrumb` obligatorio al inicio del template
- Redirect post-login usa `route.query.redirect` con fallback a `/dashboard`
- Ruta de login: `/auth/login` (no `/login`)

---

## Checklist de Calidad

- [ ] ¿`AuthService` es clase estática con `skipErrorHandler: true`?
- [ ] ¿Auth store usa `defineStore('auth', () => { ... })` (setup store)?
- [ ] ¿Auth store usa `setSession` / `clearSession` para gestionar el token?
- [ ] ¿`isAuthenticated` verifica `sessionStorage('auth_token')`?
- [ ] ¿Login valida con Zod (desde `@/validators/`) antes de enviar?
- [ ] ¿Se usa `FormInput` en lugar de `<input>` nativo?
- [ ] ¿Se usa `AppButton` con `:is-loading` en el botón de submit?
- [ ] ¿`AppBreadcrumb` incluido al inicio de la vista?

