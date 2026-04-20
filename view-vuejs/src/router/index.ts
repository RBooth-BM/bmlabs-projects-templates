import { createRouter, createWebHistory } from 'vue-router'
import { APP_ROUTES } from '@/constants'
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
      meta: { requiresAuth: false },
      children: [
        {
          path: '',
          redirect: { name: 'home' },
        },
        {
          path: APP_ROUTES.HOME.slice(1),
          name: 'home',
          component: () => import('@/views/HomeView.vue'),
          meta: { requiresAuth: false },
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
      redirect: APP_ROUTES.DASHBOARD,
    },
  ],
})

router.beforeEach(async (to) => {
  const authStore = useAuthStore()
  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth)
  const isLoginRoute = to.name === 'login'

  authStore.loadFromStorage()

  if (isLoginRoute && authStore.isAuthenticated) {
    const isStillAuthenticated = await authStore.checkAuthIfNeeded()
    if (isStillAuthenticated) {
      return { name: 'dashboard' }
    }
  }

  if (!requiresAuth) {
    return true
  }

  if (!authStore.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  const hasValidSession = await authStore.checkAuthIfNeeded()
  if (!hasValidSession) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  return true
})

export default router
