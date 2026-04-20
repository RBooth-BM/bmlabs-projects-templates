---
name: vue-openapi-direct-codegen
description: Generación guiada desde OpenAPI para producir archivos Vue/TS de stack completo.
license: Proprietary
compatibility: Vue 3, TypeScript, Zod, Pinia, Swagger/OpenAPI
metadata:
  author: BM
  version: 1.0.0
---

# Skill: OpenAPI Direct Codegen

## Propósito
Estandarizar la generación de artefactos frontend desde esquemas OpenAPI con mínimo retrabajo manual.

## Flujo
1. Leer schema/endpoint de `swagger.json`
2. Generar types (`[entity].types.ts`)
3. Generar validator Zod (`[entity].validator.ts`)
4. Generar API service estático (`[entity].service.ts`)
5. Generar composable (`use[Entity].ts`)
6. Generar modal (`CreateEdit[Entity]Modal.vue`)
7. Generar view CRUD 12/3 (`[Entity]View.vue`)

## Reglas
- Tipado estricto sin `any`
- Métodos HTTP consistentes con OpenAPI
- Uso de componentes compartidos
- Validación en español con Zod
- Compatibilidad dark mode

## Checklist
- [ ] 0 `any`
- [ ] Build sin errores (`npm run build`)
- [ ] Estructura de carpetas estándar BM
- [ ] Nomenclatura consistente con `NAMING_PATTERNS.md`
