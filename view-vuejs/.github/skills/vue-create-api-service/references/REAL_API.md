# Referencia Real: API Service (Patrón BM)

> Código fuente extraído del proyecto base `test-clean-agentic-app`.
> DEBES replicar estos patrones exactos al generar nuevos servicios.

---

## 1. HTTP Client Centralizado (`http-client.ts`)

Todo proyecto BM tiene un cliente Axios centralizado que maneja interceptores de autenticación y errores globales. **NUNCA** crees una instancia nueva de Axios.

```typescript
// src/services/api/http-client.ts
import axios, {
  type AxiosError,
  type AxiosInstance,
  type AxiosResponse,
  type InternalAxiosRequestConfig,
} from 'axios'
import { apiConfig } from '@/config/api.config'

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

// Interceptor de Request: inyecta Bearer token desde sessionStorage
httpClient.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = sessionStorage.getItem('auth_token')

  if (token) {
    config.headers = config.headers ?? {}
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

// Interceptor de Response: manejo 401 → redirige a /auth/login
httpClient.interceptors.response.use(
  (response: AxiosResponse) => response,
  async (error: AxiosError) => {
    if (error.config?.skipErrorHandler) {
      return await Promise.reject(error)
    }

    if (error.response?.status === 401) {
      sessionStorage.removeItem('auth_token')
      localStorage.removeItem('auth_user')

      if (window.location.pathname !== '/auth/login') {
        window.location.href = '/auth/login'
      }
    }

    return await Promise.reject(error)
  },
)

export default httpClient
```

**Regla Inflexible**: Siempre importar `httpClient` desde este archivo. Nunca `import axios from 'axios'`.

**Token Strategy**: Este proyecto usa token Bearer en `sessionStorage('auth_token')`. El interceptor lo inyecta automáticamente en cada request. En 401 redirige a `/auth/login` y limpia sesión.

---

## 2. Patrón de Servicio: Clase Estática

```typescript
// src/services/api/services/users.service.ts
import httpClient from '../http-client'
import type { SortDirection } from '@/constants/sort-direction.constants'
import type { User } from '@/types'
import type { ApiRequestConfig } from '@/types/api.types'

/** DTO para creación/edición de usuarios */
export interface UserDto {
  id?: string
  name: string
  username: string
  email: string
  password?: string
  profileId?: string
  agencyId?: string
  branchIds?: string[]
  isClient?: boolean
  isActive: boolean
  createdAt?: string
  updatedAt?: string
}

/** Servicio de API para Usuarios — métodos estáticos tipados */
export class UsersService {
  private static readonly BASE_URL = '/users'

  /** Obtiene usuarios paginados con filtros opcionales */
  static async getPaged(
    page: number = 1,
    pageSize: number = 10,
    nameFilter?: string,
    profileFilter?: string,
    usernameFilter?: string,
    emailFilter?: string,
    isActiveFilter?: boolean,
    sortBy?: string,
    sortDirection?: SortDirection,
  ): Promise<{
    items: User[]
    totalCount: number
    page: number
    pageSize: number
    totalPages: number
  }> {
    const params: Record<string, unknown> = { page, pageSize }
    if (nameFilter) params.nameFilter = nameFilter
    if (profileFilter) params.profileFilter = profileFilter
    if (usernameFilter) params.usernameFilter = usernameFilter
    if (emailFilter) params.emailFilter = emailFilter
    if (isActiveFilter !== undefined) params.isActiveFilter = isActiveFilter
    if (sortBy) params.sortBy = sortBy
    if (sortDirection) params.sortDirection = sortDirection

    const response = await httpClient.get<{
      items: User[]
      totalCount: number
      page: number
      pageSize: number
      totalPages: number
    }>(`${this.BASE_URL}/Search`, { params })

    return response.data
  }

  /** Obtiene un usuario por ID */
  static async getById(id: string): Promise<UserDto> {
    const response = await httpClient.get<UserDto>(`${this.BASE_URL}/${id}`)
    return response.data
  }

  /** Obtiene usuarios activos (para Selects) */
  static async getActive(): Promise<UserDto[]> {
    const response = await httpClient.get<UserDto[]>(`${this.BASE_URL}/Active`)
    return response.data
  }

  /** Crea o actualiza un usuario */
  static async createOrUpdate(
    dto: UserDto,
    config?: ApiRequestConfig,
  ): Promise<{ user: UserDto; warning?: string }> {
    const response = await httpClient.patch<UserDto | { user: UserDto; warning: string }>(
      `${this.BASE_URL}/CreateOrUpdate`, dto, config,
    )
    const data = response.data
    if (data && typeof data === 'object' && 'warning' in data && 'user' in data) {
      return { user: (data as { user: UserDto; warning: string }).user, warning: (data as { user: UserDto; warning: string }).warning }
    }
    return { user: data as UserDto }
  }
}
```

### Patrón alternativo: Objeto con funciones (proyectos más simples)

```typescript
// src/_services/Guia.api.ts (patrón corporativo)
import ApiClient from '@/api/ApiClient'

interface GuiaPaginatedResponse {
  items: GuiaData[]
  totalCount: number
  totalPages: number
  page: number
  pageSize: number
}

async function GetGuiasPaginated(
  userId: string, page: number, pageSize: number,
): Promise<{ status: number; data: GuiaPaginatedResponse }> {
  return await ApiClient.get('Guia/GetPaginated', {
    params: { userId, page, pageSize },
  })
}

const GuiaService = { GetGuiasPaginated }
export default GuiaService
```

---

## 3. Tipos (archivo separado `*.types.ts`)

```typescript
// src/types/api.types.ts
export interface ApiRequestConfig {
  skipErrorHandler?: boolean
  skipLoadingBar?: boolean
  silent404?: boolean
}

export interface ApiErrorResponse {
  title?: string
  message?: string
  detail?: string
  errors?: Record<string, string[]>
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}
```

---

## Checklist de Calidad

- [ ] ¿Usa `httpClient` centralizado (no `axios` directo)?
- [ ] ¿Clase con métodos estáticos (`static async`)?
- [ ] ¿Todos los retornos tipados con interfaces?
- [ ] ¿DTOs en archivo separado o exportados junto al servicio?
- [ ] ¿`BASE_URL` como propiedad privada?
- [ ] ¿Sin lógica de negocio (solo transporte de datos)?
- [ ] ¿Soporte para `ApiRequestConfig` en métodos de escritura?
