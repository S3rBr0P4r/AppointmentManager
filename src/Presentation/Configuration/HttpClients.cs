using System.Text;
using AppointmentManager.Domain.Services;

namespace AppointmentManager.Presentation.Configuration
{
    public static class HttpClients
    {
        public static IServiceCollection ConfigureHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<IDoctorShiftService, DoctorShiftService>(client =>
            {
                var slotServiceUri = Environment.GetEnvironmentVariable("SLOT_SERVICE_URI");
                var slotServiceUserName = Environment.GetEnvironmentVariable("SLOT_SERVICE_USERNAME");
                var slotServicePassword = Environment.GetEnvironmentVariable("SLOT_SERVICE_PASSWORD");
                client.BaseAddress = new Uri(slotServiceUri);
                client.DefaultRequestHeaders.Authorization
                    = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"{slotServiceUserName}:{slotServicePassword}")));
            });
            return services;
        }
    }
}
