import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { AuthService } from '@/services/api/services/auth.service'
import type { AuthUser, LoginRequestDto } from '@/types/auth.types'
import { AUTH_API_ENDPOINTS, AUTH_CHECK_INTERVAL_MS, AUTH_STORAGE_KEYS } from '@/constants'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AuthUser | null>(null)
  const lastAuthCheckAt = ref<number>(0)

  const isAuthenticated = computed(() => !!sessionStorage.getItem(AUTH_STORAGE_KEYS.TOKEN))

  const setSession = (payload: { user: AuthUser; token: string }) => {
    user.value = payload.user
    localStorage.setItem(AUTH_STORAGE_KEYS.USER, JSON.stringify(payload.user))
    sessionStorage.setItem(AUTH_STORAGE_KEYS.TOKEN, payload.token)
  }

  const clearSession = () => {
    user.value = null
    localStorage.removeItem(AUTH_STORAGE_KEYS.USER)
    sessionStorage.removeItem(AUTH_STORAGE_KEYS.TOKEN)
    lastAuthCheckAt.value = 0
  }

  const login = async (credentials: LoginRequestDto) => {
    const response = await AuthService.login(credentials)
    const token =
      response.access_token ||
      response.accessToken ||
      response.token ||
      response.jwt

    if (!token) {
      throw new Error('El servidor no devolvió un token de autenticación')
    }

    const userPayload: AuthUser = response.user ?? {
      id: response.usuarioId ?? undefined,
      name: credentials.email,
      email: credentials.email,
    }

    setSession({ user: userPayload, token })
  }

  const loadFromStorage = () => {
    const savedUser = localStorage.getItem(AUTH_STORAGE_KEYS.USER)
    if (!savedUser) return

    try {
      user.value = JSON.parse(savedUser) as AuthUser
    } catch {
      clearSession()
    }
  }

  const checkAuth = async (): Promise<boolean> => {
    const token = sessionStorage.getItem(AUTH_STORAGE_KEYS.TOKEN)
    if (!token) {
      clearSession()
      return false
    }

    if (AUTH_API_ENDPOINTS.ME === null) {
      // Sin endpoint de validación: la presencia del token es suficiente
      loadFromStorage()
      lastAuthCheckAt.value = Date.now()
      return true
    }

    try {
      const response = await AuthService.me()
      if (!response.user) {
        clearSession()
        return false
      }
      user.value = response.user
      localStorage.setItem(AUTH_STORAGE_KEYS.USER, JSON.stringify(response.user))
      lastAuthCheckAt.value = Date.now()
      return true
    } catch {
      clearSession()
      return false
    }
  }

  const checkAuthIfNeeded = async (): Promise<boolean> => {
    const now = Date.now()
    if (now - lastAuthCheckAt.value < AUTH_CHECK_INTERVAL_MS) {
      return !!user.value
    }
    return checkAuth()
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
    setSession,
    clearSession,
    login,
    logout,
    loadFromStorage,
    checkAuth,
    checkAuthIfNeeded,
  }
})
