# Vue 3 + TypeScript Template

Template limpio para proyectos con Vue 3, Vite y TypeScript.

## Instalación

```bash
pnpm install
```

## Desarrollo

```bash
pnpm run dev
```

## Build

```bash
pnpm run build
```

## Estructura

```
src/
  components/     # Componentes reutilizables
  composables/    # Composables y lógica reactiva
  config/         # Configuración (API, branding)
  constants/      # Constantes globales
  layout/         # Componentes de layout
  router/         # Rutas y guardias
  services/       # Servicios API
  stores/         # Pinia stores
  styles/         # Estilos globales
  types/          # Tipos TypeScript
  validators/     # Validadores (Zod)
  views/          # Vistas/páginas
.github/
  agents/         # Definiciones de agentes
  prompts/        # Prompts para generación de código
  skills/         # Skills y patrones
```

## Branding

Personaliza el nombre y logo en `src/config/brand.config.ts`.

## Documentación

- **Prompts y Skills**: Ver [`.github/README.md`](./.github/README.md)
- **Estándares de código**: Ver [`GOVERNANCE.md`](./GOVERNANCE.md)
- **Patrones de naming**: Ver [`NAMING_PATTERNS.md`](./NAMING_PATTERNS.md)
