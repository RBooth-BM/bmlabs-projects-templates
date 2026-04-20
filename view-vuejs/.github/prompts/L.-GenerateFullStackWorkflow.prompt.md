---
agent: copilot
name: L.-GenerateFullStackWorkflow
description: Orquestador CRUD completo (6 pasos en orden)
---

# Workflow: Swagger JSON → Full Stack CRUD

**Flujo (6 pasos, ejecutar en orden):**
1. **Types** → `src/types/[entity].types.ts` (ver skill A-CreateComponent)
2. **Validator** → `src/validators/[entity].validator.ts` (ver skill G-CreateValidator)
3. **Service** → `src/services/api/services/[entity].service.ts` (ver skill B-CreateApiService)
4. **Composable** → `src/composables/use[Entity].ts` (ver skill D-CreateComposables)
5. **Modal** → `src/views/.../CreateEdit[Entity]Modal.vue` (ver skill H-CreateForm)
6. **View** → `src/views/.../[Entity]View.vue` (ver skill C-CreateDataGrid)

**Quality Gate**: Valida con `npm run build` (cero `any`, cero TS errors).

**Instrucción**: Lee solo el skill de cada paso. No repitas reglas aquí; ve a la skill.
