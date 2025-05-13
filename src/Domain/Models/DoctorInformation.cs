namespace AppointmentManager.Domain.Models
{
    public class DoctorInformation
    {
        public Facility Facility { get; set; }

        public int SlotDurationMinutes { get; set; }

        public List<WorkDay> WorkDays { get; set; }
    }
}
