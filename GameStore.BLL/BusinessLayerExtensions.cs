using System.Net;
using GameStore.BLL.GamesQueryPipeline;
using GameStore.BLL.GamesQueryPipeline.Interfaces;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.CommentServices;
using GameStore.BLL.Interfaces.EntityServices;
using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Interfaces.GameServices;
using GameStore.BLL.Interfaces.GameServices.ImageServices;
using GameStore.BLL.Interfaces.GameSetupStrategies;
using GameStore.BLL.Interfaces.NotificationServices;
using GameStore.BLL.Interfaces.OrderServices;
using GameStore.BLL.Interfaces.PaymentServices;
using GameStore.BLL.Interfaces.Security;
using GameStore.BLL.Options;
using GameStore.BLL.Services.CommentServices;
using GameStore.BLL.Services.CommentServices.CommentSetupStrategies;
using GameStore.BLL.Services.EntityServices;
using GameStore.BLL.Services.ExternalServices;
using GameStore.BLL.Services.GameServices;
using GameStore.BLL.Services.GameServices.GameSetupStrategies;
using GameStore.BLL.Services.GameServices.ImageServices;
using GameStore.BLL.Services.InvoiceCreator;
using GameStore.BLL.Services.NotificationServices;
using GameStore.BLL.Services.NotificationServices.NotificationStrategies;
using GameStore.BLL.Services.OrderServices;
using GameStore.BLL.Services.PaymentServices;
using GameStore.BLL.Services.PaymentServices.PaymentHandlers;
using GameStore.BLL.Services.Security;
using GameStore.BLL.Services.Security.AuthentificationStrategies;
using GameStore.BLL.Services.Security.PageAccessRules;
using GameStore.BLL.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace GameStore.BLL;

