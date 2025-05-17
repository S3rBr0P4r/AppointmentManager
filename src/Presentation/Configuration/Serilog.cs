using Serilog;

namespace AppointmentManager.Presentation.Configuration
{
    public static class Serilog
    {
        public static WebApplication? EnableSerilogRequestLogging(this WebApplication? app)
        {
            app?.UseSerilogRequestLogging(options =>
            {
                options.IncludeQueryInRequestPath = true;
            });
            return app;
        }

        public static ConfigureHostBuilder EnableSerilog(this ConfigureHostBuilder host)
        {
            host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));
            return host;
        }
    }
}
