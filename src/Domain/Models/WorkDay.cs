namespace AppointmentManager.Domain.Models
{
    public class WorkDay
    {
        public DateOnly Day { get; set; }
        public WorkPeriod WorkPeriod { get; set; } = new();

        public List<BusySlot> BusySlots { get; set; } = [];
    }
}
