//=============================================================================
// PROGRAM.CS - API .NET TEMPLATE
//=============================================================================
// 
// Este archivo configura y ejecuta la aplicación Web API ASP.NET Core.
// Está diseñado como template base para proyectos de API siguiendo mejores 
// prácticas de arquitectura, seguridad y observabilidad.
//
// ESTRUCTURA:
// 1. Configuración de servicios (Dependency Injection)
// 2. Configuración del pipeline HTTP (Middleware)
// 3. Ejecución de la aplicación
//
// PERSONALIZACIÓN:
// - Reemplaza "YourApi" con el nombre de tu proyecto
// - Configura connection strings según tu base de datos
// - Adapta configuraciones según necesidades específicas
//=============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

//=============================================================================
// CONFIGURACIÓN DE LOGGING INICIAL
//=============================================================================
// Configurar Serilog como logger principal para capturar logs de inicio
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Iniciando aplicación YourApi...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    //=========================================================================
    // CONFIGURACIÓN DE LOGGING CON SERILOG
    //=========================================================================
    // Reemplaza el logging por defecto de ASP.NET Core con Serilog
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());

    //=========================================================================
    // CONFIGURACIÓN DE BASE DE DATOS
    //=========================================================================
    // Configura Entity Framework con el provider de base de datos
    // Cambia UseSqlServer por el provider que necesites:
    // - UseNpgsql() para PostgreSQL
    // - UseMySql() para MySQL  
    // - UseSqlite() para SQLite
//=============================================================================
// PROGRAM.CS - API .NET TEMPLATE
//=============================================================================
// 
// Este archivo configura y ejecuta la aplicación Web API ASP.NET Core.
// Está diseñado como template base para proyectos de API siguiendo mejores 
// prácticas de arquitectura, seguridad y observabilidad.
//
// ESTRUCTURA:
// 1. Configuración de servicios (Dependency Injection)
// 2. Configuración del pipeline HTTP (Middleware)
// 3. Ejecución de la aplicación
//
// PERSONALIZACIÓN:
// - Reemplaza "YourApi" con el nombre de tu proyecto
// - Configura connection strings según tu base de datos
// - Adapta configuraciones según necesidades específicas
//=============================================================================

