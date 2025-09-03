using System.Reflection;
using System.Text;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Options;
using GameStore.Common.Constants;
using GameStore.Data.Data;
using GameStore.Domain.Entities.Enums;
using GameStore.Domain.Entities.UserEntities;
using GameStore.WebApi.Authorization.Handlers;
using GameStore.WebApi.Authorization.Requierments;
using GameStore.WebApi.Filters;
using GameStore.WebApi.Filters.Handlers;
using GameStore.WebApi.Middlewares;
using GameStore.WebApi.Middlewares.Loggers;
using GameStore.WebApi.Services;
using GameStore.WebApi.Services.Converters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IO;

namespace GameStore.WebApi.Configurations;

public static class WebApiConfigurations
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddBasicApiServices()
            .AddExceptionHandlers()
            .AddPaymentResultServices()
            .AddIdentity()
            .AddAuthentication(configuration)
            .AddAuthorization();
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddMemoryCache();
        return services.AddAuthorization(options =>
        {
            foreach (UserPermissionTypes permission in Enum.GetValues(typeof(UserPermissionTypes)))
            {
                options.AddPolicy(permission.ToString(), policy =>
                    policy.Requirements.Add(new PermissionRequirement(permission.ToString())));
            }
            
            options.AddPolicy(AuthorizationPolicies.NotBanned, policy =>
            {
                policy.AddRequirements(new NotBannedRequirement());
            });
        });
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
    {
        return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            }).Services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 0;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.AllowedUserNameCharacters = null;
            })
            .AddEntityFrameworkStores<GameStoreContext>();
        
        return services;
    }

    public static WebApplication UseApiConfigurations(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameStore V1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseCors();
        app.UseMiddleware<GamesCountMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<PerformanceLoggingMiddleware>();
        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();


        return app;
    }

    private static IServiceCollection AddBasicApiServices(this IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers(options =>
                options.Filters.Add<ExceptionFilter>())
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new FlexibleEnumJsonConverter<UserPermissionTypes>());
                options.JsonSerializerOptions.Converters.Add(new FlexibleEnumJsonConverter<BanDuration>());
                options.JsonSerializerOptions.Converters.Add(new FlexibleEnumJsonConverter<PublishingDateFilter>());
                options.JsonSerializerOptions.Converters.Add(new FlexibleEnumJsonConverter<SortingOptions>());
                options.JsonSerializerOptions.Converters.Add(new FlexibleEnumJsonConverter<UserNotificationTypes>());
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            var xmlFile = $"{assemblyName.Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        services.AddScoped<IAuthorizationHandler, NotBannedHandler>();
        services.AddHttpContextAccessor();

        return services;
    }

    private static void UseCors(this WebApplication app)
    {
        app.UseCors(policy =>
        {
            policy.WithOrigins("http://localhost:57055", "http://localhost:8080", "http://192.168.0.153:8080", "http://localhost:4200", "https://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("x-total-numbers-of-games");
        });
    }

    private static IServiceCollection AddExceptionHandlers(this IServiceCollection service)
    {
        service.AddSingleton<IExceptionHandler, KeyNotFoundExceptionHandler>();
        service.AddSingleton<IExceptionHandler, InvalidGenreHierarchyExceptionHandler>();
        service.AddSingleton<IExceptionHandler, DbConcurrencyExceptionHandler>();
        service.AddSingleton<IExceptionHandler, IoExceptionHandler>();
        service.AddSingleton<IExceptionHandler, TaskCanceledExceptionHandler>();
        service.AddSingleton<IExceptionHandler, PaymentMethodIsNotSupportedExceptionHandler>();
        service.AddSingleton<IExceptionHandler, PaymentFailedExceptionHandler>();
        service.AddSingleton<IExceptionHandler, UnauthorizedAccessExceptionHandler>();
        service.AddScoped<ExceptionFilter>();

        service.AddSingleton<IResponseLogger, ResponseLogger>();
        service.AddSingleton<IRequestLogger, RequestLogger>();
        service.AddSingleton<RecyclableMemoryStreamManager>();

        return service;
    }
}