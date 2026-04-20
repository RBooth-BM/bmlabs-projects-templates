/**
 * Constants for status values in the application
 */

export const STATUS_ACTIVE = '1' as const
export const STATUS_INACTIVE = '0' as const

export const STATUS_TEXT_ACTIVE = 'Activa' as const
export const STATUS_TEXT_INACTIVE = 'Inactiva' as const

export const STATUS_COLOR_ACTIVE = 'green' as const
export const STATUS_COLOR_INACTIVE = 'red' as const

export const DEFAULT_STATUS_COLOR = '#6B7280'

export type StatusValue = typeof STATUS_ACTIVE | typeof STATUS_INACTIVE
export type StatusColor = typeof STATUS_COLOR_ACTIVE | typeof STATUS_COLOR_INACTIVE
