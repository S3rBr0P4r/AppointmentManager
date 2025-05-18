using AppointmentManager.Presentation.Handlers;
using Microsoft.AspNetCore.Authentication;

namespace AppointmentManager.Presentation.Configuration
{
    public static class Authorization
    {
        public static IServiceCollection AddBasicAuthorization(this IServiceCollection services)
        {
            services
                .AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            return services;
        }
    }
}
