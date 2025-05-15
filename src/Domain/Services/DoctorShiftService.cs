using System.Net;
using System.Net.Mime;
using System.Text;
using AppointmentManager.Domain.Entities;
using AppointmentManager.Domain.Formatters;
using AppointmentManager.Domain.Models;
using Newtonsoft.Json;

namespace AppointmentManager.Domain.Services
{
    public class DoctorShiftService : IDoctorShiftService
    {
        private const string GetWeeklyAvailabilityEndpointName = "GetWeeklyAvailability";
        private const string TakeSlotEndpointName = "TakeSlot";
        private readonly IDateRequestFormatter _dateRequestFormatter;
        private readonly HttpClient _httpClient;

        public DoctorShiftService(IDateRequestFormatter dateRequestFormatter, HttpClient httpClient)
        {
            _dateRequestFormatter = dateRequestFormatter;
            _httpClient = httpClient;
        }

        public async Task<SlotsInformation> GetSlotsInformationAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var slotsInformation = new SlotsInformation();
            var dateRequestParameter = _dateRequestFormatter.GetCompatibleDateWithSlotService(date);
            var requestUri = $"{GetWeeklyAvailabilityEndpointName}/{dateRequestParameter}";
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            if (DateRequestedToSlotServiceIsNotMonday(response, responseContent)) throw new ArgumentException($"Date '{date}' must be Monday");
            if (SlotServiceIsDown(response)) throw new TimeoutException("Slot service is down and work days shift cannot be retrieved");
            
            dynamic jsonContent = JsonConvert.DeserializeObject(responseContent);
            slotsInformation.WorkDays = GetWorkDaysInformation(date, jsonContent);
            slotsInformation.SlotDurationMinutes = jsonContent?.SlotDurationMinutes;

            return slotsInformation;
        }

        public async Task<HttpResponseMessage> AddSlotToShiftAsync(Appointment appointment, CancellationToken cancellationToken)
        {
            // TODO: Why await _httpClient.PostAsJsonAsync throws BadRequest with message `Valid slot required` from Slot service?
            var jsonObject = JsonConvert.SerializeObject(appointment);
            var content = new StringContent(jsonObject, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient.PostAsync(TakeSlotEndpointName, content, cancellationToken);
            return response;
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
                if (!WorkDayIsAvailable(propertyContent.Value)) continue;
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
                    LunchStartHour = day.WorkPeriod.LunchStartHour,
                    LunchEndHour = day.WorkPeriod.LunchEndHour,
                    EndHour = day.WorkPeriod.EndHour
                }
            };
            if (day.BusySlots is not null)
            {
                workDay.BusySlots = day.BusySlots.ToObject<List<BusySlot>>();
            }

            return workDay;
        }

        private static bool WorkDayIsAvailable(dynamic? workDay)
        {
            return workDay is not null;
        }
    }
}
