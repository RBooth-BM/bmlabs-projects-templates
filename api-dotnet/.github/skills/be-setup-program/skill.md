---
name: be-setup-program
description: >
  Utiliza este skill cuando necesites configurar el startup de una aplicación .NET, establecer middleware, o estructurar el bootstrap de la API.
  Aplica cuando el usuario mencione "configurar Program.cs", "startup configuration", "middleware pipeline", "inyección de dependencias", "autenticación", "Swagger", "CORS", "DbContext",
  "health checks", "logging", "background services", o cuando necesite conectar todas las capas de la arquitectura — incluso si no dice explícitamente "Program.cs".
  Genera configuración completa de ASP.NET Core con autenticación, EF Core, DI, Swagger, CORS, Serilog, health checks y middleware pipeline optimizado.
argument-hint: 'Describe qué secciones del Program.cs necesitas configurar o generar desde cero'
---

# Program.cs Setup — ASP.NET Core API

## Cuándo usar este skill
Utiliza este skill para generar o completar un archivo `Program.cs` para una API ASP.NET Core 8+. El skill te guiará en la configuración de secciones clave como autenticación Azure AD/JWT, registro de DbContext con EF Core, inyección de dependencias, Swagger, CORS, Serilog, health checks, background services, middleware pipeline, JSON serialization y conexión de capas de la arquitectura. Este skill se basa en las mejores prácticas de configuración y diseño de aplicaciones ASP.NET Core, garantizando que el `Program.cs` generado sea claro, mantenible y esté correctamente configurado para su uso en la aplicación.
- Modificar un `Program.cs` existente para una API .NET 8+.
- Agregar o modificar secciones del pipeline: autenticación, CORS, Swagger, DbContext, DI, etc.
- Integrar nuevas capas (repositorios, servicios, DTOs/mappers/validators) al startup.
- Configurar background services, health checks u opciones de host.
- Revisar el orden correcto del middleware pipeline.

## Estructura general del Program.cs

El archivo `Program.cs` sigue un orden estricto dividido en **tres bloques** principales:

```
1. Bootstrap y logging temprano
2. Configuración del builder (servicios)
3. Configuración de la app (middleware pipeline)
```

---

## Procedimiento paso a paso

### Paso 1 — Bootstrap y Serilog temprano

Configurar Serilog como bootstrap logger **antes** de crear el builder, para capturar errores de inicio. Envolver todo en `try/catch/finally` para garantizar `Log.CloseAndFlush()`.

```csharp
using Serilog;

// Cargar variables de entorno 
DotNetEnv.Env.Load();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Iniciando aplicación");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Reconfigurar Serilog con la configuración completa
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services));

    // ... toda la configuración ...

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error fatal durante el inicio de la aplicación");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
```

**Reglas:**
- Nunca usar `Console.WriteLine` para logging estructurado; usar `ILogger<T>` o `Log`.
- La configuración de Serilog debe leerse desde `appsettings.json` (sección `Serilog`).
- No loggear secretos, tokens, connection strings.

---

### Paso 2 — Inyección de dependencias (módulos DI)

Registrar las dependencias usando las **clases estáticas de extensión DI** que ya existen en el proyecto. El orden no afecta la resolución, pero se recomienda seguir el flujo lógico: DTOs/mappers → helpers → repositories → services.

```csharp
// DTOs, AutoMapper y FluentValidation
DependencyInjectionMappers.AddMappersAndValidators(builder.Services);

// Helpers
DependencyInjectionHelper.AddHelpers(builder.Services);

// Repositorios
DependencyInjectionRepositories.AddRepositories(builder.Services);

// Servicios de aplicación
DependencyInjectionService.AddServices(builder.Services);
```

**Reglas:**
- Cada capa tiene su propia clase `DependencyInjection*` que centraliza el registro.
- Usar `AddScoped` para repositorios y servicios (1 instancia por request HTTP).
- Usar `AddSingleton` solo para configuraciones inmutables o servicios thread-safe.
- Usar `AddTransient` solo para servicios ligeros sin estado.
- Al agregar una nueva entidad, registrar su repositorio y servicio en las clases DI correspondientes.

