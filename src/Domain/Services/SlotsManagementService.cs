using System.Net;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AppointmentManager.Domain.Services
{
    public class SlotsManagementService : ISlotsManagementService
    {
        private readonly ILogger<SlotsManagementService> _logger;
        private readonly IDoctorShiftService _doctorShiftService;

        public SlotsManagementService(ILogger<SlotsManagementService> logger, IDoctorShiftService doctorShiftService)
        {
            _logger = logger;
            _doctorShiftService = doctorShiftService;
        }

        public async Task<IEnumerable<Slot>> GetAvailableSlotsAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var availableSlots = new List<Slot>();
            var slotsInformation = await _doctorShiftService.GetSlotsInformationAsync(date, cancellationToken);

            foreach (var workDay in slotsInformation.WorkDays)
            {
                var startWorkPeriod = new DateTime(workDay.Day.Year, workDay.Day.Month, workDay.Day.Day, workDay.WorkPeriod.StartHour, 0, 0);
                var startLunchPeriod = new DateTime(workDay.Day.Year, workDay.Day.Month, workDay.Day.Day, workDay.WorkPeriod.LunchStartHour, 0, 0);
                var endLunchPeriod = new DateTime(workDay.Day.Year, workDay.Day.Month, workDay.Day.Day, workDay.WorkPeriod.LunchEndHour, 0, 0);
                var endWorkPeriod = new DateTime(workDay.Day.Year, workDay.Day.Month, workDay.Day.Day, workDay.WorkPeriod.EndHour, 0, 0);
                
                while (endWorkPeriod > startWorkPeriod)
                {
                    var slot = new Slot
                    {
                        Start = startWorkPeriod,
                        End = startWorkPeriod.AddMinutes(slotsInformation.SlotDurationMinutes)
                    };

                    if (SlotIsAlreadyBooked(workDay.BusySlots, slot))
                    {
                        _logger.LogInformation("Slot from {SlotStart} till {SlotEnd} already booked", slot.Start, slot.End);
                        startWorkPeriod = startWorkPeriod.AddMinutes(slotsInformation.SlotDurationMinutes);
                        continue;
                    }

                    if (SlotStartIsInsideWorkPeriod(slot, startWorkPeriod, startLunchPeriod, endLunchPeriod, endWorkPeriod))
                    {
                        availableSlots.Add(slot);
                    }

                    startWorkPeriod = startWorkPeriod.AddMinutes(slotsInformation.SlotDurationMinutes);
                }
            }

            _logger.LogInformation(
                "{AvailableSlotsCount} available slot(s) found with a duration of {SlotsInformationSlotDurationMinutes} minutes each of them",
                availableSlots.Count, slotsInformation.SlotDurationMinutes);
            return availableSlots;
        }

        public async Task<HttpStatusCode> TakeSlotAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            var response = await _doctorShiftService.AddSlotToShiftAsync(appointment, cancellationToken);
            if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
            {
                throw new ArgumentException(response.Content.ReadAsStringAsync(cancellationToken).Result);
            }

            _logger.LogInformation(
                "Slot from {AppointmentStart} till {AppointmentEnd} booked to patient {PatientName} {PatientSecondName}",
                appointment.Start, appointment.End, appointment.Patient.Name, appointment.Patient.SecondName);
            return response.StatusCode;
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
            DateTime startWorkPeriod,
            DateTime startLunchPeriod,
            DateTime endLunchPeriod,
            DateTime endWorkPeriod)
        {
            return (TimeOnly.FromDateTime(slot.Start) >= TimeOnly.FromDateTime(startWorkPeriod) &&
                    TimeOnly.FromDateTime(slot.Start) < TimeOnly.FromDateTime(startLunchPeriod)) ||
                   (TimeOnly.FromDateTime(slot.Start) >= TimeOnly.FromDateTime(endLunchPeriod) &&
                    TimeOnly.FromDateTime(slot.Start) < TimeOnly.FromDateTime(endWorkPeriod));
        }
    }
}
