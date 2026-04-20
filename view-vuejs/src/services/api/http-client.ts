import axios, {
  type AxiosError,
  type AxiosInstance,
  type AxiosResponse,
  type InternalAxiosRequestConfig,
} from 'axios'
import { apiConfig } from '@/config/api.config'
import { APP_ROUTES, AUTH_STORAGE_KEYS } from '@/constants'

declare module 'axios' {
  export interface AxiosRequestConfig {
    skipErrorHandler?: boolean
  }
}

const createHttpClient = (): AxiosInstance =>
  axios.create({
    baseURL: apiConfig.baseURL,
    timeout: apiConfig.timeout,
    withCredentials: apiConfig.withCredentials,
    headers: apiConfig.headers,
  })

const httpClient = createHttpClient()

httpClient.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = sessionStorage.getItem(AUTH_STORAGE_KEYS.TOKEN)

  if (token) {
    config.headers = config.headers ?? {}
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

httpClient.interceptors.response.use(
  (response: AxiosResponse) => response,
  async (error: AxiosError) => {
    if (error.config?.skipErrorHandler) {
      return await Promise.reject(error)
    }

    if (error.response?.status === 401) {
      sessionStorage.removeItem(AUTH_STORAGE_KEYS.TOKEN)
      localStorage.removeItem(AUTH_STORAGE_KEYS.USER)

      const { default: router } = await import('@/router')
      if (router.currentRoute.value.path !== APP_ROUTES.LOGIN) {
        await router.replace({ path: APP_ROUTES.LOGIN })
      }
    }

    return await Promise.reject(error)
  },
)

export default httpClient
