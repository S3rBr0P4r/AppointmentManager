using AppointmentManager.Domain.Formatters;
using AppointmentManager.Domain.Models;
using Newtonsoft.Json;

namespace AppointmentManager.Domain.Services
{
    public class DoctorShiftService : IDoctorShiftService
    {
        private readonly IDateRequestFormatter _dateRequestFormatter;
        private readonly HttpClient _httpClient;

        public DoctorShiftService(IDateRequestFormatter dateRequestFormatter, HttpClient httpClient)
        {
            _dateRequestFormatter = dateRequestFormatter;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<WorkDay>> GetWorkDaysShiftAsync(DateOnly date)
        {
            var workDays = new List<WorkDay>();
            var dateRequestParameter = _dateRequestFormatter.GetCompatibleDateWithSlotService(date);
            using HttpResponseMessage response = await _httpClient.GetAsync(dateRequestParameter);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseContent))
            {
                // Todo throw exception
            }
            
            dynamic? jsonContent = JsonConvert.DeserializeObject(responseContent);
            if (jsonContent is null)
            {
                // Todo throw exception
            }
            var monday = jsonContent?.Monday;
            if (monday is not null)
            {
                var workDay = new WorkDay
                {
                    Day = date,
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = monday.WorkPeriod.StartHour,
                        EndHour = monday.WorkPeriod.EndHour,
                        LunchStartHour = monday.WorkPeriod.LunchStartHour,
                        LunchEndHour = monday.WorkPeriod.LunchEndHour
                    }
                };
                if (monday.BusySlots is not null)
                {
                    workDay.BusySlots = monday.BusySlots.ToObject<List<BusySlot>>();
                }

                workDays.Add(workDay);
            }

            return workDays;
        }
    }
}
