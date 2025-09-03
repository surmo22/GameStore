using GameStore.BLL.Interfaces.ExternalServices;
using GameStore.BLL.Options;
using GameStore.BLL.Services.ExternalServices;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.Configure<SenderOptions>(
    builder.Configuration.GetSection("Sender"));

builder.Services.AddSingleton<ISendGridClient, SendGridClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var apiKey = config["SendGridApiKey"];
    return new SendGridClient(apiKey);
});

builder.Services.AddSingleton<IEmailSender, SendGridEmailSender>();

builder.Build().Run();
