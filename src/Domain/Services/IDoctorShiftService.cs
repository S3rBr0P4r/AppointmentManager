using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Models;

namespace AppointmentManager.Domain.Services
{
    public interface IDoctorShiftService
    {
        public Task<SlotsInformation> GetSlotsInformationAsync(DateOnly date, CancellationToken cancellationToken);

        public Task<HttpResponseMessage> AddSlotToShiftAsync(Appointment appointment, CancellationToken cancellationToken);
    }
}