**Convención de clases DI del proyecto:**

| Clase | Ubicación | Responsabilidad |
|-------|-----------|-----------------|
| `DependencyInjectionMappers` | `core/dtos/DtoDependencyRegistration.cs` | AutoMapper perfiles + FluentValidation validators |
| `DependencyInjectionHelper` | `helpers/DependencyInjectionHelper.cs` | Helpers (JWT, etc.) |
| `DependencyInjectionRepositories` | `core/repositories/DependencyInjectionExtensions.cs` | Todos los repositorios |
| `DependencyInjectionService` | `service/DependencyInjectionService.cs` | Servicios de aplicación |

---

### Paso 3 — Configuración tipada (Options pattern)

Vincular secciones de `appsettings.json` a clases POCO usando `IOptions<T>`, SOLO SI ES NECESARIO y es explicitamente pedido. **Nunca hardcodear** valores de configuración. Ejemplo para configuraciones de S3 y BUK:

```csharp
builder.Services.Configure<S3Config>(
    builder.Configuration.GetSection("S3Config"));

builder.Services.Configure<BukConfig>(
    builder.Configuration.GetSection("BUK"));
```

**Reglas:**
- Las clases de configuración van en `core/config/`.
- Usar `IOptions<T>` (singleton), `IOptionsSnapshot<T>` (scoped) o `IOptionsMonitor<T>` (hot-reload) según la necesidad.
- Los secretos reales deben venir de variables de entorno o secret managers, no de `appsettings.json` en producción.

---

### Paso 4 — HttpClient tipado

Registrar `HttpClient` con la interfaz y su implementación para servicios externos. incluir solo si es neceario y explicitamente pedido. Ejemplo para un servicio externo llamado `BukApiService`:

```csharp
builder.Services.AddHttpClient<IBukApiService, BukApiService>();
```

**Reglas:**
- Siempre usar `IHttpClientFactory` (implícito con `AddHttpClient<TInterface, TImpl>`).
- Configurar timeouts, base address y headers en la lambda de configuración si es necesario.
- No crear instancias de `HttpClient` manualmente (evita socket exhaustion).

---

### Paso 5 — Background services y opciones de host

incluir solo si es necesario y explicitamente pedido. Registrar background services con `AddHostedService<T>()` y configurar opciones de host para manejo de excepciones y tiempos de shutdown. Ejemplo:

```csharp
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    options.ShutdownTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddHostedService<MiBackgroundService>();
```

**Reglas:**
- Los background services deben manejar `CancellationToken` y capturar excepciones propias.
- Registrar con `AddHostedService<T>()`.
- Ajustar `ShutdownTimeout` si procesan trabajos largos.

---

### Paso 6 — Kestrel y seguridad de servidor

```csharp
builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);
```

**Regla:** Siempre deshabilitar el header `Server` para no exponer tecnología.

---

### Paso 7 — DbContext con Entity Framework Core

```csharp
builder.Services.AddDbContext<Context>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Context"));
    // O para sql server
    //options.UseSqlServer(builder.Configuration.GetConnectionString("Context"));
        
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
```

**Reglas:**
- Connection string siempre desde configuración, nunca inline.
- `EnableSensitiveDataLogging` y `EnableDetailedErrors` **solo en Development**.
- El `Context` debe estar definido en `core/entities/Context.cs`.
- Las migraciones se manejan con `dotnet ef migrations add` / `dotnet ef database update`.
- Usar provider acorde a la BD del proyecto (Npgsql para PostgreSQL, SqlServer para SQL Server).

---

### Paso 8 — Controllers y serialización JSON

```csharp
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 64;
    });
```

