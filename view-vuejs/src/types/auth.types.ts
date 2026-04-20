export interface LoginRequestDto {
  usernameOrEmail: string
  password: string
}

export interface AuthUser {
  id?: string
  name: string
  email?: string
  role?: string
}

export interface LoginResponseDto {
  user?: AuthUser
  accessToken?: string
  token?: string
  [key: string]: unknown
}

export interface MeResponseDto {
  user?: AuthUser | null
  [key: string]: unknown
}
