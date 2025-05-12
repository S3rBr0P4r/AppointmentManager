using AppointmentManager.Application.Services;
using AppointmentManager.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentManager.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        {
            services.AddScoped<ISlotsManagementService, SlotsManagementService>();
            return services;
        }
    }
}
