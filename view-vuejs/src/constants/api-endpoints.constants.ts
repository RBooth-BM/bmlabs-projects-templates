/**
 * API endpoint segments (relative to apiConfig.baseURL).
 * ME y LOGOUT son null cuando el backend no expone esos endpoints.
 */
export const AUTH_API_ENDPOINTS: {
  BASE: string
  LOGIN: string
  ME: string | null
  LOGOUT: string | null
} = {
  BASE: '/api/v1',
  LOGIN: '/auth/login',
  ME: '/auth/me',
  LOGOUT: null,
}
