/**
 * Constants for action keys used in authorization and permissions
 */

export const ACTION_KEYS = {
  Dashboard: {
    View: 'Dashboard.View',
  },
  Home: {
    View: 'Home.View',
  },
  Example: {
    View: 'Example.View',
    Create: 'Example.Create',
    Edit: 'Example.Edit',
    Delete: 'Example.Delete',
  },
} as const

type ActionGroup = (typeof ACTION_KEYS)[keyof typeof ACTION_KEYS]
export type ActionKey = ActionGroup[keyof ActionGroup]