public static class BusinessLayerExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddGameSetupStrategies()
            .AddCoreBusinessServices()
            .AddCommentServices()
            .ConfigureOptions()
            .AddOrderServices()
            .AddPaymentServices()
            .AddExternalServices()
            .AddUtils()
            .AddGameFilteringPipeline()
            .AddUserAndRoleServices()
            .AddAccessRules()
            .AddImageServices()
            .AddNotificationServices();

        return services;
    }

    private static void AddNotificationServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationStrategy, PushNotificationStrategy>();
        services.AddScoped<INotificationStrategy, EmailNotificationStrategy>();
        services.AddScoped<INotificationStrategy, SmsNotificationStrategy>();
        services.AddScoped<INotificationUserService, NotificationUserService>();
    }

    private static IServiceCollection AddImageServices(this IServiceCollection services)
    {
        services.AddScoped<IGameImageService, GameImageService>();
        services.AddScoped<IImageDataParser, GameImageDataParser>();
        services.AddScoped<IImageMimeMapper, ImageMimeMapper>();

        return services;
    }

    private static IServiceCollection AddAccessRules(this IServiceCollection services)
    {
        services.AddScoped<IPageAccessRule, GameAccessRule>();
        services.AddScoped<IPageAccessRule, CommonEntitiesAccessRule>();
        services.AddScoped<IPageAccessRule, UserPanelAccessRule>();
        services.AddScoped<IPageAccessRule, HistoryAccessRule>();
        services.AddScoped<IPageAccessRule, OrdersAccessRule>();
        services.AddScoped<IPageAccessRule, OrderEditAccessRule>();
        services.AddScoped<IPageAccessRule, CommentAccessRule>();
        services.AddScoped<IPageAccessRule, CommentsModerationAccessRule>();
        services.AddScoped<IPageAccessRule, RoleAccessRule>();
        services.AddScoped<IPageAccessRule, CommonEntitiesManagementAccessRule>();
        services.AddScoped<IPageAccessRule, GameManagementAccessRule>();
        
        services.AddScoped<IPageAccessChecker, PageAccessChecker>();
        return services;
    }

    private static IServiceCollection AddUserAndRoleServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        services.AddScoped<IAuthentificationStrategy, ExternalAuthentificationStrategy>();
        services.AddScoped<IAuthentificationStrategy, InternalAuthentificationStrategy>();
        
        services.AddScoped<ICurrentUserPermissionService, CurrentUserPermissionService>();
        return services;
    }
    
    private static IServiceCollection AddGameFilteringPipeline(this IServiceCollection services)
    {
        return services.AddScoped<IGamePipelineFactory, GamePipelineFactory>();
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        services.AddHttpClient<IPaymentExternalService, PaymentExternalService>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<PaymentExternalServiceOptions>>();
                client.BaseAddress = new Uri(options.Value.BaseUrl);
            })
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptions<PaymentExternalServiceOptions>>();
                var logger = sp.GetRequiredService<ILogger<PaymentExternalService>>();
                return CreateRetryPolicy(options.Value.MaxAttempts, options.Value.DelayInSeconds, logger);
            });

        services.AddHttpClient<IAuthentificationExternalService, AuthentificationExternalService>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<AuthentificationExternalServiceOptions>>();
                client.BaseAddress = new Uri(options.Value.BaseUrl);
            })
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptions<AuthentificationExternalServiceOptions>>();
                var logger = sp.GetRequiredService<ILogger<AuthentificationExternalServiceOptions>>();
                return CreateRetryPolicy(options.Value.MaxAttempts, options.Value.DelayInSeconds, logger);
            });
        
        return services;
    }

    private static IServiceCollection AddCommentServices(this IServiceCollection services)
    {
        services.AddScoped<ICommentActionService, CommentActionService>();
        services.AddScoped<ICommentActionStrategy, QuoteActionStrategy>();
        services.AddScoped<ICommentActionStrategy, ReplyActionStrategy>();

        return services;
    }

    private static IServiceCollection AddPaymentServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentHandlerFactory, PaymentHandlerFactory>();
        services.AddScoped<IPaymentService, PaymentService>()
            .AddScoped<IPaymentHandler, BoxPaymentHandler>()
            .AddScoped<IPaymentHandler, VisaPaymentHandler>()
            .AddScoped<IPaymentHandler, BankPaymentHandler>();
        services.AddScoped<IPdfCreator, PdfCreator>();
        return services;
    }

    private static IServiceCollection ConfigureOptions(this IServiceCollection services)
    {
        services.AddOptions<FilePathOptions>()
            .Configure<IConfiguration>((settings, config) =>
                config.GetSection("GameFiles").Bind(settings));

        services.AddOptions<PaymentExternalServiceOptions>()
            .Configure<IConfiguration>((settings, config) =>
                config.GetSection("PaymentExternalService").Bind(settings));

        services.AddOptions<BoxPaymentOptions>()
            .Configure<IConfiguration>((settings, config) =>
                config.GetSection("IBoxPaymentMethod").Bind(settings));

        services.AddOptions<VisaPaymentOptions>()
            .Configure<IConfiguration>((settings, config) =>
                config.GetSection("Visa").Bind(settings));

        services.AddOptions<BankOptions>()
            .Configure<IConfiguration>((settings, config) =>
                config.GetSection("Bank").Bind(settings));

        services.AddOptions<JwtSettings>()
            .Configure<IConfiguration>((settings, config) =>
                config.GetSection("JwtSettings").Bind(settings));

        services.AddOptions<AuthentificationExternalServiceOptions>()
            .Configure<IConfiguration>((settings, config) =>
                config.GetSection("AuthentificationService").Bind(settings));
        
        return services;
    }

    private static IServiceCollection AddCoreBusinessServices(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
            .AddScoped<ICommentService, CommentService>()
            .AddScoped<IGameService, GameService>()
            .AddScoped<IGenreService, GenreService>()
            .AddScoped<IPlatformService, PlatformService>()
            .AddScoped<IFilePathService, FilePathService>()
            .AddScoped<IGameFileService, GameFileService>()
            .AddScoped<IPublisherService, PublisherService>()
            .AddScoped<IGameSetupService, GameSetupService>()
            .AddScoped<IUserBanService, UserBanService>()
            .AddScoped<IShippersService, ShippersService>();
        

        return services;
    }

    private static IServiceCollection AddGameSetupStrategies(this IServiceCollection services)
    {
        services.AddScoped<IGameSetupStep, GameGenreSetupStep>()
            .AddScoped<IGameSetupStep, GameIdSetupStep>()
            .AddScoped<IGameSetupStep, GameKeySetupStep>()
            .AddScoped<IGameSetupStep, GamePlatformSetupStep>()
            .AddScoped<IGameSetupStep, GamePublisherSetupStep>()
            .AddScoped<IGameSetupStep, CreationDateSetupStep>()
            .AddScoped<IGameSetupStep, GameImageSetupStep>();
        return services;
    }

    private static IServiceCollection AddOrderServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderServices, OrderServices>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderManager, OrderManager>();
        services.AddScoped<ICartManager, CartManager>();
        services.AddScoped<IPaymentMethodsService, PaymentMethodsService>();
        return services;
    }

    private static IServiceCollection AddUtils(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IGuidProvider, GuidProvider>();
        return services;
    }
    
    private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(int maxAttempts, int delayInSeconds, ILogger logger)
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => 
                r.StatusCode is not HttpStatusCode.OK)
            .WaitAndRetryAsync(
                retryCount: maxAttempts,
                sleepDurationProvider: _ =>
                    TimeSpan.FromSeconds(delayInSeconds), 
                onRetry: (response, delay, retryAttempt, _) =>
                {
                    logger.LogWarning($"Retry {retryAttempt} due to {response?.Result?.StatusCode ?? 0}. Waiting {delay.TotalSeconds} seconds.");
                });
    }
}