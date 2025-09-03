using GameStore.WebApi.PaymentResultHandler;
using GameStore.WebApi.PaymentResultHandler.Strategies;

namespace GameStore.WebApi.Configurations;

public static class PaymentResultHandlerConfiguration
{
    public static IServiceCollection AddPaymentResultServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IPaymentResultStrategy, BoxResultStrategy>()
            .AddScoped<IPaymentResultStrategy, VisaResultStrategy>()
            .AddScoped<IPaymentResultStrategy, InvoiceResultStrategy>()
            .AddScoped<IPaymentResultHandler, PaymentResultHandler.PaymentResultHandler>();
    }
}