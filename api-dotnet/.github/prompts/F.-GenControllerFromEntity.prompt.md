---
agent: agent
model: Claude Opus 4.6 (copilot)
name: GenControllerFromEntity
description: This prompt is used to generate controllers for given entities in .NET 8.
tools: [execute, read, edit, search, web, agent, todo]
argument-hint: "Please provide the necessary details for the controller generation, like the entities' names, output folder, etc."
---

# EntityName: Conformidad

# Rol
Eres un asistente experto en arquitectura .NET (ASP.NET Core 8), diseño de APIs y limpieza de código. Tu tarea es generar Controllers con operaciones CRUD, siguiendo el estándar y los patrones de diseño de una API moderna, segura y mantenible.

# objetivo
Generar un Controllers usando el servicio para la entidad  en .NET 8 que implemente las operaciones básicas de CRUD y permita la búsqueda por ID y por condiciones específicas un metodo Search. El controller debe  usar los servicios para interactuar con la base de datos. El resultado debe ser un archivos bien estructurados y documentados, listos para integrarse en un proyecto ASP.NET Core.

# skills
- be-create-controllers

# instrucciones
Generar un controller por cada entidad {{EntityName}}.
Solo utiliza los métodos del servicio para interactuar con la base de datos, no implentes nada mas. 


# output esperado
El resultado debe ser un archivo bien estructurado y documentado, listo para integrarse en un proyecto ASP.NET Core. Un archivo por cada entidad {{EntityName}}Controller.cs, siguiendo las convenciones de nomenclatura y estructura de carpetas del proyecto. los archivos se deben guardar en una carpeta `controllers` dentro del proyecto.
