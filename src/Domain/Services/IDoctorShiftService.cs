using AppointmentManager.Domain.Models;

namespace AppointmentManager.Domain.Services
{
    public interface IDoctorShiftService
    {
        public Task<SlotsInformation> GetSlotsInformationAsync(DateOnly date);
    }
}
