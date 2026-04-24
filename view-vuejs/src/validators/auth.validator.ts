import { z } from 'zod'

/**
 * Schema de validación para login (contrato Swagger: LoginModel)
 */
export const loginSchema = z.object({
  email: z
    .string()
    .trim()
    .min(1, 'El correo es requerido')
    .email('Ingresa un correo electrónico válido')
    .max(100, 'El correo no puede superar los 100 caracteres'),
  password: z
    .string()
    .min(1, 'La contraseña es requerida')
    .min(8, 'La contraseña debe tener al menos 8 caracteres')
    .max(32, 'La contraseña no puede superar los 32 caracteres'),
})

export type LoginForm = z.infer<typeof loginSchema>