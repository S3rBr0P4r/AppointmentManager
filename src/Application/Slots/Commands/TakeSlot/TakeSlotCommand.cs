using AppointmentManager.Domain.Entities;

namespace AppointmentManager.Application.Slots.Commands.TakeSlot
{
    public class TakeSlotCommand
    {
        public string FacilityId { get; set; }
        public DateTime Start { get; set; }

        public DateTime End { get; set; }
        public string Comments { get; set; }
        public Patient Patient { get; set; }
    }
}
