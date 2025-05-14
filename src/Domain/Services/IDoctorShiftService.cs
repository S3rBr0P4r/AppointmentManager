using AppointmentManager.Domain.Models;

namespace AppointmentManager.Domain.Services
{
    public interface IDoctorShiftService
    {
        public Task<IEnumerable<WorkDay>> GetWorkDaysShiftAsync(DateOnly date);
    }
}