**Reglas:**
- Usar `System.Text.Json` (por defecto en .NET 8). No agregar Newtonsoft salvo necesidad real.
- `ReferenceHandler.IgnoreCycles` evita errores de serialización por referencias circulares de EF Core.
- Considerar `JsonStringEnumConverter` si se exponen enums como string.
- `AddEnvironmentVariables()` **siempre** presente para sobreescribir configuración por ambiente.

---

### Paso 9 — CORS

```csharp
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomCorsPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins ?? Array.Empty<string>())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
```

**Reglas:**
- Los orígenes permitidos se leen de configuración, **nunca** hardcodeados.
- No usar `AllowAnyOrigin()` en producción.
- Si se usa `AllowCredentials()`, no se puede combinar con `AllowAnyOrigin()`.
- Definir la política con un nombre y referenciarla en `app.UseCors("CustomCorsPolicy")`.

---

### Paso 10 — Autenticación

La autenticación se configura de forma diferenciada por ambiente.

#### Desarrollo (autenticación de prueba)

```csharp
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddAuthentication("Test")
        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
            "Test", options => { });
}
```

Requiere un `TestAuthenticationHandler` al final del archivo o en una clase separada:

```csharp
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```

#### Producción (Azure AD + Cookie + OpenIdConnect + JWT Bearer)

```csharp
else
{
    var azureAd = builder.Configuration.GetSection("AzureAd");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/api/v1/Auth/login";
        options.LogoutPath = "/api/v1/Auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = $"{azureAd["Instance"]}{azureAd["TenantId"]}/v2.0";
        options.ClientId = azureAd["ClientId"];
        options.ClientSecret = azureAd["ClientSecret"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.CallbackPath = azureAd["CallbackPath"];
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = $"{azureAd["Instance"]}{azureAd["TenantId"]}/v2.0";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = azureAd["ClientId"],
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = $"{azureAd["Instance"]}{azureAd["TenantId"]}/v2.0",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
}
```

**Reglas:**
- Toda config de Azure AD se lee de `appsettings.json` sección `AzureAd`.
- Nunca hardcodear `ClientId`, `ClientSecret`, `TenantId`.
- En APIs REST puras, preferir solo JWT Bearer. Cookie + OIDC es para apps con flujo de login web.
- `OnRedirectToLogin` debe retornar 401 en vez de redirigir (comportamiento API).

---

### Paso 11 — Swagger / OpenAPI

```csharp
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Mi API",
        Description = "Descripción de la API"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization. Ingrese 'Bearer [token]'."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpContextAccessor();
```

**Reglas:**
- Incluir `AddSecurityDefinition` + `AddSecurityRequirement` para que el botón Authorize funcione en Swagger UI.
- `AddHttpContextAccessor` permite inyectar `IHttpContextAccessor` para acceder al contexto HTTP (claims, tokens) desde servicios.
- Documentar todos los endpoints con atributos `[ProducesResponseType]` en los controllers.

---

### Paso 12 — Construir la app y configurar el pipeline

El **orden del middleware es crítico**. Respetar esta secuencia:

```csharp
var app = builder.Build();

// 1. PathBase (si la API corre bajo un sub-path), si no es explicito dejar vacio o omitirlo,
var pathBase = "/mi_api";
app.UsePathBase(pathBase);

// 2. Routing
app.UseRouting();

// 3. Exception handling y Swagger (según ambiente)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
}

// 4. Swagger disponible en todos los ambientes (opcional)
app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", pathBase);
    c.RoutePrefix = "swagger";
    c.DefaultModelsExpandDepth(-1);
});

// 5. HTTPS y archivos estáticos
app.UseHttpsRedirection();
app.UseStaticFiles();

// 6. CORS (DEBE ir antes de Auth)
app.UseCors("CustomCorsPolicy");

// 7. Autenticación y autorización (en este orden exacto)
app.UseAuthentication();
app.UseAuthorization();

// 8. Map controllers
app.MapControllers();
```

