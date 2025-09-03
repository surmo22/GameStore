using Serilog;
using Serilog.Sinks.Syslog;

namespace GameStore.WebApi.Configurations;

public static class WebApplicationBuilderSerilogExtensions
{
    public static void SetupSerilog(this WebApplicationBuilder builder)
    {
        builder.Host
            .UseSerilog(
                (ctx, lc) =>
                {
                    lc.ReadFrom.Configuration(ctx.Configuration)
                        .WriteTo.UdpSyslog(
                            host: "127.0.0.1",
                            port: 514,
                            facility: Facility.Local0,
                            format: SyslogFormat.RFC5424 
                        );

                });
    }
}
