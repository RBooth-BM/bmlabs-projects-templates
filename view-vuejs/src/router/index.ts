import { createRouter, createWebHistory } from 'vue-router'
import { APP_ROUTES, AUTH_API_ENDPOINTS, DEFAULT_AFTER_LOGIN_ROUTE } from '@/constants'
import { useAuthStore } from '@/stores/auth.store'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: APP_ROUTES.LOGIN,
      name: 'login',
      component: () => import('@/views/auth/LoginView.vue'),
      meta: { public: true },
    },
    {
      path: '/',
      component: () => import('@/layout/DefaultLayout.vue'),
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          redirect: { name: 'home' },
        },
        {
          path: APP_ROUTES.HOME.slice(1),
          name: 'home',
          component: () => import('@/views/HomeView.vue'),
          meta: { requiresAuth: true },
        },
        {
          path: APP_ROUTES.DASHBOARD.slice(1),
          name: 'dashboard',
          component: () => import('@/views/DashboardView.vue'),
          meta: { requiresAuth: true },
        },
        // Agregar nuevas rutas según necesidad:
        // {
        //   path: '[entity]',
        //   name: '[entity]',
        //   component: () => import('@/views/[Entity]/[Entity]View.vue'),
        //   meta: { requiresAuth: true },
        // },
      ],
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: DEFAULT_AFTER_LOGIN_ROUTE,
    },
  ],
})

router.beforeEach(async (to) => {
  const authStore = useAuthStore()
  authStore.loadFromStorage()

  // Rutas públicas: si ya está autenticado, redirigir al área principal
  if (to.meta.public) {
    if (authStore.isAuthenticated) {
      return { path: DEFAULT_AFTER_LOGIN_ROUTE }
    }
    return true
  }

  // Validar sesión con el servidor solo si hay endpoint /me configurado
  if (AUTH_API_ENDPOINTS.ME !== null) {
    await authStore.checkAuthIfNeeded()
  }

  if (!authStore.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  return true
})

export default router

