namespace AppointmentManager.Domain.Models
{
    public class SlotsInformation
    {
        public double SlotDurationMinutes { get; set; }

        public IEnumerable<WorkDay> WorkDays { get; set; }
    }
}
