# BMS Labs - AI Playbook

Colección de templates y playbooks de desarrollo asistido con **Claude Code**, diseñados para acelerar la construcción de proyectos robustos, seguros y mantenibles en distintas tecnologías.

Cada template incluye un playbook especializado (instrucciones, skills y contexto de proyecto) que Claude Code lee automáticamente para generar código consistente y de calidad desde el primer día.

---

## Módulos

| Módulo | Tecnología | Descripción |
|---|---|---|
| [`api-dotnet/`](api-dotnet/README.md) | ASP.NET Core (.NET 10+) | Template para APIs backend RESTful con arquitectura por capas |
| [`app-react/`](app-react/README.md) | React | Template para aplicaciones móviles React |
| [`view-vuejs/`](view-vuejs/README.md) | Vue 3 + TypeScript | Template para frontends web con Vue 3, Pinia y Zod |

---

## Cómo usar un template

Cada módulo está diseñado para ser incluido como **submódulo Git** en tu proyecto. El submódulo se vincula en `.claude/`, que es donde Claude Code busca las instrucciones y skills del playbook.

```bash
# Para la API .NET
git submodule add -b claude https://github.com/bmslabs/bmlabs-ai-playbook-api-dotnet .claude

# Para el frontend Vue 3
git submodule add -b claude https://github.com/bmslabs/bmlabs-ai-playbook-app-vuejs .claude
```

Una vez integrado, copiar `CLAUDE.md` a la raíz del proyecto para que Claude Code lo tome como fuente de instrucciones globales:

```bash
cp .claude/CLAUDE.md ./CLAUDE.md
```

> El skill `/be-genesis` y `/fe-genesis` automatizan estos pasos al inicializar un proyecto nuevo.

---

## api-dotnet

Template para construir APIs backend con **ASP.NET Core**. Implementa arquitectura por capas con patrones consolidados y desarrollo guiado por Claude Code.

**Tecnologías principales:** .NET 10+ · Entity Framework Core · AutoMapper · FluentValidation · Swagger · PostgreSQL

**Flujo de trabajo:**

```
Entity → DTOs → Validators → Mappings → Repository → Service → Controller
```

**Skills disponibles:**

| Skill | Descripción |
|---|---|
| `/be-genesis` | Bootstrap completo del proyecto: estructura, Git, playbook y CI/CD |
| `/be-create-entities` | Entidades EF Core con timestamps UTC y Guid |
| `/be-create-dtos` | Request/Response DTOs con Data Annotations |
| `/be-create-validators` | Reglas FluentValidation |
| `/be-create-mappings` | Perfiles AutoMapper Entity ↔ DTO |
| `/be-create-repository` | Repositorio con patrón estándar + AsNoTracking |
| `/be-create-service` | Service con orquestación de repositorios |
| `/be-create-controller` | Controller delgado REST con Swagger |
| `/be-setup-docker-compose` | docker-compose.yml con PostgreSQL + .env |

**Primer paso:** ejecutar el skill Genesis para inicializar el proyecto:

```
/be-genesis projectName=empresa-proyecto-api
```

---

## app-react

Template para construir **aplicaciones móviles con React**. Estructurado para ser robusto, seguro y mantenible.

---

## view-vuejs

Template para construir frontends web con **Vue 3 + TypeScript**. Incluye skills para generación de módulos completos desde cero o desde un contrato OpenAPI.

**Tecnologías principales:** Vue 3 · TypeScript estricto · Pinia · Vue Router · Zod · Tailwind CSS v4

**Flujo recomendado desde OpenAPI:**

```
/fe-genesis → /fe-openapi-to-form → /fe-create-datagrid → módulo completo
```

**Skills disponibles:**

| Skill | Descripción |
|---|---|
| `/fe-genesis` | Bootstrap completo: scaffold, Tailwind, Pinia, Docker y calidad |
| `/fe-create-api-service` | Clase estática con `httpClient` tipado |
| `/fe-create-composables` | Composable reactivo con estado y lógica de negocio |
| `/fe-create-datagrid` | Vista CRUD completa con patrón 12/3, filtros y paginación |
| `/fe-openapi-to-form` | Form + DTO + Validator + Service desde schema OpenAPI |
| `/fe-create-auth-forms` | Vistas Login/Signup con Zod + sessionStorage |
| `/fe-create-protected-routes` | Vue Router con auth guards |

---

## Contribución

Todas las contribuciones se realizan a través de **Pull Requests**. Consulta [CONTRIBUTIONS.md](CONTRIBUTIONS.md) para el proceso detallado.

```bash
# 1. Fork + clonar
git clone https://github.com/tu-usuario/bmlabs-projects-templates.git

# 2. Crear branch
git checkout -b feature/nombre-de-tu-feature

# 3. Push + abrir PR
git push origin feature/nombre-de-tu-feature
```
