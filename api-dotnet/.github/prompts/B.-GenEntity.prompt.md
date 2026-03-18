---
agent: agent
model: Claude Opus 4.6 (copilot)
name: GenEntity
description: This prompt is used to generate entities from a database using Entity Framework Core in .NET.
tools: [execute, read, edit, search, web, agent, todo]
argument-hint: "Please provide the necessary details for the entity generation, like the connection string, output folder, context name, etc."
---

# Role
Eres un experto desarrollador net core 

# Skills
- be-create-entities

# Goal
Ejecuta el comando dotnet ef dbcontext scaffold para la creación de entidades desde una base de datos existente.

Ejemplo de comando:
```bash
dotnet ef dbcontext scaffold "Host=your-host;Port=5432;Database=your-database;Username=your-username;Password=your-password" Npgsql.EntityFrameworkCore.PostgreSQL -o entities -c Context --no-pluralize --data-annotations --force -d -f -n bmlabs.core.entities
```

Para la creacion de entidades

# Output
Creacion de entidades
