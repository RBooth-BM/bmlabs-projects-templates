# BMS Labs - Projects Templates

Colección de templates y guías para el desarrollo de proyectos utilizando diferentes tecnologías y frameworks. Este repositorio está diseñado para acelerar el proceso de creación de nuevos proyectos proporcionando estructuras base, configuraciones optimizadas y mejores prácticas.

## Templates Disponibles

### 📚 [API .NET](./api-dotnet/)
- **Tecnología**: ASP.NET Core 8+  
- **Propósito**: APIs backend robustas y escalables
- **Características**: 
  - Arquitectura por capas
  - Configuración de Swagger/OpenAPI
  - Logging con Serilog
  - Docker support
  - GitHub Copilot skills integrados

### ⚛️ [App React](./app-react/) 
- **Tecnología**: React
- **Estado**: En desarrollo
- **Propósito**: Aplicaciones web frontend modernas

### 🟢 [View Vue.js](./view-vuejs/)
- **Tecnología**: Vue.js  
- **Estado**: En desarrollo
- **Propósito**: Interfaces de usuario reactivas

## Estructura del Repositorio

```
bmlabs-projects-templates/
├── api-dotnet/          # Template para APIs .NET
├── app-react/           # Template para aplicaciones React
├── view-vuejs/          # Template para vistas Vue.js
├── .gitignore          # Exclusiones globales
└── README.md           # Este archivo
```

## Cómo Usar los Templates

### Método 1: Clonación Completa
```bash
git clone https://github.com/bmslabs/bmlabs-projects-templates.git
cd bmlabs-projects-templates
cp -r api-dotnet/ ../mi-nuevo-proyecto/
```

### Método 2: Descarga Específica
```bash
# Descargar solo el template que necesites
svn export https://github.com/bmslabs/bmlabs-projects-templates/trunk/api-dotnet mi-nuevo-proyecto
```

### Método 3: Template GitHub
- Usa este repositorio como template desde la interfaz de GitHub
- Selecciona solo el directorio que necesites

## Personalización

Cada template incluye:

- **README específico** con instrucciones de personalización
- **Archivos de configuración** comentados y documentados
- **Estructura de proyecto** siguiendo mejores prácticas
- **Guías de desarrollo** con GitHub Copilot (donde aplique)

## Contribuciones

Para contribuir a los templates:

1. Fork este repositorio
2. Crea una branch para tu feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -am 'Agrega nueva funcionalidad'`)
4. Push a la branch (`git push origin feature/nueva-funcionalidad`)
5. Crea un Pull Request

## Convenciones

### Naming
- Carpetas en minúsculas con guiones (`api-dotnet`, `app-react`)
- Archivos siguiendo convenciones de cada tecnología
- Variables y configuraciones en inglés en el código

### Documentación
- README.md detallado en cada template
- Comentarios explicativos en archivos de configuración
- Ejemplos de uso incluidos

### Versionado
- Semantic versioning para releases
- Tags para versiones estables de templates
- Changelog mantenido para cambios importantes

## Tecnologías y Herramientas

- **Control de Versiones**: Git
- **CI/CD**: GitHub Actions (configurado por template)
- **Containerización**: Docker & Docker Compose
- **Documentación**: Markdown con Mermaid diagrams
- **Development**: GitHub Copilot optimized

## Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## Soporte

Para reportar bugs o solicitar features:
- Crea un [issue](https://github.com/bmslabs/bmlabs-projects-templates/issues)
- Etiqueta apropiadamente según el template afectado
- Incluye detalles de reproducción y entorno

---

**Desarrollado por BMS Labs** - Acelerando el desarrollo de software con templates de calidad empresarial.