using AppointmentManager.Core.Entities;

namespace AppointmentManager.Application.Services
{
    public interface ISlotsManagementService
    {
        public Task<IEnumerable<Slot>> GetAvailableSlotsAsync(DateOnly date);
    }
}
