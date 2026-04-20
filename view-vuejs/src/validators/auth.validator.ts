import { z } from 'zod'

/**
 * Schema de validación para login
 */
export const loginSchema = z.object({
  usernameOrEmail: z
    .string()
    .trim()
    .min(1, 'El usuario o correo es requerido')
    .superRefine((value, ctx) => {
      if (value.includes('@')) {
        const emailResult = z.string().email('Ingresa un correo electronico valido').safeParse(value)
        if (!emailResult.success) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: 'Ingresa un correo electronico valido',
          })
        }
        return
      }

      if (value.length < 3) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: 'El usuario debe tener al menos 3 caracteres',
        })
      }
    }),
  password: z
    .string()
    .min(1, 'La contraseña es requerida')
    .min(6, 'La contraseña debe tener al menos 6 caracteres'),
})

export type LoginForm = z.infer<typeof loginSchema>