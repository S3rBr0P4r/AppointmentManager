using System.Net;
using AppointmentManager.Domain.Entities;

namespace AppointmentManager.Domain.Services
{
    public interface ISlotsManagementService
    {
        public Task<IEnumerable<Slot>> GetAvailableSlotsAsync(DateOnly date, CancellationToken cancellationToken);

        public Task<HttpStatusCode> TakeSlotAsync(Appointment appointment, CancellationToken cancellationToken);
    }
}