//=============================================================================
// CONFIGURACIÓN DE LOGGING INICIAL
//=============================================================================
// Configurar Serilog como logger principal para capturar logs de inicio
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Iniciando aplicación YourApi...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    //=========================================================================
    // CONFIGURACIÓN DE LOGGING CON SERILOG
    //=========================================================================
    // Reemplaza el logging por defecto de ASP.NET Core con Serilog
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());

    //=========================================================================
    // CONFIGURACIÓN DE BASE DE DATOS (OPCIONAL - DESCOMENTA CUANDO TENGAS CONTEXTO)
    //=========================================================================
    // Configura Entity Framework con el provider de base de datos
    // Descomenta cuando tengas tu DbContext configurado:
    //
    // builder.Services.AddDbContext<YourDbContext>(options =>
    // {
    //     var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    //     options.UseSqlServer(connectionString); // Cambia por el provider que necesites
    //
    //     if (builder.Environment.IsDevelopment())
    //     {
    //         options.EnableSensitiveDataLogging();
    //         options.EnableDetailedErrors();
    //     }
    // });

    //=========================================================================
    // CONFIGURACIÓN DE CONTROLLERS Y API
    //=========================================================================
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Configuración de serialización JSON
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

    //=========================================================================
    // CONFIGURACIÓN DE SWAGGER/OPENAPI
    //=========================================================================
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "YourApi",
            Version = "v1",
            Description = "API desarrollada con ASP.NET Core template",
            Contact = new OpenApiContact
            {
                Name = "Tu Equipo de Desarrollo",
                Email = "team@yourcompany.com"
            }
        });

        // Configurar autenticación JWT en Swagger (opcional)
        // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        // {
        //     Description = "JWT Authorization header usando Bearer scheme",
        //     Name = "Authorization", 
        //     In = ParameterLocation.Header,
        //     Type = SecuritySchemeType.ApiKey,
        //     Scheme = "Bearer"
        // });

        // Incluir comentarios XML en Swagger (opcional)
        // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        // c.IncludeXmlComments(xmlPath);
    });

    //=========================================================================
    // CONFIGURACIÓN DE CORS
    //=========================================================================
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowedOrigins", policy =>
        {
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? new[] { "http://localhost:3000" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    //=========================================================================
    // CONFIGURACIÓN DE SERVICIOS DE APLICACIÓN
    //=========================================================================
    // Registra tus servicios, repositories, etc. aquí
    
    // Ejemplo de AutoMapper (descomentar si lo usas)
    // builder.Services.AddAutoMapper(typeof(Program));
    
    // Ejemplo de FluentValidation (descomentar si lo usas)  
    // builder.Services.AddFluentValidationAutoValidation();
    // builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    // Registrar tus servicios personalizados
    // builder.Services.AddScoped<IYourService, YourService>();
    // builder.Services.AddScoped<IYourRepository, YourRepository>();

    //=========================================================================
    // CONFIGURACIÓN DE AUTENTICACIÓN (OPCIONAL)
    //=========================================================================
    // Descomenta y configura según tus necesidades de autenticación
    
    // JWT Authentication
    // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //     .AddJwtBearer(options =>
    //     {
    //         options.TokenValidationParameters = new TokenValidationParameters
    //         {
    //             ValidateIssuer = true,
    //             ValidateAudience = true,
    //             ValidateLifetime = true,
    //             ValidateIssuerSigningKey = true,
    //             ValidIssuer = builder.Configuration["Jwt:Issuer"],
    //             ValidAudience = builder.Configuration["Jwt:Audience"],
    //             IssuerSigningKey = new SymmetricSecurityKey(
    //                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    //         };
    //     });

    //=========================================================================
    // CONFIGURACIÓN DE HEALTH CHECKS (OPCIONAL)
    //=========================================================================
    // builder.Services.AddHealthChecks();

    //=========================================================================
    // CONSTRUCCIÓN DE LA APLICACIÓN
    //=========================================================================
    var app = builder.Build();

    //=========================================================================
    // CONFIGURACIÓN DEL PIPELINE HTTP (MIDDLEWARE)
    //=========================================================================
    
    // Configurar Swagger solo en desarrollo por defecto
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "YourApi v1");
            c.RoutePrefix = string.Empty; // Swagger en la raíz (opcional)
        });
    }

    // Middleware de logging de requests HTTP
    app.UseSerilogRequestLogging();

    // Middleware de seguridad
    app.UseHttpsRedirection();

    // Middleware de CORS (debe ir antes de UseAuthentication)
    app.UseCors("AllowedOrigins");

    // Middleware de autenticación y autorización (si está configurado)
    // app.UseAuthentication();
    // app.UseAuthorization();

    // Mapear controllers
    app.MapControllers();

    // Health check endpoint (si está configurado)
    // app.MapHealthChecks("/health");

    //=========================================================================
    // CONFIGURACIONES ADICIONALES PARA PRODUCCIÓN
    //=========================================================================
    if (app.Environment.IsProduction())
    {
        // Configuraciones específicas de producción
        app.UseHsts(); // HTTP Strict Transport Security
        
        // Custom error handling middleware
        // app.UseExceptionHandler("/Error");
    }

    //=========================================================================
    // INICIALIZACIÓN DE BASE DE DATOS (OPCIONAL)  
    //=========================================================================
    // Ejecutar migraciones automáticamente al iniciar (solo si es necesario)
    // using (var scope = app.Services.CreateScope())
    // {
    //     var context = scope.ServiceProvider.GetRequiredService<YourDbContext>();
    //     context.Database.Migrate();
    // }

    //=========================================================================
    // EJECUCIÓN DE LA APLICACIÓN
    //=========================================================================
    Log.Information("Aplicación iniciada correctamente en {Environment}", app.Environment.EnvironmentName);
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar");
}
finally
{
    Log.CloseAndFlush();
}

