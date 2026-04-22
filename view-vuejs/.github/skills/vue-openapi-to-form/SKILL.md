---
name: vue-openapi-to-form
description: Transforma especificaciones OpenAPI 3.0 o Schemas de datos en un stack completo de frontend (DTO, API Service, Composable y Formulario) 100% funcional.
license: Proprietary
compatibility: Vue 3, TypeScript, Axios, Tailwind CSS
metadata:
  author: Ricardo B.T.
  version: 1.0.0
---

# Skill: OpenAPI to Form (Automated Stack Generation)

## Propósito
Acelerar el desarrollo transformando definiciones técnicas (OpenAPI/JSON) en una arquitectura de 4 capas (Types, Service, Composable, View) lista para producción, asegurando tipado estricto de extremo a extremo.

## Invocación
```bash
/H # Seguido del schema o descripción de la entidad
```

## Instrucciones / Estándares Aplicados

Si el usuario proporciona un Schema OpenAPI o modelo de datos, entonces DEBES generar el siguiente flujo atómico:

### 0. Análisis del OpenAPI
**ANTES de generar cualquier código, DEBES:**
- Revisar el Swagger/OpenAPI para identificar métodos HTTP correctos (GET, POST, PATCH, PUT, DELETE).
- Mapear formatos OpenAPI a validadores Zod: `string format:uuid` → `z.string().uuid()`, `string format:email` → `z.string().email()`, etc.
- Identificar campos nullable y opcionales según el schema.
- Verificar si hay endpoints separados para Create/Update o unificados.

### 1. Capa de Datos (`entity.types.ts`)
- Define interfaces TypeScript precisas para `RequestDTO` y `ResponseDTO`.
- Mapea tipos de OpenAPI (`integer`, `string format:date`) a tipos TS correspondientes.
- Incluye validadores Zod que respeten formatos OpenAPI (UUID, email, date, etc.).

### 2. Capa de Servicio (`entity.service.ts`)
- Implementa clase estática usando el `httpClient` centralizado.
- **MÉTODO HTTP CORRECTO**: Usa el método especificado en el OpenAPI (POST, PATCH, PUT, etc.).
- Define métodos `static async` para `getPaged`, `getById`, `createOrUpdate` y `delete`.

### 3. Capa de Lógica (`useEntity.ts`)
- Composable de Composition API que encapsule el estado: `loading`, `error`, `save()`.
- Gestiona la reactividad del objeto del formulario.

### 4. Capa de UI (`EntityForm.vue`)
- **Prohibido HTML nativo para acciones**: Usa `AppButton`, `FormSelect`, `FormTextarea`, `BaseModal` de `@/components/shared/`.
- Integra validaciones en tiempo real (RUT, Email, campos obligatorios).
- Aplica el estándar de layout responsivo con Tailwind CSS.

## Checklist de Calidad

- [ ] ¿Revisó el OpenAPI para métodos HTTP y formatos de datos?
- [ ] ¿Se generaron las 4 capas (Types, Api, Composable, Form)?
- [ ] ¿El formulario usa componentes corporativos de `@/components/shared/`?
- [ ] ¿Queda prohibido el uso de `any` en los DTOs?
- [ ] ¿Se gestionan estados de carga (`loading`) en el botón de envío?
- [ ] ¿Los errores de API se capturan y muestran via Toasts/Swal?
- [ ] ¿Mapeó correctamente formatos especiales (Email, Date, UUID)?

## Que Genera
Un módulo completo y listo para ser importado en una vista CRUD, garantizando que la lógica de negocio y la de interfaz estén separadas según los principios SOLID.

## Referencias y Código Reutilizable
Para generar la integración exacta, DEBES consultar el código fuente del patrón real alojado aquí:
- [VER CÓDIGO FUENTE REAL ORIGEN](./references/TEMPLATES_CRUD.md)
