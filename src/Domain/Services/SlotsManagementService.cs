using System.Linq.Expressions;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Models;
using Newtonsoft.Json;

namespace AppointmentManager.Domain.Services
{
    public class SlotsManagementService : ISlotsManagementService
    {
        private readonly HttpClient _httpClient;

        public SlotsManagementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Slot>> GetAvailableSlotsAsync(DateOnly date)
        {
            var slots = new List<Slot>();
            var dateRequestParameter = $"{date.Year}{date.Month}{date.Day}";
            using HttpResponseMessage response = await _httpClient.GetAsync(dateRequestParameter);
            dynamic? responseContent = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            var doctorInformation = new DoctorInformation
            {
                Facility = new Facility
                {
                    FacilityId = responseContent.Facility.FacilityId,
                    Name = responseContent.Facility.Name,
                    Address = responseContent.Facility.Address
                },
                SlotDurationMinutes = responseContent.SlotDurationMinutes,
                WorkDays = []
            };

            if (responseContent.Monday is not null)
            {
                var workDay = new WorkDay
                {
                    Name = nameof(responseContent.Monday),
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = responseContent.Monday.WorkPeriod.StartHour,
                        EndHour = responseContent.Monday.WorkPeriod.EndHour,
                        LunchStartHour = responseContent.Monday.WorkPeriod.LunchStartHour,
                        LunchEndHour = responseContent.Monday.WorkPeriod.LunchEndHour
                    }
                };
                if (responseContent.Monday.BusySlots is not null)
                {
                    workDay.BusySlots = responseContent.Monday.BusySlots.ToObject<List<BusySlot>>();
                }

                doctorInformation.WorkDays.Add(workDay);
            }

            foreach (var workDay in doctorInformation.WorkDays)
            {
                if (workDay.Name.Equals("Monday"))
                {
                    var startWorkPeriod = new DateTime(date.Year, date.Month, date.Day, workDay.WorkPeriod.StartHour, 0, 0);
                    var endWorkPeriod = new DateTime(date.Year, date.Month, date.Day, workDay.WorkPeriod.EndHour, 0, 0);
                    var startLunchPeriod = new TimeOnly(workDay.WorkPeriod.LunchStartHour, 0, 0);
                    var endLunchPeriod = new TimeOnly(workDay.WorkPeriod.LunchEndHour, 0, 0);
                    while (endWorkPeriod > startWorkPeriod)
                    {
                        var slotDay = new DateOnly(startWorkPeriod.Year, startWorkPeriod.Month, startWorkPeriod.Day);
                        var slotStartTime = new TimeOnly(startWorkPeriod.Hour, startWorkPeriod.Minute, startWorkPeriod.Second);
                        var slot = new Slot
                        {
                            StartDate = slotDay,
                            StartTime = slotStartTime,
                            EndDate = slotDay,
                            EndTime = slotStartTime.AddMinutes(doctorInformation.SlotDurationMinutes)
                        };
                        if (!(slot.StartTime >= startLunchPeriod && slot.EndTime <= endLunchPeriod) && !workDay.BusySlots.Exists(busySlot =>
                            {
                                var busySlotTime = TimeOnly.FromDateTime(busySlot.Start);
                                return busySlotTime.Equals(slot.StartTime);
                            }))
                        {
                            slots.Add(slot);
                        }
                        startWorkPeriod = startWorkPeriod.AddMinutes(doctorInformation.SlotDurationMinutes);
                    }
                }
                
            }

            return slots;
        }
    }
}
