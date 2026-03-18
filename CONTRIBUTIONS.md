# Guía de Contribución

¡Gracias por tu interés en contribuir a **BMS Labs - Projects Templates**! Este repositorio se beneficia enormemente de las contribuciones de la comunidad.

## Cómo Contribuir

### Proceso de Contribución

Todas las contribuciones deben realizarse a través de **Pull Requests (PR)**. Esto nos permite mantener la calidad del código y revisar cada cambio antes de integrarlo.

### Pasos para Contribuir

1. **Fork del Repositorio**
   ```bash
   # Hacer fork desde la interfaz de GitHub
   # Luego clonar tu fork
   git clone https://github.com/tu-usuario/bmlabs-projects-templates.git
   cd bmlabs-projects-templates
   ```

2. **Crear una Branch**
   ```bash
   # Crear branch desde main
   git checkout -b feature/nombre-de-tu-feature
   
   # O para correcciones
   git checkout -b fix/descripcion-del-fix
   ```

3. **Realizar Cambios**
   - Mantén los cambios focalizados y atómicos
   - Sigue las convenciones existentes del proyecto
   - Actualiza documentación si es necesario

4. **Commit de Cambios**
   ```bash
   git add .
   git commit -m "Tipo: Descripción clara de los cambios
   
   - Detalle específico 1
   - Detalle específico 2
   - Impacto o beneficio del cambio"
   ```

5. **Push y Crear PR**
   ```bash
   git push origin feature/nombre-de-tu-feature
   ```
   Luego crear el Pull Request desde la interfaz de GitHub.

## Tipos de Contribuciones

### Contribuciones Bienvenidas

- **Nuevos Templates**: Agregar templates para nuevas tecnologías
- **Mejoras de Templates Existentes**: Optimizaciones, nuevas features
- **Documentación**: Mejorar README, guías, comentarios
- **Corrección de Bugs**: Fixes en configuraciones o código
- **Skills de GitHub Copilot**: Nuevos skills o mejoras
- **Configuraciones**: Docker, CI/CD, herramientas de desarrollo

### Áreas Prioritarias

- Completar templates de `app-react` y `view-vuejs`
- Agregar templates para otras tecnologías (Python, Java, etc.)
- Mejorar integración con GitHub Copilot
- Configuraciones de CI/CD para diferentes plataformas
- Tests automatizados para templates

## Estándares de Calidad

### Código y Configuración

- **Consistencia**: Seguir el estilo existente en cada template
- **Documentación**: Código bien comentado y documentado
- **Agnóstico**: Templates deben ser fáciles de personalizar
- **Seguridad**: No incluir credenciales o información sensible

### Documentación

- **README completos**: Cada template debe tener documentación detallada
- **Comentarios explicativos**: En archivos de configuración
- **Ejemplos claros**: Instrucciones paso a paso
- **Español como idioma principal**: Para documentación

### Estructura de Commits

Usa el formato de **Conventional Commits**:

```
tipo(alcance): descripción

- Detalle del cambio 1
- Detalle del cambio 2
```

**Tipos válidos:**
- `feat`: Nueva funcionalidad o template
- `fix`: Corrección de errores
- `docs`: Cambios en documentación
- `style`: Mejoras de formato (sin cambio funcional)
- `refactor`: Refactoring de código
- `test`: Agregar o modificar tests
- `chore`: Mantenimiento (dependencias, configs)

## Proceso de Revisión

### Criterios de Revisión

Los Pull Requests serán revisados considerando:

1. **Funcionalidad**: ¿Los cambios funcionan correctamente?
2. **Calidad**: ¿El código sigue las mejores prácticas?
3. **Documentación**: ¿Está bien documentado?
4. **Impacto**: ¿Mejora la experiencia del desarrollador?
5. **Seguridad**: ¿No introduce vulnerabilidades?

### Tiempo de Revisión

- **PRs simples**: 1-3 días hábiles
- **PRs complejos**: 3-7 días hábiles
- **PRs urgentes**: Serán priorizados según necesidad

## Configuración de Desarrollo

### Prerrequisitos

Según el template en el que trabajes:

- **api-dotnet**: .NET 8+, Entity Framework, Docker
- **app-react**: Node.js, npm/yarn
- **view-vuejs**: Node.js, Vue CLI

### Testing Local

Antes de crear el PR:

1. **Verifica que compile**: Sin errores de sintaxis
2. **Prueba funcionalidad**: Features funcionan como esperado
3. **Revisa documentación**: README actualizado y preciso
4. **Valida seguridad**: Sin credenciales o datos sensibles

## Qué NO Hacer

- **No hacer push directo a main**: Siempre usar Pull Requests
- **No incluir credenciales**: Usar placeholders genéricos
- **No commits gigantes**: Mantener cambios atómicos y focalizados
- **No romper compatibilidad**: Sin cambios breaking sin discusión previa
- **No duplicar código**: Reutilizar configuraciones cuando sea posible

## Comunicación

### Canales de Comunicación

- **Issues**: Para reportar bugs o solicitar features
- **Discussions**: Para preguntas generales o propuestas
- **PR Comments**: Para discusión específica de código

### Etiquetas de Issues

- `bug`: Errores reportados
- `enhancement`: Mejoras propuestas
- `documentation`: Relacionado con documentación
- `good first issue`: Ideal para nuevos contribuyentes
- `help wanted`: Se busca ayuda de la comunidad

## Reconocimiento

Los contribuyentes serán reconocidos en:

- **Contributors GitHub**: Automáticamente listados
- **CHANGELOG**: Menciones en releases importantes
- **README principal**: Hall of fame de contribuyentes destacados

## ¿Preguntas?

Si tienes dudas sobre el proceso de contribución:

1. **Revisa Issues existentes**: Tu pregunta podría estar respondida
2. **Crea un Issue**: Para preguntas específicas
3. **Inicia una Discussion**: Para temas más amplios

---

**¡Gracias por contribuir al crecimiento de BMS Labs Projects Templates!** 

Cada contribución, por pequeña que sea, hace que estos templates sean mejores para toda la comunidad de desarrolladores.