/**
 * Shared constants for auth/session keys and common routes.
 */

export const AUTH_STORAGE_KEYS = {
  USER: 'auth_user',
  TOKEN: 'auth_token',
} as const

export const APP_ROUTES = {
  ROOT: '/',
  HOME: '/home',
  DASHBOARD: '/dashboard',
  LOGIN: '/auth/login',
} as const

export const AUTH_CHECK_INTERVAL_MS = 5 * 60 * 1000
