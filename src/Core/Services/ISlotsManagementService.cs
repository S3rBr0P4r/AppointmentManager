using AppointmentManager.Core.Entities;

namespace AppointmentManager.Core.Services
{
    public interface ISlotsManagementService
    {
        public IEnumerable<Slot> GetAvailableSlots(string date);
    }
}
