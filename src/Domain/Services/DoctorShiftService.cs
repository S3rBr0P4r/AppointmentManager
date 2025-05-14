using System.Net;
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

        public async Task<SlotsInformation> GetSlotsInformationAsync(DateOnly date)
        {
            var slotsInformation = new SlotsInformation();
            var dateRequestParameter = _dateRequestFormatter.GetCompatibleDateWithSlotService(date);
            using var response = await _httpClient.GetAsync(dateRequestParameter);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (DateRequestedToSlotServiceIsNotMonday(response, responseContent)) throw new ArgumentException($"DateTime '{date}' must be Monday");
            if (SlotServiceIsDown(response)) throw new TimeoutException("Slot service is down and work days shift cannot be retrieved");
            
            dynamic jsonContent = JsonConvert.DeserializeObject(responseContent);
            slotsInformation.WorkDays = GetWorkDaysInformation(date, jsonContent);
            slotsInformation.SlotDurationMinutes = GetSlotDurationMinutes(jsonContent);

            return slotsInformation;
        }

        private static int? GetSlotDurationMinutes(dynamic? jsonContent)
        {
            return JsonPropertyIsAvailable(jsonContent?.SlotDurationMinutes) ? (int?)jsonContent?.SlotDurationMinutes : null;
        }

        private static List<WorkDay> GetWorkDaysInformation(DateOnly date, dynamic? jsonContent)
        {
            var workDays = new List<WorkDay>();
            var daysJsonProperty = new Dictionary<DateOnly, dynamic?>
            {
                { date, jsonContent?.Monday },
                { date.AddDays(1), jsonContent?.Tuesday },
                { date.AddDays(2), jsonContent?.Wednesday },
                { date.AddDays(3), jsonContent?.Thursday },
                { date.AddDays(4), jsonContent?.Friday }
            };

            foreach (var propertyContent in daysJsonProperty)
            {
                if (!JsonPropertyIsAvailable(propertyContent.Value)) continue;
                var workDay = GetWorkDay(propertyContent.Key, propertyContent.Value);
                workDays.Add(workDay);
            }
            return workDays;
        }

        private static bool SlotServiceIsDown(HttpResponseMessage response)
        {
            return response.StatusCode.Equals(HttpStatusCode.InternalServerError);
        }

        private static bool DateRequestedToSlotServiceIsNotMonday(HttpResponseMessage response, string responseContent)
        {
            return response.StatusCode.Equals(HttpStatusCode.BadRequest) && responseContent.Contains("datetime must be a Monday");
        }

        private static WorkDay GetWorkDay(DateOnly date, dynamic day)
        {
            var workDay = new WorkDay
            {
                Day = date,
                WorkPeriod = new WorkPeriod
                {
                    StartHour = day.WorkPeriod.StartHour,
                    EndHour = day.WorkPeriod.EndHour,
                    LunchStartHour = day.WorkPeriod.LunchStartHour,
                    LunchEndHour = day.WorkPeriod.LunchEndHour
                }
            };
            if (day.BusySlots is not null)
            {
                workDay.BusySlots = day.BusySlots.ToObject<List<BusySlot>>();
            }

            return workDay;
        }

        private static bool JsonPropertyIsAvailable(dynamic? property)
        {
            return property is not null;
        }
    }
}
