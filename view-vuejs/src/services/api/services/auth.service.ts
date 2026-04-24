import httpClient from '../http-client'
import type { LoginRequestDto, LoginResponseDto, MeResponseDto } from '@/types/auth.types'
import { AUTH_API_ENDPOINTS } from '@/constants'

export class AuthService {
  private static readonly BASE_URL = AUTH_API_ENDPOINTS.BASE

  private static normalizeLoginResponse(
    payload: Record<string, unknown>,
    credentials: LoginRequestDto,
  ): LoginResponseDto {
    const token =
      (payload.access_token as string | undefined) ||
      (payload.accessToken as string | undefined) ||
      (payload.token as string | undefined) ||
      (payload.jwt as string | undefined)

    const userFromPayload =
      (payload.user as Record<string, unknown> | undefined) ||
      (payload.profile as Record<string, unknown> | undefined)

    const fallbackName = credentials.email.split('@')[0] || credentials.email

    return {
      access_token: token,
      accessToken: token,
      token,
      expiration: payload.expiration as number | undefined,
      eleccionId: payload.eleccionId as string | undefined,
      usuarioId: payload.usuarioId as string | undefined,
      user: userFromPayload
        ? {
            id: (userFromPayload.id as string | undefined),
            name: (userFromPayload.name as string | undefined) || fallbackName,
            email:
              (userFromPayload.email as string | undefined) ||
              (payload.email as string | undefined) ||
              credentials.email,
            role: (userFromPayload.role as string | undefined) || (payload.role as string | undefined),
          }
        : null,
    }
  }

  private static normalizeMeResponse(payload: Record<string, unknown>): MeResponseDto {
    const userFromPayload =
      (payload.user as Record<string, unknown> | undefined) ||
      (payload.profile as Record<string, unknown> | undefined)

    if (!userFromPayload) {
      if (payload.id || payload.name || payload.email) {
        return {
          user: {
            id: payload.id as string | undefined,
            name: (payload.name as string | undefined) || 'Usuario',
            email: payload.email as string | undefined,
            role: payload.role as string | undefined,
          },
        }
      }

      return { user: null }
    }

    return {
      user: {
        id: userFromPayload.id as string | undefined,
        name: (userFromPayload.name as string | undefined) || 'Usuario',
        email: userFromPayload.email as string | undefined,
        role: userFromPayload.role as string | undefined,
      },
    }
  }

  static async login(credentials: LoginRequestDto): Promise<LoginResponseDto> {
    const response = await httpClient.post<Record<string, unknown>>(
      `${this.BASE_URL}${AUTH_API_ENDPOINTS.LOGIN}`,
      credentials,
      { skipErrorHandler: true },
    )
    return this.normalizeLoginResponse(response.data, credentials)
  }

  static async me(): Promise<MeResponseDto> {
    if (AUTH_API_ENDPOINTS.ME === null) return { user: null }
    const response = await httpClient.get<Record<string, unknown>>(
      `${this.BASE_URL}${AUTH_API_ENDPOINTS.ME}`,
      { skipErrorHandler: true },
    )
    return this.normalizeMeResponse(response.data)
  }

  static async logout(): Promise<void> {
    if (AUTH_API_ENDPOINTS.LOGOUT === null) return
    await httpClient.post(
      `${this.BASE_URL}${AUTH_API_ENDPOINTS.LOGOUT}`,
      null,
      { skipErrorHandler: true },
    )
  }
}