**Orden obligatorio del middleware:**
1. `UsePathBase` (si aplica)
2. `UseRouting`
3. `UseDeveloperExceptionPage` / `UseExceptionHandler`
4. `UseSwagger` / `UseSwaggerUI`
5. `UseHttpsRedirection`
6. `UseStaticFiles` (si aplica)
7. `UseCors` ← antes de Auth
8. `UseAuthentication` ← antes de Authorization
9. `UseAuthorization`
10. `MapControllers`
11. Health checks endpoints

---

### Paso 13 — Health checks

```csharp
app.MapGet("/liveness", async context =>
{
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync("OK");
}).WithName("liveness").WithOpenApi();

app.MapGet("/readiness", async context =>
{
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync("OK");
}).WithName("readiness").WithOpenApi();
```

**Reglas:**
- `/liveness`: indica que el proceso está vivo.
- `/readiness`: indica que puede recibir tráfico (aquí se puede validar conexión a BD, etc.).
- No mezclar lógica de negocio en health checks.
- Considerar usar `builder.Services.AddHealthChecks()` y `app.MapHealthChecks()` para checks más avanzados.

---

## Checklist de validación

Antes de dar por completado el `Program.cs`, verificar:

- [ ] Serilog configurado como bootstrap + lectura desde `appsettings.json`.
- [ ] Todas las clases DI registradas: Mappers, Helpers, Repositories, Services.
- [ ] Configuraciones tipadas con `IOptions<T>` para cada sección.
- [ ] HttpClient registrado con `AddHttpClient<TInterface, TImpl>` para servicios externos.
- [ ] Background services registrados con `AddHostedService<T>`.
- [ ] Header `Server` deshabilitado en Kestrel.
- [ ] DbContext registrado con connection string desde configuración.
- [ ] `EnableSensitiveDataLogging` solo en Development.
- [ ] Controllers con serialización JSON configurada.
- [ ] CORS con orígenes desde configuración, no hardcodeados.
- [ ] Autenticación diferenciada por ambiente (Test en dev, Azure AD + JWT en prod).
- [ ] Swagger con security definition para JWT Bearer.
- [ ] `AddHttpContextAccessor` presente.
- [ ] Pipeline de middleware en el orden correcto.
- [ ] Health checks `/liveness` y `/readiness`.
- [ ] `try/catch/finally` con `Log.CloseAndFlush()`.
- [ ] Sin secretos, tokens ni connection strings hardcodeados.
- [ ] Compila sin errores.

---

## Template completo de referencia

