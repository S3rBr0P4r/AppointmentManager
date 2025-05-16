using System.Reflection;
using Swashbuckle.AspNetCore.Filters;

namespace AppointmentManager.Presentation.Configuration
{
    public static class Swagger
    {
        public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);
                s.ExampleFilters();
            });
            services.AddSwaggerExamplesFromAssemblyOf<Program>();
            return services;
        }

        public static WebApplication? EnableSwagger(this WebApplication? app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var descriptions = app?.DescribeApiVersions();
                foreach (var description in descriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
            return app;
        }
    }
}
