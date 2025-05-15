using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Models;

namespace AppointmentManager.Domain.Services
{
    public class SlotsManagementService : ISlotsManagementService
    {
        private readonly IDoctorShiftService _doctorShiftService;

        public SlotsManagementService(IDoctorShiftService doctorShiftService)
        {
            _doctorShiftService = doctorShiftService;
        }

        public async Task<IEnumerable<Slot>> GetAvailableSlotsAsync(DateOnly date)
        {
            var availableSlots = new List<Slot>();
            var slotsInformation = await _doctorShiftService.GetSlotsInformationAsync(date);

            foreach (var workDay in slotsInformation.WorkDays)
            {
                var startWorkPeriod = new DateTime(workDay.Day.Year, workDay.Day.Month, workDay.Day.Day, workDay.WorkPeriod.StartHour, 0, 0);
                var endWorkPeriod = new DateTime(workDay.Day.Year, workDay.Day.Month, workDay.Day.Day, workDay.WorkPeriod.EndHour, 0, 0);
                var startLunchPeriod = new DateTime(workDay.Day.Year, workDay.Day.Month, workDay.Day.Day, workDay.WorkPeriod.LunchStartHour, 0, 0);
                var endLunchPeriod = new DateTime(workDay.Day.Year, workDay.Day.Month, workDay.Day.Day, workDay.WorkPeriod.LunchEndHour, 0, 0);

                while (endLunchPeriod > startWorkPeriod)
                {
                    var slot = new Slot
                    {
                        Start = startWorkPeriod,
                        End = startWorkPeriod.AddMinutes(slotsInformation.SlotDurationMinutes)
                    };

                    if (SlotIsAlreadyBooked(workDay.BusySlots, slot))
                    {
                        startWorkPeriod = startWorkPeriod.AddMinutes(slotsInformation.SlotDurationMinutes);
                        continue;
                    }

                    if (SlotStartIsInsideWorkPeriod(slot, startWorkPeriod, endWorkPeriod, startLunchPeriod, endLunchPeriod))
                    {
                        availableSlots.Add(slot);
                    }

                    startWorkPeriod = startWorkPeriod.AddMinutes(slotsInformation.SlotDurationMinutes);
                }
            }

            return availableSlots;
        }

        private static bool SlotIsAlreadyBooked(List<BusySlot> busySlots, Slot slot)
        {
            return busySlots.Exists(busySlot =>
            {
                var busySlotTime = TimeOnly.FromDateTime(busySlot.Start);
                return busySlotTime.Equals(TimeOnly.FromDateTime(slot.Start));
            });
        }

        private static bool SlotStartIsInsideWorkPeriod(
            Slot slot, 
            DateTime startWorkPeriod, DateTime endWorkPeriod,
            DateTime startLunchPeriod, DateTime endLunchPeriod)
        {
            return (TimeOnly.FromDateTime(slot.Start) >= TimeOnly.FromDateTime(startWorkPeriod) &&
                    TimeOnly.FromDateTime(slot.Start) < TimeOnly.FromDateTime(endWorkPeriod)) ||
                   (TimeOnly.FromDateTime(slot.Start) >= TimeOnly.FromDateTime(startLunchPeriod) &&
                    TimeOnly.FromDateTime(slot.Start) < TimeOnly.FromDateTime(endLunchPeriod));
        }
    }
}