```csharp
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Serilog;

// ── 1. Bootstrap ──────────────────────────────────────────────
DotNetEnv.Env.Load();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Iniciando aplicación");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services));

    var serviceName = "api";
    var azureAd = builder.Configuration.GetSection("AzureAd");

    // ── 2. Dependency Injection ───────────────────────────────
    DependencyInjectionMappers.AddMappersAndValidators(builder.Services);
    DependencyInjectionHelper.AddHelpers(builder.Services);
    DependencyInjectionRepositories.AddRepositories(builder.Services);
    DependencyInjectionService.AddServices(builder.Services);

    // ── 3. Configuración tipada ───────────────────────────────
    // builder.Services.Configure<MiConfig>(
    //     builder.Configuration.GetSection("MiSeccion"));

    // ── 4. HttpClient tipado ──────────────────────────────────
    // builder.Services.AddHttpClient<IMiExternalService, MiExternalService>();

    // ── 5. Background services ────────────────────────────────
    builder.Services.Configure<HostOptions>(options =>
    {
        options.BackgroundServiceExceptionBehavior =
            BackgroundServiceExceptionBehavior.Ignore;
        options.ShutdownTimeout = TimeSpan.FromMinutes(10);
    });
    // builder.Services.AddHostedService<MiBackgroundService>();

    // ── 6. Kestrel ────────────────────────────────────────────
    builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

    // ── 7. DbContext ──────────────────────────────────────────
    builder.Services.AddDbContext<Context>(options =>
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("Context"));

        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });

    // ── 8. Controllers y JSON ─────────────────────────────────
    builder.Configuration.AddEnvironmentVariables();
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Encoder =
                JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            options.JsonSerializerOptions.ReferenceHandler =
                ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.MaxDepth = 64;
        });

    // ── 9. CORS ───────────────────────────────────────────────
    var allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins").Get<string[]>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CustomCorsPolicy", policy =>
        {
            policy
                .WithOrigins(allowedOrigins ?? Array.Empty<string>())
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    // ── 10. Autenticación ─────────────────────────────────────
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddAuthentication("Test")
            .AddScheme<Microsoft.AspNetCore.Authentication
                .AuthenticationSchemeOptions,
                TestAuthenticationHandler>("Test", options => { });
    }
    else
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme =
                CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme =
                OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/api/v1/Auth/login";
            options.LogoutPath = "/api/v1/Auth/logout";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
            options =>
        {
            options.Authority =
                $"{azureAd["Instance"]}{azureAd["TenantId"]}/v2.0";
            options.ClientId = azureAd["ClientId"];
            options.ClientSecret = azureAd["ClientSecret"];
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true;
            options.CallbackPath = azureAd["CallbackPath"];
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority =
                $"{azureAd["Instance"]}{azureAd["TenantId"]}/v2.0";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = azureAd["ClientId"],
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer =
                    $"{azureAd["Instance"]}{azureAd["TenantId"]}/v2.0",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
    }

    // ── 11. Swagger ───────────────────────────────────────────
    builder.Services.AddAuthorization();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = serviceName,
            Description = serviceName
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "JWT Authorization. Ingrese 'Bearer [token]'."
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddHttpContextAccessor();

    // ── 12. Build app & pipeline ──────────────────────────────
    var app = builder.Build();
    var pathBase = $"/{serviceName}";
    app.UsePathBase(pathBase);
    app.UseRouting();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", pathBase);
        c.RoutePrefix = "swagger";
        c.DefaultModelsExpandDepth(-1);
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCors("CustomCorsPolicy");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // ── 13. Health checks ─────────────────────────────────────
    app.MapGet("/liveness", async context =>
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("OK");
    }).WithName("liveness").WithOpenApi();

    app.MapGet("/readiness", async context =>
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("OK");
    }).WithName("readiness").WithOpenApi();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error fatal durante el inicio de la aplicación");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
```

---

## Errores comunes a evitar

| Error | Corrección |
|-------|-----------|
| `UseAuthentication` después de `UseAuthorization` | Auth siempre primero |
| `UseCors` después de `UseAuthentication` | CORS debe ir antes de Auth |
| `EnableSensitiveDataLogging` en producción | Condicionar a `IsDevelopment()` |
| `AllowAnyOrigin()` con `AllowCredentials()` | Incompatibles; usar `WithOrigins()` |
| Connection string inline en el código | Usar `GetConnectionString()` desde configuración |
| Falta `AddHttpContextAccessor` y se inyecta `IHttpContextAccessor` | Registrar explícitamente |
| Swagger sin `AddSecurityDefinition` para JWT | El botón Authorize no funciona |
| Background service que falla sin log | Capturar excepciones y loggear con `ILogger` |
| No llamar `Log.CloseAndFlush()` al cerrar | Perder logs pendientes en el buffer |

---

## Paquetes NuGet requeridos

Para que el `Program.cs` compile correctamente, el `.csproj` debe incluir:

```xml
<PackageReference Include="Serilog.AspNetCore" />
<PackageReference Include="Serilog.Sinks.Console" />
<PackageReference Include="Serilog.Sinks.File" />
<PackageReference Include="DotNetEnv" />
<PackageReference Include="Microsoft.EntityFrameworkCore" />
<!-- Elegir el provider correcto: -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
<!-- o -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" />
<PackageReference Include="Swashbuckle.AspNetCore" />
```
