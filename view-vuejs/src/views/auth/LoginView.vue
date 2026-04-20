<template>
  <div
    class="min-h-screen flex flex-col items-center justify-center bg-gray-100 dark:bg-gray-900 px-4 py-8"
  >
    <!-- Botón de cambio de tema -->
    <button
      @click="toggleTheme"
      class="fixed top-4 right-4 z-10 flex h-10 w-10 items-center justify-center rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-200 shadow-lg transition-all hover:bg-gray-50 dark:hover:bg-gray-700"
      aria-label="Cambiar tema"
    >
      <Sun v-if="!isDarkMode" class="h-5 w-5" />
      <Moon v-else class="h-5 w-5" />
    </button>

    <main
      class="w-full max-w-md flex flex-col items-center rounded-xl bg-white dark:bg-gray-800 shadow-lg border border-gray-200 dark:border-gray-700 p-8"
    >
      <div class="flex flex-col items-center mb-8">
        <img
          :src="logoSrc"
          alt="Logo BM Labs"
          class="h-20 w-20 rounded-2xl object-cover mb-6 shadow-lg"
        />
        <h1 class="mt-6 text-xl font-semibold text-gray-800 dark:text-gray-100">
          Inicio de sesión
        </h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400 text-center">
          Plataforma administrativa corporativa
        </p>
      </div>

      <form class="w-full space-y-5" @submit.prevent="handleSubmit">
        <FormInput
          id="usernameOrEmail"
          v-model="form.usernameOrEmail"
          label="Usuario o correo"
          type="text"
          placeholder="usuario@empresa.com"
          autocomplete="username"
          :error="!!errors.usernameOrEmail"
          :error-message="errors.usernameOrEmail"
          @blur="validateField('usernameOrEmail')"
          required
        />

        <FormInput
          id="password"
          v-model="form.password"
          label="Contraseña"
          type="password"
          show-password-toggle
          placeholder="********"
          autocomplete="current-password"
          :error="!!errors.password"
          :error-message="errors.password"
          @blur="validateField('password')"
          required
        />

        <div v-if="apiError" class="rounded-md bg-red-50 dark:bg-red-900/20 p-3">
          <p class="text-sm text-red-600 dark:text-red-400">{{ apiError }}</p>
        </div>

        <div class="pt-1">
          <AppButton
            id="btn-login"
            type="submit"
            variant="primary"
            :full-width="true"
            :is-loading="isSubmitting"
            :disabled="isSubmitting"
          >
            {{ isSubmitting ? 'Ingresando...' : 'Ingresar' }}
          </AppButton>
        </div>

        <p class="text-center">
          <router-link
            to="/forgot-password"
            class="text-sm text-gray-500 dark:text-gray-400 hover:text-[var(--color-primary)] hover:underline"
          >
            ¿Olvidaste tu contraseña?
          </router-link>
        </p>
      </form>
    </main>

    <p class="mt-6 text-xs text-gray-400 dark:text-gray-500 text-center">
      Sistema corporativo de gestión y operaciones
    </p>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { onMounted } from 'vue'
import { Sun, Moon } from 'lucide-vue-next'
import { loginSchema, type LoginForm } from '@/validators/auth.validator'
import FormInput from '@/components/shared/FormInput.vue'
import AppButton from '@/components/shared/AppButton.vue'
import { APP_ROUTES } from '@/constants'
import logoSrc from '@/assets/logo/logo.svg'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const isDarkMode = ref(false)

const toggleTheme = () => {
  isDarkMode.value = !isDarkMode.value
  if (isDarkMode.value) {
    document.documentElement.classList.add('dark')
    localStorage.setItem('theme', 'dark')
  } else {
    document.documentElement.classList.remove('dark')
    localStorage.setItem('theme', 'light')
  }
}

const form = reactive<LoginForm>({
  usernameOrEmail: '',
  password: '',
})

const errors = reactive<Partial<Record<keyof LoginForm, string>>>({})
const apiError = ref('')
const isSubmitting = ref(false)
const touched = reactive<Record<keyof LoginForm, boolean>>({
  usernameOrEmail: false,
  password: false,
})

const validateField = (field: keyof LoginForm) => {
  touched[field] = true
  const fieldValue = form[field]

  if (field === 'usernameOrEmail') {
    const result = loginSchema.shape.usernameOrEmail.safeParse(fieldValue)
    if (!result.success) {
      errors.usernameOrEmail = result.error.issues[0]?.message
    } else {
      delete errors.usernameOrEmail
    }
    return
  }

  const result = loginSchema.shape.password.safeParse(fieldValue)
  if (!result.success) {
    errors.password = result.error.issues[0]?.message
  } else {
    delete errors.password
  }
}

watch(
  () => form.usernameOrEmail,
  () => {
    if (touched.usernameOrEmail) {
      const result = loginSchema.shape.usernameOrEmail.safeParse(form.usernameOrEmail)
      if (!result.success) {
        errors.usernameOrEmail = result.error.issues[0]?.message
      } else {
        delete errors.usernameOrEmail
      }
    }
  },
)

watch(
  () => form.password,
  () => {
    if (touched.password) {
      const result = loginSchema.shape.password.safeParse(form.password)
      if (!result.success) {
        errors.password = result.error.issues[0]?.message
      } else {
        delete errors.password
      }
    }
  },
)

const validateForm = () => {
  const result = loginSchema.safeParse(form)

  // Limpiar errores previos
  Object.keys(errors).forEach(key => {
    delete errors[key as keyof LoginForm]
  })

  if (!result.success) {
    result.error.issues.forEach((issue) => {
      const path = issue.path.join('.') as keyof LoginForm
      errors[path] = issue.message
    })
    return false
  }

  return true
}

const handleSubmit = async () => {
  if (!validateForm()) return

  isSubmitting.value = true
  apiError.value = ''

  try {
    // Mock login - en producción esto llamaría al auth store
    await new Promise(resolve => setTimeout(resolve, 1000)) // Simular delay

    // Simular respuesta exitosa
    const mockUser = {
      id: '1',
      name: form.usernameOrEmail,
      email: form.usernameOrEmail,
    }

    // En producción: await authStore.login(form)
    authStore.setSession({ user: mockUser, token: 'mock-token' })

    // Redirigir
    const redirectTo = (route.query.redirect as string) || APP_ROUTES.DASHBOARD
    router.push(redirectTo)

  } catch (error) {
    apiError.value = error instanceof Error ? error.message : 'Error al iniciar sesión'
  } finally {
    isSubmitting.value = false
  }
}

// Inicializar tema desde localStorage
onMounted(() => {
  const savedTheme = localStorage.getItem('theme')
  if (savedTheme === 'dark') {
    isDarkMode.value = true
    document.documentElement.classList.add('dark')
  }
})
</script>