//=============================================================================
// EJEMPLO DE DBCONTEXT PLACEHOLDER (OPCIONAL)
//=============================================================================
// Descomenta y modifica cuando necesites Entity Framework
//
// using Microsoft.EntityFrameworkCore;
//
// public class YourDbContext : DbContext
// {
//     public YourDbContext(DbContextOptions<YourDbContext> options) : base(options)
//     {
//     }
//
//     // Agrega tus DbSets aquí
//     // public DbSet<YourEntity> YourEntities { get; set; }
//
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         base.OnModelCreating(modelBuilder);
//         
//         // Configuraciones de entidades aquí
//         // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
//     }
// }
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);

        // Configuraciones adicionales para desarrollo
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging(); // Solo en desarrollo
            options.EnableDetailedErrors();       // Solo en desarrollo
        }
    };

    //=========================================================================
    // CONFIGURACIÓN DE CONTROLLERS Y API
    //=========================================================================
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Configuración de serialización JSON
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

    //=========================================================================
    // CONFIGURACIÓN DE SWAGGER/OPENAPI
    //=========================================================================
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "YourApi",
            Version = "v1",
            Description = "API desarrollada con ASP.NET Core template",
            Contact = new OpenApiContact
            {
                Name = "Tu Equipo de Desarrollo",
                Email = "team@yourcompany.com"
            }
        });

        // Configurar autenticación JWT en Swagger (opcional)
        // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        // {
        //     Description = "JWT Authorization header usando Bearer scheme",
        //     Name = "Authorization", 
        //     In = ParameterLocation.Header,
        //     Type = SecuritySchemeType.ApiKey,
        //     Scheme = "Bearer"
        // });

        // Incluir comentarios XML en Swagger (opcional)
        // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        // c.IncludeXmlComments(xmlPath);
    });

    //=========================================================================
    // CONFIGURACIÓN DE CORS
    //=========================================================================
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowedOrigins", policy =>
        {
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? new[] { "http://localhost:3000" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    //=========================================================================
    // CONFIGURACIÓN DE SERVICIOS DE APLICACIÓN
    //=========================================================================
    // Registra tus servicios, repositories, etc. aquí
    
    // Ejemplo de AutoMapper (descomentar si lo usas)
    // builder.Services.AddAutoMapper(typeof(Program));
    
    // Ejemplo de FluentValidation (descomentar si lo usas)  
    // builder.Services.AddFluentValidationAutoValidation();
    // builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    // Registrar tus servicios personalizados
    // builder.Services.AddScoped<IYourService, YourService>();
    // builder.Services.AddScoped<IYourRepository, YourRepository>();

    //=========================================================================
    // CONFIGURACIÓN DE AUTENTICACIÓN (OPCIONAL)
    //=========================================================================
    // Descomenta y configura según tus necesidades de autenticación
    
    // JWT Authentication
    // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //     .AddJwtBearer(options =>
    //     {
    //         options.TokenValidationParameters = new TokenValidationParameters
    //         {
    //             ValidateIssuer = true,
    //             ValidateAudience = true,
    //             ValidateLifetime = true,
    //             ValidateIssuerSigningKey = true,
    //             ValidIssuer = builder.Configuration["Jwt:Issuer"],
    //             ValidAudience = builder.Configuration["Jwt:Audience"],
    //             IssuerSigningKey = new SymmetricSecurityKey(
    //                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    //         };
    //     });

    //=========================================================================
    // CONFIGURACIÓN DE HEALTH CHECKS (OPCIONAL)
    //=========================================================================
    // builder.Services.AddHealthChecks()
    //     .AddDbContext<YourDbContext>();

    //=========================================================================
    // CONSTRUCCIÓN DE LA APLICACIÓN
    //=========================================================================
    var app = builder.Build();

    //=========================================================================
    // CONFIGURACIÓN DEL PIPELINE HTTP (MIDDLEWARE)
    //=========================================================================
    
    // Configurar Swagger solo en desarrollo por defecto
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "YourApi v1");
            c.RoutePrefix = string.Empty; // Swagger en la raíz (opcional)
        });
    }

    // Middleware de logging de requests HTTP
    app.UseSerilogRequestLogging();

    // Middleware de seguridad
    app.UseHttpsRedirection();

    // Middleware de CORS (debe ir antes de UseAuthentication)
    app.UseCors("AllowedOrigins");

    // Middleware de autenticación y autorización (si está configurado)
    // app.UseAuthentication();
    // app.UseAuthorization();

    // Mapear controllers
    app.MapControllers();

    // Health check endpoint (si está configurado)
    // app.MapHealthChecks("/health");

    //=========================================================================
    // CONFIGURACIONES ADICIONALES PARA PRODUCCIÓN
    //=========================================================================
    if (app.Environment.IsProduction())
    {
        // Configuraciones específicas de producción
        app.UseHsts(); // HTTP Strict Transport Security
        
        // Custom error handling middleware
        // app.UseExceptionHandler("/Error");
    }

    //=========================================================================
    // INICIALIZACIÓN DE BASE DE DATOS (OPCIONAL)  
    //=========================================================================
    // Ejecutar migraciones automáticamente al iniciar (solo si es necesario)
    // using (var scope = app.Services.CreateScope())
    // {
    //     var context = scope.ServiceProvider.GetRequiredService<YourDbContext>();
    //     context.Database.Migrate();
    // }

    //=========================================================================
    // EJECUCIÓN DE LA APLICACIÓN
    //=========================================================================
    Log.Information("Aplicación iniciada correctamente en {Environment}", app.Environment.EnvironmentName);
    
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar");
}
finally
{
    Log.CloseAndFlush();
}




// Manejador de autenticación de prueba para desarrollo
public class TestAuthenticationHandler : Microsoft.AspNetCore.Authentication.AuthenticationHandler<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions> options,
        Microsoft.Extensions.Logging.ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder,
        Microsoft.AspNetCore.Authentication.ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<Microsoft.AspNetCore.Authentication.AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "TestUser") };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "Test");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        var ticket = new Microsoft.AspNetCore.Authentication.AuthenticationTicket(principal, "Test");
        return Task.FromResult(Microsoft.AspNetCore.Authentication.AuthenticateResult.Success(ticket));
    }
}