using Asp.Versioning;

namespace AppointmentManager.Presentation.Configuration
{
    public static class Versioning
    {
        public static IServiceCollection ConfigureVersioning(this IServiceCollection services)
        {
            services
                .AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
            return services;
        }
    }
}
