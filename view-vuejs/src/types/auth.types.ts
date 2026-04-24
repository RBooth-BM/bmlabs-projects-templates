export interface LoginRequestDto {
  email: string
  password: string
}

export interface AuthUser {
  id?: string
  name: string
  email?: string
  role?: string
}

export interface LoginResponseDto {
  access_token?: string | null
  accessToken?: string | null
  token?: string | null
  jwt?: string | null
  expiration?: number | null
  eleccionId?: string | null
  usuarioId?: string | null
  user?: AuthUser | null
  [key: string]: unknown
}

export interface MeResponseDto {
  user?: AuthUser | null
  [key: string]: unknown
}
