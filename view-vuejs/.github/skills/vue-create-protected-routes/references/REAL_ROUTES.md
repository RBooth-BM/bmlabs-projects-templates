# Referencia Real: Router y Navigation Guards (Patrón BM)

> Patrón consolidado desde proyectos productivos de BM (`airexp-backoffice-app`, `ssa-epp-backoffice-app`, `polpaico-muestreo-app`) y adaptado al template base.
> DEBES replicar este patrón al configurar el router en nuevos módulos.

---

## 1. Estructura del Router (`src/router/index.ts`)

```typescript
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { APP_ROUTES } from '@/constants'

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
        // ↓ Agregar nuevas rutas aquí como children
        // {
        //   path: 'entities',
        //   name: 'entities',
        //   component: () => import('@/views/entities/EntitiesView.vue'),
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
  await authStore.checkAuthIfNeeded()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (to.name === 'login' && authStore.isAuthenticated) {
    return { name: 'dashboard' }
  }
})

export default router
```

---

## 2. Reglas Críticas

| Regla | Detalle |
|---|---|
| Ruta de login | `/auth/login` (nombre `'login'`). NO `/login`. |
| Guard | Usa `useAuthStore()` dentro de `beforeEach` + `await authStore.checkAuthIfNeeded()` |
| Patrón de guard | Retornar objetos/rutas, evitando `next()` |
| Rutas protegidas | Como children del layout wrapper; `home` puede permanecer pública en el template base |
| Ruta fallback | `/:pathMatch(.*)* → redirect APP_ROUTES.DASHBOARD` |
| Lazy loading | SIEMPRE `() => import('@/views/...')` para code splitting |
| Redirect post-login | Guard pasa `{ query: { redirect: to.fullPath } }` al login |

---

## 3. Variantes Productivas Observadas

- En repos productivos antiguos aparece `next()` en guards; en el template estándar se usa retorno (`return { name: 'x' }`) para Vue Router 4.
- En algunos repos la ruta de login es `/login`; en el estándar actual se unifica como `/auth/login` mediante constantes (`APP_ROUTES.LOGIN`).
- Existen validaciones avanzadas de sesión (rehidratación, cooldown post-logout, validación backend); cuando existan en store/composables deben integrarse dentro de `beforeEach` sin romper tipado estricto.

---

## 4. Cómo Agregar una Nueva Ruta

---

```typescript
// Dentro del array children del layout wrapper:
{
  path: 'module/entities',
  name: 'entities-list',
  component: () => import('@/views/module/EntitiesListView.vue'),
  meta: { requiresAuth: true },
},
```


