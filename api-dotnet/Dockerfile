# Dockerfile para API .NET Template
# Este es un Dockerfile agnóstico diseñado como referencia para proyectos .NET API
# Modifica los valores según las necesidades específicas de tu proyecto

# =============================================================================
# STAGE 1: Build Stage
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Exponer puerto para desarrollo (ajustar según necesidades)
EXPOSE 8080
EXPOSE 8081

# Copiar archivos de proyecto primero (para aprovechar Docker layer caching)
# Reemplazar "YourProject.csproj" con el nombre real del proyecto
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos del código fuente
COPY . ./

# Compilar la aplicación
# Reemplazar "YourProject" con el nombre real del proyecto
RUN dotnet publish -c Release -o /app/publish --no-restore

# =============================================================================
# STAGE 2: Runtime Stage  
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Crear usuario no-root para seguridad
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Instalar certificados CA actualizados para HTTPS
RUN apt-get update && apt-get install -y ca-certificates && rm -rf /var/lib/apt/lists/*

# Copiar archivos compilados desde el stage de build
COPY --from=build /app/publish .

# Cambiar propietario de archivos al usuario no-root
RUN chown -R appuser:appuser /app
USER appuser

# Variables de entorno recomendadas para producción
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_USE_POLLING_FILE_WATCHER=false
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Puerto por defecto (ajustar según necesidades)
EXPOSE 8080

# Comando de inicio de la aplicación
# Reemplazar "YourProject.dll" con el nombre real del assembly principal
ENTRYPOINT ["dotnet", "YourProject.dll"]

# =============================================================================
# COMENTARIOS Y CONFIGURACIONES ADICIONALES
# =============================================================================

# Para usar diferentes puertos:
# ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
# EXPOSE 5000
# EXPOSE 5001

# Para configuración de logging:
# ENV Logging__LogLevel__Default=Information
# ENV Logging__LogLevel__Microsoft=Warning

# Para configuración de base de datos (ejemplo):
# ENV ConnectionStrings__DefaultConnection="Server=db;Database=YourDB;User Id=sa;Password=YourPassword;"

# Para configuración de CORS en desarrollo:
# ENV ASPNETCORE_ENVIRONMENT=Development

# Volúmenes para persistencia de datos (opcional):
# VOLUME ["/app/data"]

# Para usar con Docker Compose, crear docker-compose.yml:
# version: '3.8'
# services:
#   api:
#     build: .
#     ports:
#       - "8080:8080"
#     environment:
#       - ASPNETCORE_ENVIRONMENT=Development
#       - ConnectionStrings__DefaultConnection=Server=db;Database=YourDB;...
#     depends_on:
#       - db
#   db:
#     image: mcr.microsoft.com/mssql/server:2022-latest
#     environment:
#       - SA_PASSWORD=YourPassword123
#       - ACCEPT_EULA=Y

# =============================================================================
# INSTRUCCIONES DE CONSTRUCCIÓN Y USO
# =============================================================================

# Para construir la imagen:
# docker build -t your-api-name:latest .

# Para ejecutar el contenedor:
# docker run -p 8080:8080 your-api-name:latest

# Para desarrollo con variables de entorno:
# docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development your-api-name:latest

# Para usar con base de datos externa:
# docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="YourConnectionString" your-api-name:latest


#---
# EJEMPLO COMPLETO DE DOCKERFILE PARA API .NET

# FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as build
# WORKDIR /app
# ARG PROJECT=./proyecto-api.csproj
# COPY $PROJECT ./
# RUN dotnet restore $PROJECT
# COPY . ./
# RUN dotnet publish $PROJECT -c Release -o out

# FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine as runtime 
# WORKDIR /app
# EXPOSE 8080
# RUN apk add --no-cache tzdata icu-libs
# ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
# COPY --from=build /app/out .

# ENTRYPOINT ["dotnet", "proyecto-api.dll"]
