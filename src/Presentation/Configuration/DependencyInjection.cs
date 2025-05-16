using System.Net;
using AppointmentManager.Application.Slots.Commands.TakeSlot;
using AppointmentManager.Application.Slots.Queries.GetAvailableSlots;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Formatters;
using AppointmentManager.Domain.Handlers;
using AppointmentManager.Domain.Services;
using AppointmentManager.Infrastructure.Dispatchers;
using AppointmentManager.Infrastructure.Handlers;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppointmentManager.Presentation.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainDependencies(this IServiceCollection services)
        {
            services.AddScoped<IDoctorShiftService, DoctorShiftService>();
            services.AddScoped<IDateRequestFormatter, DateRequestFormatter>();
            services.AddScoped<ISlotsManagementService, SlotsManagementService>();
            return services;
        }

        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            return services;
        }

        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddScoped<IQueryHandler<GetAvailableSlotsQuery, IEnumerable<Slot>>, GetAvailableSlotsQueryHandler>();
            services.AddScoped<ICommandHandler<TakeSlotCommand, HttpStatusCode>, TakeSlotCommandHandler>();
            return services;
        }

        public static IServiceCollection AddPresentationDependencies(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();
            return services;
        }
    }
}
