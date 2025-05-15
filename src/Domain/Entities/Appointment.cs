namespace AppointmentManager.Domain.Entities
{
    public class Appointment(string facilityId, string comments, Patient patient)
    {
        public string FacilityId { get; set; } = facilityId;
        public DateTime Start { get; set; }

        public DateTime End { get; set; }
        public string Comments { get; set; } = comments;
        public Patient Patient { get; set; } = patient;
    }
}
