<template>
  <div
    class="min-h-screen flex flex-col items-center justify-center bg-gray-100 dark:bg-gray-900 px-4 py-8"
  >
    <main
      class="w-full max-w-md flex flex-col items-center rounded-xl bg-white dark:bg-gray-800 shadow-lg border border-gray-200 dark:border-gray-700 p-8"
    >
      <div class="flex flex-col items-center mb-8">
        <div
          class="h-12 w-12 rounded-xl bg-[var(--color-primary)] text-white flex items-center justify-center text-sm font-semibold"
          aria-hidden="true"
        >
          BM
        </div>
        <h1 class="mt-6 text-xl font-semibold text-gray-800 dark:text-gray-100">
          Inicio de sesion
        </h1>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400 text-center">
          Plataforma administrativa corporativa
        </p>
      </div>

      <form class="w-full space-y-5" @submit.prevent="handleSubmit">
        <FormInput
          v-model="usernameOrEmail"
          label="Usuario o correo"
          type="text"
          placeholder="usuario@empresa.com"
          autocomplete="username"
          :error="!!errors.usernameOrEmail"
          :error-message="errors.usernameOrEmail"
          required
          @blur="validateUsernameOrEmail"
        />
        <FormInput
          v-model="password"
          label="Contrasena"
          type="password"
          show-password-toggle
          placeholder="********"
          autocomplete="current-password"
          :error="!!errors.password"
          :error-message="errors.password"
          required
        />

        <div class="pt-1">
          <button
            type="submit"
            class="w-full rounded-lg bg-[var(--color-primary)] py-3 px-4 text-sm font-medium text-white shadow-sm transition hover:opacity-90 focus:outline-none focus:ring-2 focus:ring-[var(--color-primary)] focus:ring-offset-2 disabled:opacity-60 disabled:cursor-not-allowed"
            :disabled="isSubmitting"
          >
            <span v-if="!isSubmitting">Ingresar</span>
            <span v-else class="inline-flex items-center gap-2">Ingresando...</span>
          </button>
        </div>

        <p class="text-center">
          <router-link
            to="/forgot-password"
            class="text-sm text-gray-500 dark:text-gray-400 hover:text-[var(--color-primary)] hover:underline"
          >
            Olvidaste tu contrasena?
          </router-link>
        </p>
      </form>
    </main>

    <p class="mt-6 text-xs text-gray-400 dark:text-gray-500 text-center">
      Sistema corporativo de gestion y operaciones
    </p>
  </div>
</template>

<script setup lang="ts">
import { FormInput } from '@/components/form-input'
import { useLoginForm } from './composables'

const { usernameOrEmail, password, isSubmitting, errors, validateUsernameOrEmail, handleSubmit } =
  useLoginForm()
</script>
