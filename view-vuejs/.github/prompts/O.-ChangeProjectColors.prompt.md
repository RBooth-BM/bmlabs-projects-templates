---
mode: agent
tools: [read, edit]
---

<role>
Eres un ingeniero senior Vue 3. Tu tarea es cambiar el esquema de colores del proyecto de forma consistente.
</role>

<objective>
Actualizar el color primario de la marca y todos los tokens derivados en los 2 archivos de configuración de colores.
</objective>

<inputs>
El usuario debe proveer:
- **primaryColor**: Nuevo color primario en HEX (ej: `#7c3aed`)
- **primaryDark**: Variante para modo oscuro (ej: `#a78bfa`) — si no se provee, usar versión más clara del primary
- **sidebarBg**: Color del sidebar en modo claro (por defecto igual a primaryColor)
- **brandFirst** (opcional): Primera parte del nombre de marca (ej: "MY")
- **brandLast** (opcional): Segunda parte del nombre de marca (ej: "APP")
</inputs>

<files_to_edit>

### 1. `src/style/style.css`
Actualizar las variables CSS en `:root` y `.dark`:
```css
:root {
  --brand-primary:      {{ primaryColor }};
  --brand-primary-rgb:  {{ R, G, B del primaryColor }};
  --sidebar-bg:         {{ sidebarBg || primaryColor }};
}
.dark {
  --brand-primary:     {{ primaryDark }};
  --brand-primary-rgb: {{ R, G, B del primaryDark }};
  --sidebar-bg:        #0f172a; /* mantener oscuro por defecto */
}
```
**Regla**: Calcular el valor RGB descomponiendo el HEX. Ejemplo: `#7c3aed` → `124, 58, 237`.

### 2. `src/layout/components/AppSidebar.vue`
Actualizar el color hardcodeado en `<style scoped>`:
```css
.sidebar-container {
  background: {{ primaryColor }};
}
```
Buscar la línea `background: #1e3a8a;` (o el valor actual) y reemplazarla.

### 3. `src/config/brand.config.ts` *(solo si se proveen brandFirst/brandLast)*
```typescript
export const BRAND_CONFIG = {
  name: {
    first: '{{ brandFirst }}',
    last: '{{ brandLast }}',
    full: '{{ brandFirst }} {{ brandLast }}'
  },
  ...
}
```

</files_to_edit>

<checklist>
- [ ] Variables `--brand-primary` y `--brand-primary-rgb` actualizadas en `:root`
- [ ] Variables actualizadas en `.dark`
- [ ] `.sidebar-container { background }` actualizado en AppSidebar.vue
- [ ] RGB calculado correctamente a partir del HEX
- [ ] `brand.config.ts` actualizado si se proveyó nombre de marca
- [ ] `npm run build` pasa sin errores TS
</checklist>

<usage_examples>
```
/O-ChangeProjectColors primaryColor=#7c3aed primaryDark=#a78bfa brandFirst=MY brandLast=APP
/O-ChangeProjectColors primaryColor=#059669 primaryDark=#34d399
/O-ChangeProjectColors primaryColor=#dc2626 primaryDark=#f87171 sidebarBg=#7f1d1d
```
</usage_examples>
