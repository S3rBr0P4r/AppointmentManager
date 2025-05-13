using AppointmentManager.Application.Slots.Queries.GetAvailableSlots;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Services;
using AppointmentManager.Infrastructure.Dispatchers;
using AppointmentManager.Infrastructure.Handlers;

namespace AppointmentManager.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        {
            services.AddScoped<ISlotsManagementService, SlotsManagementService>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
            services.AddScoped<IQueryHandler<GetAvailableSlotsQuery, IEnumerable<Slot>>, GetAvailableSlotsQueryHandler>();
            return services;
        }
    }
}
