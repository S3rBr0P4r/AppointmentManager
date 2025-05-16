using AppointmentManager.Presentation.ExceptionHandlers;

namespace AppointmentManager.Presentation.Configuration
{
    public static class ExceptionHandlers
    {
        public static IServiceCollection AddExceptionsHandlers(this IServiceCollection services)
        {
            services.AddExceptionHandler<BadRequestExceptionHandler>();
            services.AddExceptionHandler<InternalServerErrorExceptionHandler>();
            return services;
        }
    }
}
